using System.Collections.Generic;
using CollectorService.Data;
using CommunicationModel.RestModels;
using Newtonsoft.Json.Linq;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.Data
{
	public interface IDatabaseService
	{

		void BackupConfiguration(JObject rawConfig);

		void SaveUnstableRecord(UnstableRuleRecord newRecord);

		ConfigBackupRecord GetConfigs();

		List<UnstableServiceDbRecord> GetAllUnstableRecords();

		List<UnstableServiceDbRecord> GetUnstableRecordsForService(string serviceId);

		UnstableServiceDbRecord GetLatestRecord();

	}
}