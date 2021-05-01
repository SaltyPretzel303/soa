using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunicationModel;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class GetAllSensorsRequest : IRequest<List<SensorRegistryRecord>>
	{

	}

	public class GetAllSensorsRequestHandler
		: IRequestHandler<GetAllSensorsRequest, List<SensorRegistryRecord>>
	{

		private IRegistryCache registryCache;

		public GetAllSensorsRequestHandler(IRegistryCache registryCache)
		{
			this.registryCache = registryCache;
		}

		public async Task<List<SensorRegistryRecord>> Handle(
			GetAllSensorsRequest request,
			CancellationToken token)
		{
			return await registryCache.GetAllRecords();
		}
	}

}