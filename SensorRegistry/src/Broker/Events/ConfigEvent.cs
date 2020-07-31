using Newtonsoft.Json.Linq;
using SensorRegistry.Configuration;

namespace SensorRegistry.Broker.Events
{
	public interface IConfigEventHandler
	{
		void HandleNewConfig(JObject newConfig);
	}

	public class NewConfigurationHandler : IConfigEventHandler
	{
		public void HandleNewConfig(JObject newConfig)
		{
			ServiceConfiguration.Instance.UpdateConfig(newConfig);
		}
	}

}