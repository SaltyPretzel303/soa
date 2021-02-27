using System.Collections.Generic;
using CommunicationModel;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class GetAllSensorsRequest : IRequest<List<SensorRegistryRecord>>
	{

	}

	public class GetAllSensorsRequestHandler : RequestHandler<GetAllSensorsRequest, List<SensorRegistryRecord>>
	{

		private IRegistryCache registryCache;

		public GetAllSensorsRequestHandler(IRegistryCache registryCache)
		{
			this.registryCache = registryCache;
		}

		protected override List<SensorRegistryRecord> Handle(GetAllSensorsRequest request)
		{
			return this.registryCache.GetAllSensors();
		}
	}

}