
using CollectorService.Broker.Events;
using CollectorService.Configuration;
using CollectorService.Data.Registry;
using Newtonsoft.Json.Linq;

namespace CollectorService.Broker
{

	public delegate void DConfigurationHandler(JObject newConfiguration);
	public delegate void DRegistryChangedHandler(SensorRecord sensorRecord);

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

		public abstract void publishEvent(CollectorEvent eventToPublish);

		public abstract void shutDown();

		public abstract void subscribeForConfiguration(DConfigurationHandler configHandler);

		public abstract void reload(ServiceConfiguration newConfiguration);

		public abstract void subscribeForSensorRegistry(DRegistryChangedHandler newSensorHandler, DRegistryChangedHandler sensorRemovedHandler);

	}
}