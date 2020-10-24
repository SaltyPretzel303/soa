using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CollectorService.Broker.Events;
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

		private bool inReload;

		public BrokerEventReceiver(IMediator mediator)
		{
			this.mediator = mediator;
			this.inReload = false;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			await this.EstablishConnection(stoppingToken);

			this.SetupRegistryEventConsumer();

			// TODO maybe close previously opened connection before just throwing exception 
			stoppingToken.ThrowIfCancellationRequested();

			this.SetupConfigEventConsumer();

			ServiceConfiguration.subscribeForChange((IReloadable)this);
		}

		private async Task EstablishConnection(CancellationToken stoppingToken)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			bool connectionReady = false;
			while (!connectionReady &&
				!stoppingToken.IsCancellationRequested)
			{

				connectionReady = this.ConfigureConnection();

				await Task.Delay(config.retryConnectionDelay);
			}
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

		public void reload(ServiceConfiguration newConfig)
		{

			// this.inReload = true;

			// if (this.channel != null &&
			// this.channel.IsOpen)
			// {
			// 	this.channel.Close();
			// }

			// if (this.connection != null &&
			// this.connection.IsOpen)
			// {
			// 	this.connection.Close();
			// }



			// this.inReload = false;

			Console.WriteLine("Reloading broker event receiver ... ");

		}
	}

}
