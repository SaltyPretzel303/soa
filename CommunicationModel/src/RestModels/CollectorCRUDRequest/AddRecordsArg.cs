using System.Collections.Generic;

namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class AddRecordsArg
	{
		public string sensorName { get; set; }
		public List<string> newRecords { get; set; }

		public AddRecordsArg()
		{

		}

		public AddRecordsArg(string sensorName, List<string> newRecords)
		{
			this.sensorName = sensorName;
			this.newRecords = newRecords;
		}

	}
}