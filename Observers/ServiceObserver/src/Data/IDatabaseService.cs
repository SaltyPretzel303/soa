using System.Collections.Generic;
using CollectorService.Data;
using CommunicationModel.RestModels;
using Newtonsoft.Json.Linq;
using ServiceObserver.Configuration;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.Data
{
	public interface IDatabaseService
	{
		void SaveUnstableRecord(UnstableRuleRecord newRecord);

		List<UnstableServiceDbRecord> GetAllUnstableRecords();

		List<UnstableServiceDbRecord> GetUnstableRecordsForService(string serviceId);

		UnstableServiceDbRecord GetLatestRecord();

		void BackupConfiguration(ServiceConfiguration rawConfig);

		ConfigBackupRecord GetConfigs();

	}
}