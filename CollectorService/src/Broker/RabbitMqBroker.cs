using System.Text;
using System;
using CollectorService.Broker.Events;
using CollectorService.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using CollectorService.src.Broker.Events;
using CollectorService.src.Broker.Reporter.Reports.Registry;
using RabbitMQ.Client.Exceptions;

namespace CollectorService.Broker
{

	public class RabbitMqBroker : MessageBroker
	{

		private bool connectionReady;
		public bool IsReady
		{
			get { return this.connectionReady; }
		}

		private string brokerHostName;
		private int port;

		private IConnection connection;

		private string configQueue;
		private DConfigurationHandler configurationHandler;

		private DRegistryChangedHandler newSensorHandler;
		private DRegistryChangedHandler sensorRemovedHandler;

		public RabbitMqBroker()
		{

			this.connectionReady = false;

			ServiceConfiguration configuration = ServiceConfiguration.Instance;

			this.brokerHostName = configuration.brokerAddress;
			this.port = configuration.brokerPort;

			this.createConnection();

		}

		private void createConnection()
		{
			ConnectionFactory conn_factory = new ConnectionFactory() { HostName = this.brokerHostName, Port = this.port };

			// if create connections is called from reload previous connections is still alive
			if (this.connection != null && this.connection.IsOpen)
			{
				this.connection.Close();
			}


			try
			{
				this.connection = conn_factory.CreateConnection();
			}
			catch (BrokerUnreachableException e)
			{

				Console.WriteLine("Broker unreachable exception on address: " + this.brokerHostName + ":" + this.port);

				this.connectionReady = false;

				return;

			}

			if (this.connection != null && connection.IsOpen)
			{

				IModel channel = connection.CreateModel();
				this.initBroker(channel);

				this.connectionReady = true;

			}


		}

		private void initBroker(IModel channel)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			// declare exchange for publishing collector events (reports)
			channel.ExchangeDeclare(config.serviceReportTopic, "topic", true, true, null);

		}

		override public void publishEvent(CollectorEvent eventToPublish)
		{

			if (!this.connectionReady)
			{

				this.createConnection();

				if (!this.connectionReady)
				{

					Console.WriteLine("Failed to publish event, connection can't be established ... ");
					return;

				}

			}

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			IModel channel = this.connection.CreateModel();

			byte[] content = Encoding.UTF8.GetBytes(eventToPublish.toJson().ToString());

			channel.BasicPublish(conf.serviceReportTopic, conf.collectorReportFilter, false, null, content);

			Console.WriteLine("Publishing: " + eventToPublish.toJson().ToString());

			channel.Close();

		}

		public override void shutDown()
		{

			Console.WriteLine("Closing rabbit connection ... ");

			if (this.connection != null && this.connection.IsOpen)
			{
				this.connection.Close();
			}

			Console.WriteLine(" Rabbit connection closed ...  ");

		}

		public override void subscribeForConfiguration(DConfigurationHandler configHandler)
		{

			this.configurationHandler = configHandler;

			if (this.connection == null || !this.connection.IsOpen)
			{
				this.createConnection();

				if (this.connection == null || !this.connection.IsOpen)
				{
					Console.WriteLine("Connection with broker can't be created ... @ SuscribeForConfiguration ");
					return;
				}

			}

			// declare exchange for receiving configuration
			IModel channel = this.connection.CreateModel();
			ServiceConfiguration config = ServiceConfiguration.Instance;

			channel.ExchangeDeclare(config.configurationTopic, "topic", true, true, null);

			this.configQueue = channel.QueueDeclare().QueueName;

			channel.QueueBind(this.configQueue, config.configurationTopic, config.targetConfiguration, null);

			EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
			consumer.Received += (src_channel, eventArg) =>
			{

				Console.WriteLine("New configuration received ...");

				// extract configuration 
				string sConfig = System.Text.Encoding.UTF8.GetString(eventArg.Body);
				JObject jConfig = JObject.Parse(sConfig);

				// pass it to the configHandler
				configurationHandler(jConfig);

			};

			channel.BasicConsume(this.configQueue, true, consumer);

		}

		public override void reload(ServiceConfiguration newConfiguration)
		{

			Console.WriteLine("Reloading rabbit ... ");

			this.connectionReady = false;

			ServiceConfiguration configuration = ServiceConfiguration.Instance;

			this.brokerHostName = configuration.brokerAddress;
			this.port = configuration.brokerPort;

			// "re-create" connection
			this.createConnection();

			if (this.configurationHandler != null)
			{
				this.subscribeForConfiguration(this.configurationHandler);
			}

			this.connectionReady = true;

		}

		public override void subscribeForSensorRegistry(DRegistryChangedHandler newSensorHandler, DRegistryChangedHandler sensorRemovedHandler)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			this.newSensorHandler = newSensorHandler;
			this.sensorRemovedHandler = sensorRemovedHandler;

			if (this.connection == null || !this.connection.IsOpen)
			{

				// try to create connection
				this.createConnection();

				// check is connection successfully created
				if (this.connection == null || !this.connection.IsOpen)
				{

					Console.WriteLine("Connection with broker can't be created ... @ SuscribeForSensorRegistry ");
					return;
				}

			}


			IModel channel = this.connection.CreateModel();
			string newSensorQueue = channel.QueueDeclare().QueueName;
			string sensorRemovedQueue = channel.QueueDeclare().QueueName;

			channel.ExchangeDeclare(conf.sensorRegistryTopic, "topic", true, true, null);

			channel.QueueBind(newSensorQueue, conf.sensorRegistryTopic, conf.newSensorFilter, null);
			EventingBasicConsumer newSensorConsumer = new EventingBasicConsumer(channel);
			newSensorConsumer.Received += (src_channel, eventArg) =>
			{

				Console.WriteLine("New sensor Registry event ... ");

				string payload = System.Text.Encoding.UTF8.GetString(eventArg.Body);
				JObject jPayload = JObject.Parse(payload);
				RegistryEvent rEvent = jPayload.ToObject<RegistryEvent>();
				Report report = rEvent.report;

				this.newSensorHandler(report.record);

			};
			channel.BasicConsume(newSensorQueue, true, newSensorConsumer);

			channel.QueueBind(sensorRemovedQueue, conf.sensorRegistryTopic, conf.sensorRemovedFilter, null);
			EventingBasicConsumer sensorRemovedConsumer = new EventingBasicConsumer(channel);
			sensorRemovedConsumer.Received += (srcChannel, eventArg) =>
			{

				Console.WriteLine("Sensor removed Registry event ... ");

				string payload = System.Text.Encoding.UTF8.GetString(eventArg.Body);
				JObject jPayload = JObject.Parse(payload);
				RegistryEvent rEvent = jPayload.ToObject<RegistryEvent>();
				Report report = rEvent.report;

				this.newSensorHandler(report.record);

			};
			channel.BasicConsume(sensorRemovedQueue, true, sensorRemovedConsumer);

		}
	}
}