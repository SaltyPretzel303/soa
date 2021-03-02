using Newtonsoft.Json.Linq;
using ServiceObserver.Report;

namespace ServiceObserver.Data
{
	public interface IDatabaseService
	{

		void BackupConfiguration(JObject rawConfig);

		void SaveEvent(ServiceEvent newEvent);

	}
}