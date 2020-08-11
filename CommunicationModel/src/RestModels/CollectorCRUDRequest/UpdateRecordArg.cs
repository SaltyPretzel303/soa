namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class UpdateRecordArg
	{
		public string sensorName { get; set; }
		public string timestamp { get; set; }
		public string field { get; set; }
		public string value { get; set; }

		public UpdateRecordArg()
		{

		}

		public UpdateRecordArg(string sensorName, string timestamp, string field, string value)
		{
			this.sensorName = sensorName;
			this.timestamp = timestamp;
			this.field = field;
			this.value = value;
		}

	}
}