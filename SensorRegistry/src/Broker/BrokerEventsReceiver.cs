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
using SensorRegistry.Broker.Events;
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

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			this.ConfigureConnection();

			ServiceConfiguration config = ServiceConfiguration.Instance;

			Console.WriteLine("Consumer set ");

			#region config consumer

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

			#region sensor reader consumer

			string sensorEventQueue = this.channel.QueueDeclare().QueueName;

			EventingBasicConsumer sensorEventConsumer = new EventingBasicConsumer(this.channel);
			sensorEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());
				SensorReaderEvent sensorEvent = JsonConvert.DeserializeObject<SensorReaderEvent>(txtContent);

				Console.WriteLine("Received ... : " + txtContent);

				this.sensorEventHandler.HandleSensorEvent(sensorEvent);
			};
			#endregion

			this.channel.QueueBind(sensorEventQueue,
							config.sensorEventTopic,
							"",
							null);

			this.channel.BasicConsume(sensorEventQueue,
									true,
									sensorEventConsumer);

			return Task.CompletedTask;
		}

		private void ConfigureConnection()
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

			}
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