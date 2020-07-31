using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using CommunicationModel;
using SensorRegistry.Broker;
using CommunicationModel.BrokerModels;
using SensorRegistry.Configuration;

namespace SensorRegistry.Registry
{
	public class MapRegistry : ISensorRegistry
	{

		private ConcurrentDictionary<string, SensorRegistryRecord> registry;

		private IMessageBroker broker;

		public MapRegistry(IMessageBroker broker)
		{
			this.broker = broker;

			this.registry = new ConcurrentDictionary<string, SensorRegistryRecord>();
		}

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

		#region ISensorRegistry

		public RegistryResponse addSensorRecord(string sensorName, string sensorAddr, int sensorPort, int readIndex)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			SensorRegistryRecord newRecord = new SensorRegistryRecord(sensorName, sensorAddr, sensorPort, readIndex);

			if (this.registry.TryAdd(sensorName, newRecord))
			{

				SensorRegistryEvent newEvent = new SensorRegistryEvent(SensorRegEventType.NewSensor,
																	newRecord);
				this.broker.publishRegistryEvent(newEvent,
												config.newSensorFilter);

				return this.okResponse(newRecord);
			}

			return this.badResponse(RegistryStatus.sensorAlreadyExists);

		}

		// ATTENTION this method may not be thread safe 
		public RegistryResponse updateSensorRecord(string name, string address, int port, int readIndex)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			SensorRegistryRecord oldRecord = null;
			if (this.registry.TryRemove(name, out oldRecord))
			{

				SensorRegistryRecord newRecord = new SensorRegistryRecord(name, address, port, readIndex);
				this.registry.TryAdd(name, newRecord);

				SensorRegistryEvent newEvent = new SensorRegistryEvent(SensorRegEventType.SensorUpdated,
																	newRecord);
				this.broker.publishRegistryEvent(newEvent,
												config.sensorUpdatedFilter);

				return this.okResponse(newRecord);
			}

			return this.badResponse(RegistryStatus.noSuchRecord);

		}

		public RegistryResponse removeSensorRecord(string sensorName)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			SensorRegistryRecord record = null;
			if (this.registry.TryRemove(sensorName, out record))
			{

				SensorRegistryEvent newEvent = new SensorRegistryEvent(SensorRegEventType.SensorUpdated,
																	record);
				this.broker.publishRegistryEvent(newEvent,
												config.sensorRemovedFilter);

				return this.okResponse(record);
			}

			return this.badResponse(RegistryStatus.noSuchRecord);
		}

		public RegistryResponse getAllSensors()
		{
			return this.okResponse(this.registry.Values.ToList());
		}

		public RegistryResponse getSensorRecord(string sensorName)
		{
			SensorRegistryRecord record = null;
			if (this.registry.TryGetValue(sensorName, out record))
			{
				return this.okResponse(record);
			}

			return this.badResponse(RegistryStatus.noSuchRecord);
		}

		#endregion
	}

}