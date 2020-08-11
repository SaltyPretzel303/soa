using System.Collections.Generic;

namespace CommunicationModel
{
	public class SensorDataRecords
	{

		public string SensorName { get; set; }
		public int RecordsCount { get; set; }

		// this strings should be serialized in JsonDocument
		public List<string> Records { get; set; }

		public SensorDataRecords()
		{
			Records = new List<string>();
		}

		public SensorDataRecords(string sensorName, int count, List<string> records)
		{
			this.SensorName = sensorName;
			this.RecordsCount = count;
			this.Records = records;
		}

	}
}
