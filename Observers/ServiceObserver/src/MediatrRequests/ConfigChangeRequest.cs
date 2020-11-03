using MediatR;
using Newtonsoft.Json.Linq;
using ServiceObserver.Configuration;

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

		protected override void Handle(ConfigChangeRequest request)
		{
			ServiceConfiguration.reload(request.NewConfig);
		}
	}

}