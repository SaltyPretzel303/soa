using CollectorService.Configuration;
using CollectorService.Data;
using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Events
{
	public interface IConfigChange
	{
		void UpdateConfiguration(JObject newConfig);
	}

	public class ConfigChangeHandler : IConfigChange
	{

		private IDatabaseService database;

		public ConfigChangeHandler(IDatabaseService database)
		{
			this.database = database;
		}

		public void UpdateConfiguration(JObject newConfig)
		{
			ServiceConfiguration.reload(newConfig, this.database);
		}
	}
}