using System.Text;
using System;
using Newtonsoft.Json.Linq;
using SensorRegistry.Configuration;
using SensorRegistry.Broker.Event;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Sockets;

namespace SensorRegistry.Broker
{

	public class RabbitMqBroker : MessageBroker
	{

		private bool ready;
		public bool IsReady
		{
			get { return this.ready; }
		}

		private IConnection connection;

		private string configQueue;
		private DConfigurationHandler configurationHandler;


		public RabbitMqBroker()
		{

			this.ready = false;

			this.createConnection();

		}

		private void createConnection()
		{

			Console.WriteLine("Trying to create connection with rabbitMQ");

			ServiceConfiguration conf = ServiceConfiguration.read();
			ConnectionFactory conn_factory = new ConnectionFactory() { HostName = conf.brokerAddress, Port = conf.brokerPort };

			// if create connections is called from reload previous connections is still alive
			if (this.connection != null && this.connection.IsOpen)
			{
				this.connection.Close();
			}

			try
			{
				this.connection = conn_factory.CreateConnection();
			}
			// TODO handle aggregate exception for fine error handling 
			catch (Exception e)
			{
				Console.WriteLine("Failed to connecto to the broker ... ");
				Console.WriteLine(e.ToString());

			}

			if (this.connection != null && this.connection.IsOpen)
			{

				IModel channel = connection.CreateModel();
				this.initBroker(channel);

				this.ready = true;

			}
			else
			{
				Console.WriteLine("Failed to establish connection with rabbitMQ ... ");
			}
		}

		private void initBroker(IModel channel)
		{

			ServiceConfiguration config = ServiceConfiguration.read();

			// declare exchange for publishing collector events (reports)
			channel.ExchangeDeclare(config.serviceReportTopic, "topic", true, true, null);
			channel.ExchangeDeclare(config.sensorRegistryTopic, "topic", true, true, null);


		}

		public override void publishEvent(RegistryEvent eventToPublish, string topic)
		{

			ServiceConfiguration conf = ServiceConfiguration.read();

			if (this.connection != null && !this.connection.IsOpen)
			{
				this.createConnection();
			}

			if (this.connection == null || !this.connection.IsOpen)
			{
				// tried to create connection but some exception occured ...  
				return;
			}

			IModel channel = this.connection.CreateModel();

			byte[] content = Encoding.UTF8.GetBytes(eventToPublish.toJson().ToString());

			// TODO borker doesn't have to know about event type
			// TODO handle exceptions for publish event 
			channel.BasicPublish(conf.sensorRegistryTopic, topic, false, null, content);

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
			ServiceConfiguration config = ServiceConfiguration.read();

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

			ServiceConfiguration configuration = ServiceConfiguration.read();

			// "re-create" connection
			this.createConnection();

			if (this.configurationHandler != null)
			{
				this.subscribeForConfiguration(this.configurationHandler);
			}

			this.ready = true;

		}

	}
}