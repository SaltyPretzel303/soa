using System.Text;
using System;
using CollectorService.Broker.Events;
using CollectorService.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;

namespace CollectorService.Broker
{

	public class RabbitMqBroker : MessageBroker
	{

		private bool ready;
		public bool IsReady
		{
			get { return this.ready; }
		}

		private string brokerHostName;
		private int port;

		private IConnection connection;

		private string configQueue;
		private DConfigurationHandler configurationHandler;


		public RabbitMqBroker()
		{

			this.ready = false;

			ServiceConfiguration configuration = ServiceConfiguration.Instance;

			this.brokerHostName = configuration.brokerAddress;
			this.port = configuration.brokerPort;

			this.createConnection();

		}

		private void createConnection()
		{
			ConnectionFactory conn_factory = new ConnectionFactory() { HostName = this.brokerHostName, Port = this.port };

			if (this.connection != null && this.connection.IsOpen)
			{
				this.connection.Close();
			}

			this.connection = conn_factory.CreateConnection();

			if (connection.IsOpen)
			{

				IModel channel = connection.CreateModel();
				this.initBroker(channel);

				this.ready = true;

			}
		}

		private void initBroker(IModel channel)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			// declare exchange for publishing collector events (reports)
			channel.ExchangeDeclare(config.collectorReportTopic, "topic", true, true, null);

		}

		override public IModel getChannel()
		{
			return this.connection.CreateModel();
		}

		override public void publishEvent(CollectorEvent eventToPublish)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			IModel channel = this.connection.CreateModel();

			byte[] content = Encoding.UTF8.GetBytes(eventToPublish.toJson().ToString());

			channel.BasicPublish(conf.collectorReportTopic, "", false, null, content);

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

			this.ready = false;

			ServiceConfiguration configuration = ServiceConfiguration.Instance;

			this.brokerHostName = configuration.brokerAddress;
			this.port = configuration.brokerPort;

			// "re-create" connection
			this.createConnection();

			if (this.configurationHandler != null)
			{
				this.subscribeForConfiguration(this.configurationHandler);
			}


		}
	}
}