using System.Collections.Generic;
using CommunicationModel;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data
{
	public interface IDatabaseService
	{
		void addToSensor(string sensorName, SensorValues newValues);

		void addToSensor(string sensorName, List<SensorValues> newRecords);

		List<SensorModel> getAllSamples();

		SensorModel getRecordRange(string sensorName,
			long fromTimestamp = 0,
			long toTimestamp = -1);

		SensorModel getRecordsList(string sensorName, List<long> timestamps);

		bool updateRecord(string sensorName, long timestamp, string field, string value);

		bool deleteRecord(string sensorName, long timestamp);

		bool deleteSensorData(string sensorName);

		int getRecordsCount(string sensorName);

		void backupConfiguration(JObject oldConfig);
	}
}