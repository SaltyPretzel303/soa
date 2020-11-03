using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceObserver.Configuration;
using ServiceObserver.MediatrRequests;

namespace ServiceObserver.Broker
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

		// private CancellationToken connectionRetryToken;

		public BrokerEventReceiver(IMediator mediator)
		{
			this.mediator = mediator;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			this.masterToken = stoppingToken;
			this.masterToken.Register(() =>
			{
				if (this.connectionRetryTokenSrc != null)
				{
					this.connectionRetryTokenSrc.Cancel();
				}
			});
			// this will cancel connection retry if application shutDown is requested

			ServiceConfiguration.subscribeForReload((IReloadable)this);

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

			this.SetupConfigEventConsumer();

		}

		private Task EstablishConnection(CancellationToken connectionRetryToken)
		{

			return Task.Run(async () =>
			{
				ServiceConfiguration config = ServiceConfiguration.Instance;

				bool connectionReady = false;
				while (!connectionReady &&
					!connectionRetryToken.IsCancellationRequested)
				{

					connectionReady = this.ConfigureConnection();

					if (!connectionReady)
					{
						try
						{
							await Task.Delay(config.connectionRetryDelay, connectionRetryToken);
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
					Console.WriteLine("Conection established ... ");
				}

			});

		}

		private bool ConfigureConnection()
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

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

				this.channel.ExchangeDeclare(config.configUpdateTopic,
											"topic",
											true,
											true,
											null);

				return true;
			}

			return false;
		}

		private void SetupConfigEventConsumer()
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			string configEventQueue = this.channel.QueueDeclare().QueueName;
			this.channel.QueueBind(configEventQueue,
								config.configUpdateTopic,
								config.serviceTypeFilter,
								null);


			EventingBasicConsumer configEventConsumer = new EventingBasicConsumer(this.channel);
			configEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());

				JObject newConfig = JObject.Parse(txtContent);

				this.mediator.Send(new ConfigChangeRequest(newConfig));
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
		public async void reload(ServiceConfiguration newConfig)
		{

			// TODO add check if this service have to be reloaded
			// if address and port number are the same don't reload it 

			// in case that connection (using old config) is still not established
			if (this.connectionRetryTokenSrc != null)
			{
				// cancel previous conenction retries
				this.connectionRetryTokenSrc.Cancel();
				if (this.connectionRetryTask != null)
				{
					await this.connectionRetryTask;
				}
			}

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

			this.SetupConfigEventConsumer();

			Console.WriteLine("Broker event receiver reloaded ... ");
		}

	}
}
