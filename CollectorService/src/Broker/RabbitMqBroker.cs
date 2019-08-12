using CollectorService.Broker.Events;
using CollectorService.Configuration;
using RabbitMQ.Client;

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
		private IModel channel;

		public RabbitMqBroker()
		{

			this.ready = false;

			ServiceConfiguration configuration = ServiceConfiguration.read();

			this.brokerHostName = configuration.brokerAddress;
			this.port = configuration.brokerPort;

			this.createConnection();

		}

		private void createConnection()
		{
			ConnectionFactory conn_factory = new ConnectionFactory() { HostName = this.brokerHostName, Port = this.port };
			this.connection = conn_factory.CreateConnection();

			if (connection.IsOpen)
			{

				this.channel = connection.CreateModel();
				if (this.channel.IsOpen)
				{

					// TODO 
					// initialize channel (connection), delcare queues and topics (if needed)

					this.ready = true;

				}

			}
		}


		override public IModel getChannel()
		{
			return this.channel;
		}

		override public void publishEvent(CollectorEvent eventToPublish)
		{

			var con_factory = new ConnectionFactory()
			{
				HostName = ""
			};

			var connection = con_factory.CreateConnection();
			var channel = connection.CreateModel();

		}
	}
}