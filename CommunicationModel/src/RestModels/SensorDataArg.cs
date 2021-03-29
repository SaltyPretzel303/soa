namespace CommunicationModel.RestModels
{
	public class SensorDataArg
	{
		public string SensorName { get; set; }
		public string IpAddress { get; set; }
		public int PortNum { get; set; }
		public int LastReadIndex { get; set; }

		public SensorDataArg()
		{
		}

		public SensorDataArg(string sensorName,
					string ipAddress,
					int portNum,
					int lastReadIndex)
		{
			SensorName = sensorName;
			IpAddress = ipAddress;
			PortNum = portNum;
			LastReadIndex = lastReadIndex;
		}
	}
}