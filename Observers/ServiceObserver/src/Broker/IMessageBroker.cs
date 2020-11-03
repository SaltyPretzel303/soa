using CommunicationModel.BrokerModels;

namespace ServiceObserver.Broker
{
	public interface IMessageBroker
	{
		void PublishObserverReport(SensorRegistryEvent newEvent, string filter);

		void PublishLifetimeEvent(ServiceLifetimeEvent ltEvent);

		void PublishLog(ServiceLog log);
	}
}