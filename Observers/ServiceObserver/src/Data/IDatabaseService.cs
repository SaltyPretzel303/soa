using System.Collections.Generic;
using System.Threading.Tasks;
using CollectorService.Data;
using ServiceObserver.Configuration;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.Data
{
	public interface IDatabaseService
	{
		Task<bool> SaveUnstableRecord(UnstableRuleRecord newRecord);

		Task<List<UnstableServiceDbRecord>> GetAllUnstableRecords();

		Task<List<UnstableServiceDbRecord>> GetUnstableRecordsForService(string serviceId);

		Task<UnstableServiceDbRecord> GetLatestRecord();

		Task<bool> BackupConfiguration(ServiceConfiguration rawConfig);

		Task<ConfigBackupRecord> GetConfigs();
	}
}