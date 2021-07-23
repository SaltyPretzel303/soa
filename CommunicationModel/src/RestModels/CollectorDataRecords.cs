using System.Collections.Generic;

namespace CommunicationModel.RestModels
{
	public class CollectorDataRecords
	{
		public string SensorName { get; set; }
		public int RecordsCount { get; set; }

		public List<SensorValues> CsvValues { get; set; }

		public CollectorDataRecords(
			string sensorName,
			int count,
			List<SensorValues> csvValues)
		{
			this.SensorName = sensorName;
			this.RecordsCount = count;

			this.CsvValues = csvValues;
		}
	}
}