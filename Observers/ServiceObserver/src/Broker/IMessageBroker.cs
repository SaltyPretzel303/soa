using CommunicationModel.BrokerModels;

namespace ServiceObserver.Broker
{
	public interface IMessageBroker
	{
		void PublishLog(ServiceLog log);

		void PublishObserverReport(ServiceEvent newEvent,string filter);

	}
}