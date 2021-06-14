using System.Collections.Generic;
using System.Threading.Tasks;
using CollectorService.Configuration;
using CommunicationModel;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data
{
	public interface IDatabaseService
	{
		Task<bool> AddToSensor(string sensorName, SensorValues newValues);

		Task<bool> AddToSensor(string sensorName, List<SensorValues> newRecords);

		Task<List<SensorModel>> GetAllSamples();

		Task<SensorModel> getRecordRange(string sensorName,
			long fromTimestamp = 0,
			long toTimestamp = long.MaxValue);

		Task<SensorModel> getRecordsList(string sensorName, List<long> timestamps);

		Task<bool> updateRecord(string sensorName, long timestamp, string field, string value);

		Task<bool> deleteRecord(string sensorName, long timestamp);

		Task<bool> deleteSensorData(string sensorName);

		Task<int> getRecordsCount(string sensorName);

		Task backupConfiguration(ServiceConfiguration oldConfig);
	}
}