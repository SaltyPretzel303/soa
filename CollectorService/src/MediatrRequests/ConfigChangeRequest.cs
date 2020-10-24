using System;
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

	public class ConfigChangeRequestHandler : RequestHandler<ConfigChangeRequest>
	{

		private IDatabaseService database;

		public ConfigChangeRequestHandler(IDatabaseService database)
		{
			this.database = database;
		}

		protected override void Handle(ConfigChangeRequest request)
		{
			ServiceConfiguration.reload(request.NewConfig, this.database);
		}

	}
}