namespace CommunicationModel.RestModels
{
	public class SensorReaderInfo
	{

		public string SensorName { get; set; }
		public string IpAddress { get; set; }
		public int ListeningPort { get; set; }
		public int LastReadIndex { get; set; }

		public SensorReaderInfo(string sensorName,
			string ipAddress,
			int listeningPort,
			int lastReadIndex)
		{
			SensorName = sensorName;
			IpAddress = ipAddress;
			ListeningPort = listeningPort;
			LastReadIndex = lastReadIndex;
		}
	}
}