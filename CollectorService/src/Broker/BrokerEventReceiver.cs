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

		private CancellationToken token;

		public BrokerEventReceiver(IMediator mediator)
		{
			this.mediator = mediator;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			this.token = stoppingToken;

			await this.EstablishConnection();

			if (this.token.IsCancellationRequested)
			{
				if (this.connection != null &&
				this.connection.IsOpen)
				{
					this.connection.Close();
				}

				return;
			}

			this.SetupRegistryEventConsumer();
			this.SetupConfigEventConsumer();

			ServiceConfiguration.subscribeForChange((IReloadable)this);
		}

		private async Task EstablishConnection()
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			bool connectionReady = false;
			while (!connectionReady &&
				!this.token.IsCancellationRequested)
			{

				connectionReady = this.ConfigureConnection();

				await Task.Delay(config.retryConnectionDelay);
			}

			// at this point
			// connection is ready or cancelation is requested (trough the token)
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

			ServiceConfiguration config = ServiceConfiguration.Instance;

			string sensorRegistryQueue = this.channel.QueueDeclare().QueueName;
			channel.QueueBind(sensorRegistryQueue,
							config.sensorRegistryTopic,
							config.allFilter,
							null);

			EventingBasicConsumer regEventConsumer = new EventingBasicConsumer(this.channel);
			regEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());
				SensorRegistryEvent eventData = JsonConvert.DeserializeObject<SensorRegistryEvent>(txtContent);

				this.mediator.Send(new SensorRegistryUpdateRequest(eventData));
			};

			this.channel.BasicConsume(sensorRegistryQueue,
									true,
									regEventConsumer);

		}

		private void SetupConfigEventConsumer()
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

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

		// has to be async because of connection retry 
		public async void reload(ServiceConfiguration newConfig)
		{
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

			await this.EstablishConnection();

			if (this.token.IsCancellationRequested)
			{
				if (this.connection != null &&
				this.connection.IsOpen)
				{
					this.connection.Close();
				}

				return;
			}

			this.SetupRegistryEventConsumer();
			this.SetupConfigEventConsumer();

			Console.WriteLine("Broker event receiver reloaded ... ");
		}
	}

}
