using System.Collections.Generic;
using CommunicationModel;

namespace CollectorService.Data.Registry
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

	}
}