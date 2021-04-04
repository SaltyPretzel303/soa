using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServiceObserver.Configuration;

namespace CollectorService.Data
{

	public class DatedConfigRecord
	{
		public ServiceConfiguration configRecord;
		public DateTime backupDate;

		public DatedConfigRecord(ServiceConfiguration configRecord, DateTime backupDate)
		{
			this.configRecord = configRecord;
			this.backupDate = backupDate;
		}

	}

	public class ConfigBackupRecord
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string serviceId { get; set; }
		public List<DatedConfigRecord> oldConfigs { get; set; }

		public ConfigBackupRecord(string serviceId, List<DatedConfigRecord> oldConfigs)
		{
			this.serviceId = serviceId;
			this.oldConfigs = oldConfigs;
		}
	}



}