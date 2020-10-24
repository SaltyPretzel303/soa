using CollectorService.Data;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class GetRecordsCountRequest : IRequest<int>
	{
		public string SensorName { get; private set; }

		public GetRecordsCountRequest(string sensorName)
		{
			SensorName = sensorName;
		}
	}

	public class GetRecordsCountRequestHandler : RequestHandler<GetRecordsCountRequest, int>
	{

		private IDatabaseService database;

		public GetRecordsCountRequestHandler(IDatabaseService database)
		{
			this.database = database;
		}

		protected override int Handle(GetRecordsCountRequest request)
		{
			return this.database.getRecordsCount(request.SensorName);
		}
	}

}