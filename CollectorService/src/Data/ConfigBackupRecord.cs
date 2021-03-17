using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data
{
	public class ConfigBackupRecord
	{
		public string serviceId { get; set; }

		public List<DatedConfigRecord> oldConfigs { get; set; }

		public ConfigBackupRecord(string serviceId, List<DatedConfigRecord> oldConfigs)
		{
			this.serviceId = serviceId;
			this.oldConfigs = oldConfigs;
		}
	}

	public class DatedConfigRecord
	{
		public JObject configRecord;
		public DateTime backupDate;

		public DatedConfigRecord(JObject configRecord, DateTime backupDate)
		{
			this.configRecord = configRecord;
			this.backupDate = backupDate;
		}
	}

}