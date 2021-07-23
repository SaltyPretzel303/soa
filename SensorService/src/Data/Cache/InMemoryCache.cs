using System.Collections.Concurrent;
using System.Collections.Generic;
using CommunicationModel;
using SensorService.Configuration;
using SensorService.Logger;

public class InMemoryCache : IDataCacheManager
{

	private ILogger logger;

	private ServiceConfiguration config;

	private ConcurrentDictionary<string, CacheRecord> DataMap;

	public InMemoryCache(ILogger logger)
	{
		this.logger = logger;
		this.config = ServiceConfiguration.Instance;

		this.DataMap = new ConcurrentDictionary<string, CacheRecord>();
	}

	public void AddData(string sensorName, string csvHeader, string csvValues)
	{
		CacheRecord sensorRecord = null;
		if (this.DataMap.TryGetValue(sensorName, out sensorRecord))
		{
			sensorRecord.CsvRecords.Add(csvValues);
		}
		else
		{
			sensorRecord = new CacheRecord(csvHeader, csvValues);
			if (!DataMap.TryAdd(sensorName, sensorRecord))
			{
				// if failed to add new sensor data
				logger.logError("Failed to add records to the cache ... ");
			}
		}
	}

	public CacheRecord GetAllSensorRecords(string sensorName)
	{
		CacheRecord sensorRecord = null;
		if (this.DataMap.TryGetValue(sensorName, out sensorRecord))
		{
			return sensorRecord;
		}

		return null;
	}

	public CacheRecord GetSensorRecordsFrom(string sensorName, int index)
	{

		CacheRecord reqRecords = null;
		CacheRecord existingRecords = null;
		if (!string.IsNullOrEmpty(sensorName) &&
			this.DataMap.TryGetValue(sensorName, out existingRecords))
		{

			int existingCount = existingRecords.CsvRecords.Count;
			if (index >= 0 && index < existingCount)
			{

				reqRecords = new CacheRecord(existingRecords.CsvHeader,
											existingRecords.CsvRecords.GetRange(index, existingCount - index));

				return reqRecords;
			}

		}

		return null;
	}

	public int GetLastReadIndex(string sensorName)
	{
		CacheRecord record = null;
		if (this.DataMap.TryGetValue(sensorName, out record))
		{
			return record.CsvRecords.Count - 1;
		}

		return -1;
	}

}