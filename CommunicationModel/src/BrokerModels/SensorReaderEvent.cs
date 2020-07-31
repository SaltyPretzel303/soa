namespace CommunicationModel.BrokerModels
{

	public class SensorInfo
	{
		public string SensorName { get; set; }
		public int LastReadIndex { get; set; }
		public string AdditionalDesc { get; set; }

		public SensorInfo(string sensorName, int lastReadIndex, string additionalDesc = "")
		{
			this.SensorName = sensorName;
			this.LastReadIndex = lastReadIndex;
			this.AdditionalDesc = additionalDesc;
		}

	}

	public class SensorReaderEvent : ServiceEvent
	{
		public string SensorName { get; set; }
		public int LastReadIndex { get; set; }
		public string AdditionalDesc { get; set; }

		public SensorReaderEvent(string sensorName,
							int lastReadIndex,
							string additionalDesc = "",
							string customMessage = "") :
			base(customMessage)
		{
			SensorName = sensorName;
			LastReadIndex = lastReadIndex;
			AdditionalDesc = additionalDesc;
		}
	}

}