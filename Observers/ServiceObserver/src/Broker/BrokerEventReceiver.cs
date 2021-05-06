using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunicationModel.BrokerModels;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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

			ServiceConfiguration.subscribeForReload((IReloadable)this);

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

			this.connectionRetryTask = null;
			this.connectionRetryTokenSrc = null;

			this.SetupConfigEventConsumer();
			this.SetupLifetimeEventConsumer();

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
					Console.WriteLine("Broker connection established ... ");
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

				this.channel.ExchangeDeclare(config.configUpdateTopic,
											"topic",
											true,
											true,
											null);

				this.channel.ExchangeDeclare(config.serviceLifetimeTopic,
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

			string configEventQueue = this.channel.QueueDeclare().QueueName;
			channel.QueueBind(configEventQueue,
				config.configUpdateTopic,
				config.serviceTypeFilter,
				null);


			var configEventConsumer = new EventingBasicConsumer(this.channel);
			configEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());

				JObject newConfig = JObject.Parse(txtContent);

				mediator.Send(new ConfigChangeRequest(newConfig));
			};

			channel.BasicConsume(configEventQueue,
				true,
				configEventConsumer);

		}

		private void SetupLifetimeEventConsumer()
		{

			string lifetimeQueue = this.channel.QueueDeclare().QueueName;
			this.channel.QueueBind(lifetimeQueue,
								config.serviceLifetimeTopic,
								config.lifetimeEventFilter,
								null);

			var lifetimeEventConsumer = new EventingBasicConsumer(this.channel);
			lifetimeEventConsumer.Received += (srcChannel, eventArg) =>
			{

				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());

				var newEvent = JsonConvert
					.DeserializeObject<ServiceLifetimeEvent>(txtContent);

				Console.WriteLine($"LifeEvent => {newEvent.lifeStage.ToString()}, "
					+ $"s.Type: {newEvent.sourceType}, "
					+ $"s.Name: {newEvent.sourceId}");

				mediator.Send(new SaveEventRequest(newEvent));

			};

			channel.BasicConsume(lifetimeQueue,
				true,
				lifetimeEventConsumer);

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
		public async Task reload(ConfigFields newConfig)
		{

			// TODO add check if this service has to be reloaded
			// if address and port number are the same don't reload it ...

			// in case that connection (using old config) is still not established
			if (this.connectionRetryTokenSrc != null)
			{
				// cancel previous connections retries
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

			SetupConfigEventConsumer();
			SetupLifetimeEventConsumer();

			Console.WriteLine("BrokerEventReceiver reloaded using new config ... ");
		}

	}
}
