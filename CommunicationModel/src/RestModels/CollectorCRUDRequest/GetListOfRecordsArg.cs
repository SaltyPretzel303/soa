using System.Collections.Generic;

namespace CommunicationModel.RestModels.CollectorCRUDRequest
{
	public class GetListOfRecordsArg
	{
		public string sensorName { get; set; }
		public List<long> timestamps { get; set; }

		public GetListOfRecordsArg()
		{

		}

		public GetListOfRecordsArg(string sensorName, List<long> timestamps)
		{
			this.sensorName = sensorName;
			this.timestamps = timestamps;
		}


	}
}