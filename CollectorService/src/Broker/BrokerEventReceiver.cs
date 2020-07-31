using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CollectorService.Broker.Events;
using CollectorService.Configuration;
using CommunicationModel.BrokerModels;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CollectorService.Broker
{
	public class BrokerEventReceiver : BackgroundService
	{

		private IConfigChange configChangeHandler;
		private ISensorRegistryUpdate sensoreRegistryUpdateHandler;

		private IConnection connection;
		private IModel channel;

		private string sensorRegistryQueue;
		private string sensorRemovedQueue;
		private string configEventQueue;


		public BrokerEventReceiver(IConfigChange configChangeHandler,
								ISensorRegistryUpdate sensoreRegistryUpdateHandler)
		{
			this.configChangeHandler = configChangeHandler;
			this.sensoreRegistryUpdateHandler = sensoreRegistryUpdateHandler;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			this.ConfigureConnection();

			EventingBasicConsumer regEventConsumer = new EventingBasicConsumer(this.channel);
			regEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());
				SensorRegistryEvent eventData = JsonConvert.DeserializeObject<SensorRegistryEvent>(txtContent);

				this.sensoreRegistryUpdateHandler.HandleRegistryUpdate(eventData);
			};

			EventingBasicConsumer configEventConsumer = new EventingBasicConsumer(this.channel);
			regEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());
				JObject newConfig = JObject.Parse(txtContent);

				this.configChangeHandler.UpdateConfiguration(newConfig);
			};

			this.channel.BasicConsume(this.configEventQueue,
									true,
									configEventConsumer);

			this.channel.BasicConsume(this.sensorRegistryQueue,
									true,
									regEventConsumer);

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
				Console.WriteLine($"Failed to create connection with broker: {e.Message}");
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

				this.sensorRegistryQueue = this.channel.QueueDeclare().QueueName;
				channel.QueueBind(sensorRegistryQueue,
								config.sensorRegistryTopic,
								config.allFilter,
								null);

				this.configEventQueue = this.channel.QueueDeclare().QueueName;
				this.channel.QueueBind(configEventQueue,
									config.configurationTopic,
									config.targetConfiguration,
									null);

			}

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

	}

}
