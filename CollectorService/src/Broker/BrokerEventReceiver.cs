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
		private ISensorRegistryUpdate sensorRegistryUpdateHandler;

		private IConnection connection;
		private IModel channel;

		public BrokerEventReceiver(IConfigChange configChangeHandler,
								ISensorRegistryUpdate sensoreRegistryUpdateHandler)
		{
			this.configChangeHandler = configChangeHandler;
			this.sensorRegistryUpdateHandler = sensoreRegistryUpdateHandler;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			ServiceConfiguration config = ServiceConfiguration.Instance;

			#region establish connection

			bool connectionReady = false;
			while (!connectionReady &&
				!stoppingToken.IsCancellationRequested)
			{
				connectionReady = this.ConfigureConnection();

				await Task.Delay(config.retryConnectionDelay);
			}

			#endregion

			stoppingToken.ThrowIfCancellationRequested();

			#region setup registry event consumer

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

				this.sensorRegistryUpdateHandler.HandleRegistryUpdate(eventData);
			};

			this.channel.BasicConsume(sensorRegistryQueue,
									true,
									regEventConsumer);

			#endregion

			#region setup config event consumer

			string configEventQueue = this.channel.QueueDeclare().QueueName;
			this.channel.QueueBind(configEventQueue,
								config.configurationTopic,
								config.targetConfiguration,
								null);


			EventingBasicConsumer configEventConsumer = new EventingBasicConsumer(this.channel);
			configEventConsumer.Received += (srcChannel, eventArg) =>
			{
				string txtContent = Encoding.UTF8.GetString(eventArg.Body.ToArray());

				Console.WriteLine($"Received new configuration ... {txtContent}");

				JObject newConfig = JObject.Parse(txtContent);

				this.configChangeHandler.UpdateConfiguration(newConfig);
			};

			this.channel.BasicConsume(configEventQueue,
									true,
									configEventConsumer);

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

	}

}
