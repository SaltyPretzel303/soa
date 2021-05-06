using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using ServiceObserver.Configuration;
using ServiceObserver.Data;

namespace ServiceObserver.MediatrRequests
{
	public class ConfigChangeRequest : IRequest
	{
		public JObject NewConfig { get; set; }

		public ConfigChangeRequest(JObject newConfig)
		{
			NewConfig = newConfig;
		}
	}

	public class ConfigChangeRequestHandler
		: IRequestHandler<ConfigChangeRequest, Unit>
	{

		private IDatabaseService database;

		public ConfigChangeRequestHandler(IDatabaseService db)
		{
			this.database = db;
		}

		public async Task<Unit> Handle(ConfigChangeRequest request,
			CancellationToken token)
		{
			await ServiceConfiguration.reload(
				request.NewConfig.ToObject<ServiceConfiguration>(),
				database);

			return Unit.Value;
		}
	}

}