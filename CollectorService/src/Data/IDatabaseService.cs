using System;
using System.Collections.Generic;
using CollectorService.Configuration;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data
{
	public interface IDatabaseService : IReloadable
	{
		void pushToSensor(String sampleName, JArray values);

		List<JObject> getAllSamples();

		List<JObject> getRecordsFromSensor(string sensorName, long fromTimestamp = 0, long toTimestamp = -1);

		int getRecordsCount(string sensorName);

		void shutDown();

		void backupConfiguration(JObject oldConfig);

		// used just for testing when accessing home url 
		List<JObject> customQuery();

	}
}