using System.Collections.Generic;

namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class AddRecordsArg
	{
		public string sensorName { get; set; }
		public List<SensorValues> newRecords { get; set; }

		public AddRecordsArg()
		{

		}

		public AddRecordsArg(string sensorName, List<SensorValues> newRecords)
		{
			this.sensorName = sensorName;
			this.newRecords = newRecords;
		}

	}
}