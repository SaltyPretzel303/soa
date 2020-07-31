using System.Collections.Generic;
using CommunicationModel;

namespace SensorRegistry.Registry
{

	public enum RegistryStatus
	{
		ok,
		sensorAlreadyExists,
		registryFull,
		noSuchRecord
	}

	public class RegistryResponse
	{

		public RegistryStatus status;
		public SensorRegistryRecord singleData;
		public List<SensorRegistryRecord> listData;

		public RegistryResponse()
		{
		}

		public RegistryResponse(RegistryStatus status, SensorRegistryRecord singleData, List<SensorRegistryRecord> listData)
		{
			this.status = status;
			this.singleData = singleData;
			this.listData = listData;
		}
	}

	public interface ISensorRegistry
	{

		RegistryResponse addSensorRecord(string sensorName, string sensorAddr, int port, int readIndex = 0);

		RegistryResponse updateSensorRecord(string name, string address, int port, int readIndex);

		RegistryResponse removeSensorRecord(string sensorName);

		RegistryResponse getSensorRecord(string sensorName);

		RegistryResponse getAllSensors();

	}
}