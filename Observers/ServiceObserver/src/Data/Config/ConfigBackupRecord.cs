using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CollectorService.Data
{
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