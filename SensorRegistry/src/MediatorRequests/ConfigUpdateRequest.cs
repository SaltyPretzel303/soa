using MediatR;
using Newtonsoft.Json.Linq;
using SensorRegistry.Configuration;

namespace SensorRegistry.MediatorRequests
{
	public class ConfigUpdateRequest : IRequest
	{
		public JObject NewConfig { get; set; }

		public ConfigUpdateRequest(JObject newConfig)
		{
			NewConfig = newConfig;
		}
	}

	public class ConfigUpdateRequestHandler
		: RequestHandler<ConfigUpdateRequest>
	{
		protected override void Handle(ConfigUpdateRequest request)
		{
			ServiceConfiguration.Instance.UpdateConfig(request.NewConfig);
		}
	}

}