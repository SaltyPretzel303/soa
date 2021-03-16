namespace CommunicationModel.BrokerModels
{
	public class CollectorPullEvent : ServiceEvent
	{

		public string sensorAddress;

		public bool success;

		public CollectorPullEvent(string sensorAddress,
								bool success,
								string additionalDesc = "")
				: base(ServiceType.DataCollector, additionalDesc)
		{
			this.sensorAddress = sensorAddress;
			this.success = success;
		}

	}
}