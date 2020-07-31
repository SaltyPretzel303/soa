using System.Collections.Generic;
using CommunicationModel;

public interface IRegistryCache
{
	List<SensorRegistryRecord> GetAllSensors();

	SensorRegistryRecord GetSingleSensor(string sensorName);

	void RemoveRecord(SensorRegistryRecord oldRecord);

	void AddNewRecord(SensorRegistryRecord newRecord);

	void UpdateRecord(SensorRegistryRecord newRecord);

}