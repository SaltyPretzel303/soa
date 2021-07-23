namespace CommunicationModel.BrokerModels
{
	public enum SensorRegEventType
	{
		NewSensor,
		SensorRemoved,
		SensorUpdated
	}

	public class SensorRegistryEvent : ServiceEvent
	{
		public SensorRegEventType eventType;
		public SensorRegistryRecord sensorRecord;

		public SensorRegistryEvent(
			string serviceId,
			SensorRegEventType eventType,
			SensorRegistryRecord sensorRecord,
			string customMessage = "")

			: base(serviceId, ServiceType.SensorRegistry, customMessage)
		{
			this.eventType = eventType;
			this.sensorRecord = sensorRecord;
		}
	}
}