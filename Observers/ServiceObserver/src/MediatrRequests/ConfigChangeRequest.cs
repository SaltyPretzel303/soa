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

	public class ConfigChangeRequestHandler : RequestHandler<ConfigChangeRequest>
	{

		private IDatabaseService db;

		public ConfigChangeRequestHandler(IDatabaseService db)
		{
			this.db = db;
		}

		protected override void Handle(ConfigChangeRequest request)
		{
			ServiceConfiguration.reload(request.NewConfig, db);
		}
	}

}