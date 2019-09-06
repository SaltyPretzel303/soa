using System.Collections.Generic;

namespace SensorRegistry.Registry
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

		public SensorRecord(string name, string address, int port)
		{
			this.name = name;
			this.address = address;
			this.port = port;
		}
	}

	public class RegistryResponse
	{

		public RegistryStatus status;
		public SensorRecord singleData;
		public List<SensorRecord> listData;

		public RegistryResponse()
		{
		}

		public RegistryResponse(RegistryStatus status, SensorRecord singleData, List<SensorRecord> listData)
		{
			this.status = status;
			this.singleData = singleData;
			this.listData = listData;
		}
	}

	public interface ISensorRegistry
	{

		RegistryResponse addSensorRecord(string sensorName, string sensorAddr, int port);

		RegistryResponse changeSensorRecord(string sensorName, string sensorAddr, int port);

		RegistryResponse removeSensorRecord(string sensorName);

		RegistryResponse getSensorAddr(string sensorName);

		RegistryResponse getAllSensors();

	}
}