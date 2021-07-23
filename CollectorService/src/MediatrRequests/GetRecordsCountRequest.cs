using System.Threading;
using System.Threading.Tasks;
using CollectorService.Data;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class GetRecordsCountRequest : IRequest<long>
	{
		public string SensorName { get; private set; }

		public GetRecordsCountRequest(string sensorName)
		{
			SensorName = sensorName;
		}
	}

	public class GetRecordsCountRequestHandler
		: IRequestHandler<GetRecordsCountRequest, long>
	{

		private IDatabaseService database;

		public GetRecordsCountRequestHandler(IDatabaseService database)
		{
			this.database = database;
		}

		public async Task<long> Handle(GetRecordsCountRequest request,
			CancellationToken cancellationToken)
		{
			return await database.getRecordsCount(request.SensorName);
		}

	}

}