using System.Collections.Generic;

public interface IDataCacheManager
{
	CacheRecord GetAllSensorRecords(string sensorName);

	CacheRecord GetSensorRecordsFrom(string sensorName, int index);

	void AddData(string sensorName, List<string> header, string newRecords);

}