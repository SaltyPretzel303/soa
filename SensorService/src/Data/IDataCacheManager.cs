using System.Collections.Generic;
using CommunicationModel;

public interface IDataCacheManager
{
	CacheRecord GetAllSensorRecords(string sensorName);

	CacheRecord GetSensorRecordsFrom(string sensorName, int index);

	void AddData(string sensorName, List<string> header, SensorValues newRecord);

	int GetLastReadIndex(string sensorName);
}