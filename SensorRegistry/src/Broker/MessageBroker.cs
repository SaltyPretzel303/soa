
using Newtonsoft.Json.Linq;
using SensorRegistry.Broker.Event;
using SensorRegistry.Configuration;

namespace SensorRegistry.Broker
{

	public delegate void DConfigurationHandler(JObject newConfiguration);

	// ATTENTION 
	// this singleton is bad (maybe it is not singleton at all)
	// all constructors of sublcasses have to be public
	// only reason this should be singleton is because rabbit connection is expensive, so there should be only one
	// this may not be the case for other brokers so it kinda doesn't have sense

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

		public abstract void publishEvent(RegistryEvent eventToPublish);

		public abstract void shutDown();

		public abstract void subscribeForConfiguration(DConfigurationHandler configHandler);

		public abstract void reload(ServiceConfiguration newConfiguration);

	}
}