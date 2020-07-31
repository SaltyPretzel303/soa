namespace CommunicationModel.BrokerModels
{
	public class CollectorPullEvent : ServiceEvent
	{

		public string sensorAddress;

		public bool success;

		public CollectorPullEvent(string sensorAddress,
								bool success,
								string additionalDesc = "")
				: base(additionalDesc)
		{
			this.sensorAddress = sensorAddress;
			this.success = success;
		}

	}
}