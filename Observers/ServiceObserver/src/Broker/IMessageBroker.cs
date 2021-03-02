using CommunicationModel.BrokerModels;

namespace ServiceObserver.Broker
{
	public interface IMessageBroker
	{
		void PublishObserverReport(SensorRegistryEvent newEvent, string filter);

		// may be confusing because this is the only service that is consuming this events
		// it is not necessary 
		void PublishLifetimeEvent(ServiceLifetimeEvent ltEvent);

		void PublishLog(ServiceLog log);
	}
}