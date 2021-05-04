using System.Collections.Generic;
using System.Threading.Tasks;
using CommunicationModel;

public interface IRegistryCache
{
	Task<List<SensorRegistryRecord>> GetAllRecords();

	SensorRegistryRecord GetRecord(string sensorName);

	void RemoveRecord(SensorRegistryRecord oldRecord);

	Task AddNewRecord(SensorRegistryRecord newRecord);

	void UpdateRecord(SensorRegistryRecord newRecord);

}