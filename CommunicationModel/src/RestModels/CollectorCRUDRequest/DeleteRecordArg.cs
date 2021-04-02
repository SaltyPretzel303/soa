namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class DeleteRecordArg
	{

		public string sensorName { get; set; }
		public long timestamp { get; set; }

		public DeleteRecordArg()
		{

		}

		public DeleteRecordArg(string sensorName, long timestamp)
		{
			this.sensorName = sensorName;
			this.timestamp = timestamp;

		}

	}
}