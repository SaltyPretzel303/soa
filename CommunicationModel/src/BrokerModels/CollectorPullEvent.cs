namespace CommunicationModel.BrokerModels
{
	public class CollectorPullEvent : ServiceEvent
	{

		public string sensorAddress { get; set; }
		public bool success { get; set; }
		public int returnedCount { get; set; }

		public CollectorPullEvent(
				string serviceId,
				string sensorAddress,
				bool success,
				int returnedCount,
				string additionalDesc = "")

				: base(serviceId, ServiceType.DataCollector, additionalDesc)
		{
			this.sensorAddress = sensorAddress;
			this.success = success;
			this.returnedCount = returnedCount;
		}

	}
}