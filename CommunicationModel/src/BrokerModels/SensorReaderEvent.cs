namespace CommunicationModel.BrokerModels
{

	public class SensorReaderEvent : ServiceEvent
	{
		public string SensorName { get; set; }
		public int LastReadIndex { get; set; }

		public string IpAddress { get; set; }
		public int ListeningPort { get; set; }

		public SensorReaderEvent(string sensorName,
							int lastReadIndex,
							string ipAddress,
							int listeningPort,
							string customMessage = "") :
			base(ServiceType.SensorReader, customMessage)
		{
			SensorName = sensorName;
			LastReadIndex = lastReadIndex;

			IpAddress = ipAddress;
			ListeningPort = listeningPort;
		}
	}

}