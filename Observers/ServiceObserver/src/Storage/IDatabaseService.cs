using Newtonsoft.Json.Linq;
using ServiceObserver.Report;

namespace ServiceObserver.Storage
{
	public interface IDatabaseService
	{

		void BackupConfiguration(JObject rawConfig);

		void SaveEvent(ServiceEvent newEvent);

	}
}