using CommunicationModel.BrokerModels;

namespace SensorService.Broker
{
	public interface IMessageBroker
	{
		void PublishLog(ServiceLog log);

		void PublishSensorEvent(SensorReaderEvent sensorEvent,string filter);

		void PublishLifetimeEvent(ServiceLifetimeEvent newEvent);
	}
}