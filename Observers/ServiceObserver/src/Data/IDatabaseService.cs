using CollectorService.Data;
using Newtonsoft.Json.Linq;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.Data
{
	public interface IDatabaseService
	{

		void BackupConfiguration(JObject rawConfig);

		void SaveUnstableRecord(UnstableRecord newRecord);

		ConfigBackupRecord getConfigs();

	}
}