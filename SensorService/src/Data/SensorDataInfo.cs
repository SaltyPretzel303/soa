using System.Collections.Generic;

namespace SensorService.Data
{
	public class SensorDataInfo
	{
		public string SensorName { get; set; }

		public string FileName { get; set; }

		public string CsvHeader { get; set; }

		public List<LineBorders> Lines { get; set; }

		public int ReadIndex { get; set; }

		public SensorDataInfo(string sensorName, string fileName)
		{
			this.SensorName = sensorName;
			this.FileName = fileName;

			this.Lines = new List<LineBorders>();

			this.ReadIndex = 0;
		}
	}

	public class LineBorders
	{
		public int StartPos { get; set; }
		public int EndPos { get; set; }
	}
}