using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunicationModel.BrokerModels;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SensorRegistry.Broker.EventHandlers;
using SensorRegistry.Configuration;
using SensorRegistry.Logger;

namespace SensorRegistry.Broker
{
	public class BrokerEventsReceiver : BackgroundService
	{

		private ILogger logger;
		private IConfigEventHandler newConfigHandler;
		private ISensorEventHandler sensorEventHandler;

		private IConnection connection;
		private IModel channel;

		public BrokerEventsReceiver(ILogger logger,
								IConfigEventHandler newConfigHandler,
								ISensorEventHandler sensorEventHandler)
		{
			this.logger = logger;
			this.newConfigHandler = newConfigHandler;
			this.sensorEventHandler = sensorEventHandler;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			ServiceConfiguration config = ServiceConfiguration.Instance;

			#region  establish connection

			bool connectionReady = false;
			while (!connectionReady &&
				!stoppingToken.IsCancellationRequested)
			{
				connectionReady = this.ConfigureConnection();

				await Task.Delay(config.connectionRetryDelay);
			}

			Console.WriteLine("Broker connection established ... ");

			#endregion

			stoppingToken.ThrowIfCancellationRequested();

			#region setup config consumers

			string configQueue = this.channel.QueueDeclare().QueueName;
			channel.QueueBind(configQueue,
							config.configUpdateTopic,
							config.configFilter,
							null);

			EventingBasicConsumer newConfigEventConsumer = new EventingBasicConsumer(this.channel);
			newConfigEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());
				JObject jsonContent = JObject.Parse(txtContent);

				this.newConfigHandler.HandleNewConfig(jsonContent);

				this.logger.LogMessage($"Received new configuration: {jsonContent.ToString()}");
			};
			this.channel.BasicConsume(configQueue,
									true,
									newConfigEventConsumer);

			#endregion

			#region setup sensorReader event consumer

			string sensorReadEventQueue = this.channel.QueueDeclare().QueueName;

			EventingBasicConsumer sensorEventConsumer = new EventingBasicConsumer(this.channel);
			sensorEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());
				SensorReaderEvent sensorEvent = JsonConvert.DeserializeObject<SensorReaderEvent>(txtContent);

				Console.WriteLine("Received ... : " + txtContent);

				this.sensorEventHandler.HandleSensorEvent(sensorEvent);
			};

			this.channel.QueueBind(sensorReadEventQueue,
							config.sensorEventTopic,
							config.sensorReadEventFilter,
							null);

			this.channel.BasicConsume(sensorReadEventQueue,
									true,
									sensorEventConsumer);

			#endregion
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
				this.logger.LogError($"Failed to create connection with broker: {e.Message}");
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

				this.channel.ExchangeDeclare(config.sensorEventTopic,
										"topic",
										true,
										true,
										null);

				return true;
			}

			return false;
		}

		public override Task StopAsync(CancellationToken cancellationToken)
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

	}

}