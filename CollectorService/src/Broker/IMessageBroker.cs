
using CollectorService.Broker.Events;
using CommunicationModel.BrokerModels;

namespace CollectorService.Broker
{

	public interface IMessageBroker
	{

		void PublishLifetimeEvent(ServiceLifetimeEvent newEvent);

		void PublishLog(ServiceLog newLog);

		void PublishCollectorAccessEvent(CollectorAccessEvent newEvent);

		void PublishCollectorPullEvent(CollectorPullEvent newEvent);

	}
}