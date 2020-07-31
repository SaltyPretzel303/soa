using CommunicationModel.BrokerModels;

namespace SensorRegistry.Broker
{

	public interface IMessageBroker
	{
		void publishRegistryEvent(SensorRegistryEvent newEvent, string filter);

		void publishLifetimeEvent(ServiceLifetimeEvent ltEvent);

		void publishLog(ServiceLog log);
	}

}