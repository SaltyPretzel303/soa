using Newtonsoft.Json.Linq;
using ServiceObserver.Report;

namespace ServiceObserver.Storage
{
	public class MongoStorage : IDatabaseService
	{

		public void backupConfiguration(JObject rawConfig)
		{
			throw new System.NotImplementedException();
		}

		public void saveEvent(ServiceEvent newEvent)
		{
			throw new System.NotImplementedException();
		}
	}
}