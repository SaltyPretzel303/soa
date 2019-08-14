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

		List<JObject> getRecordsFromSensor(string sampleName, long fromTimestamp = 0, long toTimestamp = -1);

		void shutDown();

		void backupConfiguration(JObject oldConfig);

	}
}