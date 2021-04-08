using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CollectorService.Configuration;
using CollectorService.MediatrRequests;
using CommunicationModel.BrokerModels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CollectorService.Broker
{
	public class BrokerEventReceiver : BackgroundService, IReloadable
	{

		// provided from DI
		private IMediator mediator;

		private IConnection connection;
		private IModel channel;

		private CancellationToken masterToken;

		private CancellationTokenSource connectionRetryTokenSrc;
		private Task connectionRetryTask;

		private ConfigFields config;

		// private CancellationToken connectionRetryToken;

		public BrokerEventReceiver(IMediator mediator)
		{
			this.mediator = mediator;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			this.config = ServiceConfiguration.Instance;

			this.masterToken = stoppingToken;
			this.masterToken.Register(() =>
			{
				if (this.connectionRetryTokenSrc != null)
				{
					this.connectionRetryTokenSrc.Cancel();
				}
			});
			// this will cancel connection retry if application shutDown is requested

			ServiceConfiguration.subscribeForChange((IReloadable)this);

			this.connectionRetryTokenSrc = new CancellationTokenSource();

			this.connectionRetryTask = this.EstablishConnection(this.connectionRetryTokenSrc.Token);
			// Console.WriteLine("Before await ... exec async ...");
			await this.connectionRetryTask;
			// Console.WriteLine("After await ... exec async ...");

			if (this.masterToken.IsCancellationRequested)
			{
				if (this.connection != null &&
				this.connection.IsOpen)
				{
					this.connection.Close();
				}

				return;
			}

			this.connectionRetryTask = null;
			this.connectionRetryTokenSrc = null;

			this.SetupRegistryEventConsumer();
			this.SetupConfigEventConsumer();

		}

		private Task EstablishConnection(CancellationToken connectionRetryToken)
		{

			return Task.Run(async () =>
			{
				bool connectionReady = false;
				while (!connectionReady &&
					!connectionRetryToken.IsCancellationRequested)
				{

					connectionReady = this.ConfigureConnection();

					if (!connectionReady)
					{
						try
						{
							await Task.Delay(config.retryConnectionDelay, connectionRetryToken);
						}
						catch (TaskCanceledException)
						{
							break;
						}
					}

				}

				if (connectionRetryToken.IsCancellationRequested)
				{
					Console.WriteLine("Cancel conn. retry ... ");
					if (this.connection != null &&
					this.connection.IsOpen)
					{
						this.connection.Close();
					}
				}
				else
				{
					Console.WriteLine("Connection established ... ");
				}

			});

		}

		private bool ConfigureConnection()
		{

			ConnectionFactory connectionFactory = new ConnectionFactory
			{
				HostName = config.brokerAddress,
				Port = config.brokerPort
			};
			try
			{
				this.connection = connectionFactory.CreateConnection();
				this.channel = this.connection.CreateModel();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to create connection with broker: {e.Message}");

				return false;
			}

			if (this.connection != null &&
				this.connection.IsOpen &&
				this.channel != null &&
				this.channel.IsOpen)
			{

				this.channel.ExchangeDeclare(config.sensorRegistryTopic,
											"topic",
											true,
											true,
											null);
				this.channel.ExchangeDeclare(config.configurationTopic,
											"topic",
											true,
											true,
											null);

				return true;
			}

			return false;
		}

		private void SetupRegistryEventConsumer()
		{

			string sensorRegistryQueue = this.channel.QueueDeclare().QueueName;
			channel.QueueBind(sensorRegistryQueue,
							config.sensorRegistryTopic,
							config.allFilter,
							null);

			EventingBasicConsumer regEventConsumer = new EventingBasicConsumer(this.channel);
			regEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());
				SensorRegistryEvent eventData =
					JsonConvert.DeserializeObject<SensorRegistryEvent>(txtContent);

				mediator.Send(new SensorRegistryUpdateRequest(eventData));
			};

			this.channel.BasicConsume(sensorRegistryQueue,
									true,
									regEventConsumer);

		}

		private void SetupConfigEventConsumer()
		{

			string configEventQueue = this.channel.QueueDeclare().QueueName;
			this.channel.QueueBind(configEventQueue,
								config.configurationTopic,
								config.targetConfiguration,
								null);


			EventingBasicConsumer configEventConsumer = new EventingBasicConsumer(this.channel);
			configEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());

				JObject newConfig = JObject.Parse(txtContent);

				mediator.Send(new ConfigChangeRequest(newConfig));
			};

			this.channel.BasicConsume(configEventQueue,
									true,
									configEventConsumer);

		}


		public override Task StopAsync(CancellationToken stoppingToken)
		{
			if (this.channel != null && this.channel.IsOpen)
			{
				this.channel.Close();
			}

			if (this.connection != null && this.connection.IsOpen)
			{
				this.connection.Close();
			}

			return Task.CompletedTask;
		}

		// has to be async because of the connection retry 
		public async void reload(ConfigFields newConfig)
		{

			// TODO add check if this service has to be reloaded
			// if address and port number are the same don't reload it 

			// in case that connection (using old config) is still not established
			if (this.connectionRetryTokenSrc != null)
			{
				// cancel previous connection retries
				this.connectionRetryTokenSrc.Cancel();
				if (this.connectionRetryTask != null)
				{
					await this.connectionRetryTask;
				}
			}

			this.config = newConfig;

			if (this.channel != null &&
			this.channel.IsOpen)
			{
				this.channel.Close();
			}

			if (this.connection != null &&
			this.connection.IsOpen)
			{
				this.connection.Close();
			}

			this.connectionRetryTokenSrc = new CancellationTokenSource();

			this.connectionRetryTask = this.EstablishConnection(this.connectionRetryTokenSrc.Token);
			await this.connectionRetryTask;

			if (this.masterToken.IsCancellationRequested)
			{
				if (this.connection != null &&
				this.connection.IsOpen)
				{
					this.connection.Close();
				}

				return;
			}

			this.connectionRetryTokenSrc = null;

			if (this.connection == null ||
			!this.connection.IsOpen)
			{
				return;
			}

			this.SetupRegistryEventConsumer();
			this.SetupConfigEventConsumer();

			Console.WriteLine("Broker event receiver reloaded ... ");
		}

	}
}
