using System;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceObserver.Configuration;
using ServiceObserver.Report;
using ServiceObserver.Report.Processor;

namespace ServiceObserver.Broker
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

		// initialized after subscribe for configuration call 
		private string configQueue;
		private DConfigurationHandler configurationHandler;

		// initialized after subscribe for reports call
		private string reportsQueue;
		private ReportProcessor reportProcessor;

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

			// if create connections is called from reload previous connections is still alive
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
			consumer.Received += (srcChannel, eventArg) =>
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

			this.ready = true;

		}

		public override void subscribeForReports(ReportProcessor reportProcessor)
		{

			this.reportProcessor = reportProcessor;

			IModel channel = this.connection.CreateModel();

			ServiceConfiguration config = ServiceConfiguration.Instance;
			channel.ExchangeDeclare(config.serviceReportTopic, "topic", true, true, null);

			this.reportsQueue = channel.QueueDeclare().QueueName;

			// match all routing keys
			channel.QueueBind(this.reportsQueue, config.serviceReportTopic, config.serviceReportFilter, null);
			EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
			consumer.Received += (srcChannel, eventArg) =>
			{

				Console.WriteLine("Consuming service report ...  ");

				JObject jContent = JObject.Parse(System.Text.Encoding.UTF8.GetString(eventArg.Body));
				Console.WriteLine("Report: " + jContent.ToString());

				ServiceReportEvent reportEvent = jContent.ToObject<ServiceReportEvent>();

				reportProcessor.processReport(reportEvent);


			};
			channel.BasicConsume(this.reportsQueue, true, consumer);

		}

		public override void publishServiceEvent(ServiceEvent serviceEvent)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			IModel channel = this.connection.CreateModel();
			channel.ExchangeDeclare(config.serviceQosTopic, "topic", true, true, null);

			byte[] byteContent = System.Text.Encoding.UTF8.GetBytes(serviceEvent.toJson().ToString());

			channel.BasicPublish(config.serviceQosTopic, "", false, null, byteContent);

		}

	}
}