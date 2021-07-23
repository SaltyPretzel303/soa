using System.Threading;
using System.Threading.Tasks;
using CollectorService.Configuration;
using CollectorService.Data;
using MediatR;
using Newtonsoft.Json.Linq;

namespace CollectorService.MediatrRequests
{

	public class ConfigChangeRequest : IRequest
	{
		public JObject NewConfig { get; private set; }

		public ConfigChangeRequest(JObject newConfig)
		{
			NewConfig = newConfig;
		}
	}

	public class ConfigChangeRequestHandler
		: IRequestHandler<ConfigChangeRequest, Unit>
	{
		private IDatabaseService database;

		public ConfigChangeRequestHandler(IDatabaseService database)
		{
			this.database = database;
		}

		// MediatR doesn't accept only Task as a return type
		public async Task<Unit> Handle(ConfigChangeRequest request,
			CancellationToken token)
		{
			await ServiceConfiguration.reload(
				request.NewConfig.ToObject<ServiceConfiguration>(),
				database);

			// 'void' value in MediatR 
			return Unit.Value;
		}
	}
}