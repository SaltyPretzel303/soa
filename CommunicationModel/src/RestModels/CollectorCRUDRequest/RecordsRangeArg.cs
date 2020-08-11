namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class RecordsRangeArg
	{

		public string sensorName { get; set; }
		public int fromTimestamp { get; set; }
		public int toTimestamp { get; set; }

		public RecordsRangeArg()
		{

		}

		public RecordsRangeArg(string sensorName, int fromTimestamp, int toTimestamp)
		{
			this.sensorName = sensorName;
			this.fromTimestamp = fromTimestamp;
			this.toTimestamp = toTimestamp;

		}



	}
}