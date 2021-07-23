using System.Collections.Generic;
using CommunicationModel;

public interface IDataCacheManager
{
	CacheRecord GetAllSensorRecords(string sensorName);

	CacheRecord GetSensorRecordsFrom(string sensorName, int index);

	void AddData(string sensorName, string csvHeader, string csvValues);

	int GetLastReadIndex(string sensorName);
}