namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class DeleteRecordArg
	{

		public string sensorName { get; set; }
		public string recordTimestamp { get; set; }

		public DeleteRecordArg()
		{

		}

		public DeleteRecordArg(string sensorName, string recordTimestamp)
		{
			this.sensorName = sensorName;
			this.recordTimestamp = recordTimestamp;

		}

	}
}