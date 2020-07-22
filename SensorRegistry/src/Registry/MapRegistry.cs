using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using CommunicationModel;

namespace SensorRegistry.Registry
{

	public class MapRegistry : ISensorRegistry
	{


		private ConcurrentDictionary<string, SensorRegistryRecord> registry;

		// constructors

		public MapRegistry()
		{
			this.registry = new ConcurrentDictionary<string, SensorRegistryRecord>();
		}

		// helper methods

		#region responses

		private RegistryResponse okResponse(SensorRegistryRecord data)
		{

			RegistryResponse response = new RegistryResponse();
			response.status = RegistryStatus.ok;
			response.singleData = data;

			return response;

		}

		private RegistryResponse okResponse(List<SensorRegistryRecord> data)
		{

			RegistryResponse response = new RegistryResponse();
			response.status = RegistryStatus.ok;
			response.listData = data;

			return response;

		}

		private RegistryResponse badResponse(RegistryStatus status)
		{

			RegistryResponse response = new RegistryResponse();
			response.status = status;

			return response;

		}

		#endregion responses

		// interface implementation

		public RegistryResponse addSensorRecord(string sensorName, string sensorAddr, int sensorPort)
		{

			SensorRegistryRecord newRecord = new SensorRegistryRecord(sensorName, sensorAddr, sensorPort);

			if (this.registry.TryAdd(sensorName, newRecord))
			{
				return this.okResponse(newRecord);
			}

			return this.badResponse(RegistryStatus.sensorAlreadyExists);

		}

		// ATTENTION this method may not be thread safe 
		public RegistryResponse changeSensorRecord(string sensorName, string sensorAddr, int sensorPort)
		{

			SensorRegistryRecord record = null;
			if (this.registry.TryGetValue(sensorName, out record))
			{

				record.address = sensorAddr;
				record.port = sensorPort;

				return this.okResponse(record);
			}

			return this.badResponse(RegistryStatus.noSuchRecord);

		}

		public RegistryResponse removeSensorRecord(string sensorName)
		{

			SensorRegistryRecord record = null;
			if (this.registry.TryRemove(sensorName, out record))
			{
				return this.okResponse(record);
			}

			return this.badResponse(RegistryStatus.noSuchRecord);



		}

		public RegistryResponse getAllSensors()
		{

			return this.okResponse(this.registry.Values.ToList());

		}

		public RegistryResponse getSensorAddr(string sensorName)
		{

			SensorRegistryRecord record = null;
			if (this.registry.TryGetValue(sensorName, out record))
			{

				return this.okResponse(record);

			}

			return this.badResponse(RegistryStatus.noSuchRecord);

		}

	}
}