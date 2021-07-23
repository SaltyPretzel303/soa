using System.Threading.Tasks;
using CommunicationModel.BrokerModels;

namespace SensorService.Broker
{
	public interface IMessageBroker
	{
		Task<bool> PublishLog(ServiceLog log);

		Task<bool> PublishSensorEvent(SensorReaderEvent sensorEvent, string filter);

		Task<bool> PublishLifetimeEvent(ServiceLifetimeEvent newEvent);
	}
}