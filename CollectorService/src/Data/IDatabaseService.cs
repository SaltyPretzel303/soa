using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data
{
	public interface IDatabaseService
	{
		void pushToSensor(String sampleName, JArray values);

		List<JObject> getAllSamples();

		List<JObject> getRecordsFromSensor(string sampleName, long fromTimestamp = 0, long toTimestamp = -1);

	}
}