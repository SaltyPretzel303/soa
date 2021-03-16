using CommunicationModel.BrokerModels;

namespace ServiceObserver.Broker
{
	public interface IMessageBroker
	{
		void PublishLog(ServiceLog log);

		// void PublishUnstableEvent(UnstableServiceEvent newEvent);

	}
}