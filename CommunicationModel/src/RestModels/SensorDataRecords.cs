using System.Collections.Generic;

namespace CommunicationModel
{
	public class SensorDataRecords
	{
		public string SensorName { get; set; }
		public int RecordsCount { get; set; }

		public List<SensorValues> Records { get; set; }

		public SensorDataRecords()
		{
			Records = new List<SensorValues>();
		}

		public SensorDataRecords(string sensorName,
				int count,
				List<SensorValues> records)
		{
			this.SensorName = sensorName;
			this.RecordsCount = count;
			this.Records = records;
		}
	}
}
