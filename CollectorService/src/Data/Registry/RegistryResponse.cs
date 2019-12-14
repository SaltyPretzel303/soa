using System.Collections.Generic;

namespace CollectorService.Data.Registry
{


	public enum RegistryStatus
	{
		ok,
		sensorAlreadyExists,
		registryFull,
		noSuchRecord
	}

	public class SensorRecord
	{

		public string name;
		public string address;
		public int port;

		public int lastReadIndex = 0;

		public SensorRecord(string name, string address, int port, int lastReadIndex)
		{
			this.name = name;
			this.address = address;
			this.port = port;
			this.lastReadIndex = lastReadIndex;
		}
	}


	public class RegistryResponse
	{

		public RegistryStatus status;
		public SensorRecord singleData;
		public List<SensorRecord> listData;

	}
}