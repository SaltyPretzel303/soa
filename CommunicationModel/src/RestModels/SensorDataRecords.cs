using System.Collections.Generic;

namespace CommunicationModel
{
	public class SensorDataRecords
	{
		public string SensorName { get; set; }
		public int RecordsCount { get; set; }

		public string CsvHeader { get; set; }
		public List<string> CsvValues { get; set; }

		public SensorDataRecords(
			string sensorName,
			int count,
			string csvHeader,
			List<string> csvValues)
		{
			this.SensorName = sensorName;
			this.RecordsCount = count;

			this.CsvHeader = csvHeader;
			this.CsvValues = csvValues;
		}
	}
}
