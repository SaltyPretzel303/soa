using Newtonsoft.Json.Linq;
using ServiceObserver.Report;

namespace ServiceObserver.Storage
{
	public interface IDatabaseService
	{

		void backupConfiguration(JObject rawConfig);

		void saveEvent(ServiceEvent newEvent);

	}
}