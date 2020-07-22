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

	// public class SensorRegistryRecord
	// {

	// 	public string name { get; set; }
	// 	public string address { get; set; }
	// 	public int port { get; set; }

	// 	public SensorRegistryRecord(string name, string address, int port)
	// 	{
	// 		this.name = name;
	// 		this.address = address;
	// 		this.port = port;
	// 	}
	// }

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

		RegistryResponse addSensorRecord(string sensorName, string sensorAddr, int port);

		RegistryResponse changeSensorRecord(string sensorName, string sensorAddr, int port);

		RegistryResponse removeSensorRecord(string sensorName);

		RegistryResponse getSensorAddr(string sensorName);

		RegistryResponse getAllSensors();

	}
}