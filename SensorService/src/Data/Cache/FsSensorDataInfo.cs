namespace SensorService.Data
{
	public class FsSensorDataInfo : SensorDataInfo
	{
		public object fileLock;

		public FsSensorDataInfo(string sensorName, string fileName)
			: base(sensorName, fileName)
		{
			this.fileLock = new object();
		}

		public LineBorders getLastLine()
		{
			int index = this.Lines.Count - 1;
			return this.Lines[index];
		}

	}
}