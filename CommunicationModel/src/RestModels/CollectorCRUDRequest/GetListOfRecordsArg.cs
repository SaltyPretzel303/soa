using System.Collections.Generic;

namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class GetListOfRecordsArg
	{
		public string sensorName { get; set; }
		public List<string> timestamps { get; set; }

		public GetListOfRecordsArg()
		{

		}

		public GetListOfRecordsArg(string sensorName, List<string> timestamps)
		{
			this.sensorName = sensorName;
			this.timestamps = timestamps;
		}


	}
}