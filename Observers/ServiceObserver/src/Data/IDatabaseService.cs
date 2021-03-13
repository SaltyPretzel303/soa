using Newtonsoft.Json.Linq;

namespace ServiceObserver.Data
{
	public interface IDatabaseService
	{

		void BackupConfiguration(JObject rawConfig);

	}
}