using System;
using System.Collections.Generic;
using CollectorService.Configuration;
using CommunicationModel;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data
{
	public interface IDatabaseService : IReloadable
	{
		void pushToSensor(String sampleName, JArray values);

		List<JObject> getAllSamples();

		SensorDataRecords getRecordRange(string sensorName, long fromTimestamp = 0, long toTimestamp = -1);

		SensorDataRecords getRecordsList(string sensorName, List<string> timestamps);

		bool updateRecord(string sensorName, string timestamp, string field, string value);

		bool deleteRecord(string sensorName, string timestamp);

		bool deleteSensorData(string sensorName);

		int getRecordsCount(string sensorName);

		void backupConfiguration(JObject oldConfig);

		// used just for testing when accessing home url 
		List<JObject> customQuery();

	}
}