using Newtonsoft.Json.Linq;
using ServiceObserver.Configuration;
using ServiceObserver.Report;
using ServiceObserver.Report.Processor;

namespace ServiceObserver.Broker
{

	public delegate void DConfigurationHandler(JObject newConfiguration);

	// attention same comment as in collector service
	// bad pattern
	public abstract class MessageBroker : IReloadable
	{

		private static MessageBroker instance;
		public static MessageBroker Instance
		{
			get
			{
				if (MessageBroker.instance == null)
				{

					// attention this is the place for changing Broker implementation

					MessageBroker.instance = new RabbitMqBroker();
				}

				return MessageBroker.instance;
			}
			private set { MessageBroker.instance = value; }
		}

		protected MessageBroker()
		{

		}

		public abstract void subscribeForReports(ReportProcessor reportProcessor);

		public abstract void subscribeForConfiguration(DConfigurationHandler configurationHandler);

		public abstract void publishServiceEvent(ServiceEvent serviceEvent);

		public abstract void shutDown();

		public abstract void reload(ServiceConfiguration newConfig);
	}
}