using CommunicationModel;
using CommunicationModel.BrokerModels;
using SensorRegistry.Registry;

namespace SensorRegistry.Broker.EventHandlers
{

	public interface ISensorEventHandler
	{
		void HandleSensorEvent(SensorReaderEvent newEvent);
	}

	public class NewSensorEventHandler : ISensorEventHandler
	{

		private ISensorRegistry localRegistry;

		public NewSensorEventHandler(ISensorRegistry localRegistry)
		{
			this.localRegistry = localRegistry;
		}

		public void HandleSensorEvent(SensorReaderEvent newEvent)
		{
			RegistryResponse regResponse = this.localRegistry.getSensorRecord(newEvent.SensorName);
			if (regResponse.status == RegistryStatus.ok)
			{

				regResponse.singleData.LastReadIndex = newEvent.LastReadIndex;
				this.localRegistry.updateSensorRecord(regResponse.singleData.Name,
													regResponse.singleData.Address,
													regResponse.singleData.Port,
													regResponse.singleData.LastReadIndex);

			}
		}
	}
}