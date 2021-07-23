using System.Collections.Generic;

namespace CommunicationModel.BrokerModels
{

	public class SensorReaderEvent : ServiceEvent
	{
		public string SensorName { get; set; }
		public int LastReadIndex { get; set; }

		public string IpAddress { get; set; }
		public int ListeningPort { get; set; }

		public List<string> DataHeader { get; set; }
		public SensorValues NewData { get; set; }

		public SensorReaderEvent(
			string serviceId,
			string sensorName,
			List<string> dataHeader,
			int lastReadIndex,
			SensorValues newData,
			string ipAddress,
			int listeningPort,
			string customMessage = "")
			
			: base(serviceId, ServiceType.SensorReader, customMessage)
		{
			SensorName = sensorName;
			LastReadIndex = lastReadIndex;

			IpAddress = ipAddress;
			ListeningPort = listeningPort;
			DataHeader = dataHeader;
			NewData = newData;
		}
	}

}