using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using CollectorService.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceObserver.Configuration;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.Data
{
	public class MongoStorage : IDatabaseService
	{

		private MongoClient client;
		private IMongoDatabase database;

		private ConfigFields config;

		public MongoStorage()
		{
			this.config = ServiceConfiguration.Instance;
		}

		private bool createConnection()
		{
			this.client = new MongoClient(config.dbAddress);

			if (this.client != null)
			{

				this.database = this.client.GetDatabase(config.dbName);

				if (this.database != null)
				{
					return true;
				}

			}

			Console.Write("Failed to establish connection with mongo ... ");
			return false;
		}

		public void BackupConfiguration(ServiceConfiguration oldConfig)
		{
			if (!this.createConnection())
			{
				// if connection fails 
				return;
			}

			String serviceId = NetworkInterface
					.GetAllNetworkInterfaces()
					.Where(nic => nic.OperationalStatus == OperationalStatus.Up
								&& nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
					.Select(nic => nic.GetPhysicalAddress()
					.ToString())
					.FirstOrDefault();

			// at this point this.config still contains the old config. 
			string collectionName = config.configBackupCollection;
			IMongoCollection<ConfigBackupRecord> configCollection =
				database.GetCollection<ConfigBackupRecord>(collectionName);

			DatedConfigRecord newRecord = new DatedConfigRecord(oldConfig, DateTime.Now);

			var filter = Builders<ConfigBackupRecord>
				.Filter
				.Eq(r => r.serviceId, serviceId);

			var update = Builders<ConfigBackupRecord>
				.Update
				.Push<DatedConfigRecord>(r => r.oldConfigs, newRecord);

			configCollection.UpdateOne(filter, update,
									new UpdateOptions { IsUpsert = true });
			// upsert -> create if doesn't exists

		}

		public void SaveUnstableRecord(UnstableRuleRecord newRecord)
		{
			if (!this.createConnection())
			{
				// failed to establish connection 
				return;
			}

			UnstableServiceDbRecord dbRecord = new UnstableServiceDbRecord(
															newRecord.serviceId,
															newRecord.downCount,
															newRecord.downEvents,
															newRecord.time);

			IMongoCollection<UnstableServiceDbRecord> dbCollection =
						database.GetCollection<UnstableServiceDbRecord>(
												config.unstableRecordCollection);

			dbCollection.InsertOne(dbRecord);
		}

		public ConfigBackupRecord GetConfigs()
		{
			if (!this.createConnection())
			{
				// if connection fails 
				return null;
			}

			String serviceAddr = NetworkInterface.
					GetAllNetworkInterfaces()
					.Where(nic => nic.OperationalStatus == OperationalStatus.Up
								&& nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
					.Select(nic => nic.GetPhysicalAddress()
					.ToString())
					.FirstOrDefault();


			IMongoCollection<ConfigBackupRecord> collection =
				database.GetCollection<ConfigBackupRecord>(config.configBackupCollection);
			var filter = Builders<ConfigBackupRecord>
				.Filter
				.Eq(r => r.serviceId, serviceAddr);

			ConfigBackupRecord dbResult = collection
											.Find(filter)
											.First();

			return dbResult;
		}

		public List<UnstableServiceDbRecord> GetAllUnstableRecords()
		{
			if (!this.createConnection())
			{
				return null;
			}

			IMongoCollection<UnstableServiceDbRecord> dbCollection =
						database.GetCollection<UnstableServiceDbRecord>(
												config.unstableRecordCollection);

			return dbCollection.Find<UnstableServiceDbRecord>(new BsonDocument())
							.ToList<UnstableServiceDbRecord>();
		}

		public List<UnstableServiceDbRecord> GetUnstableRecordsForService(string serviceId)
		{

			if (!this.createConnection())
			{
				return null;
			}

			IMongoCollection<UnstableServiceDbRecord> dbCollection =
						database.GetCollection<UnstableServiceDbRecord>(
									config.unstableRecordCollection);

			FilterDefinition<UnstableServiceDbRecord> filter =
								Builders<UnstableServiceDbRecord>
								.Filter.Eq(r => r.serviceId, serviceId);

			List<UnstableServiceDbRecord> result = dbCollection
										.Find<UnstableServiceDbRecord>(filter)
										.ToList<UnstableServiceDbRecord>();


			return result;
		}

		public UnstableServiceDbRecord GetLatestRecord()
		{
			if (!this.createConnection())
			{
				return null;
			}

			IMongoCollection<UnstableServiceDbRecord> dbCollection =
						database.GetCollection<UnstableServiceDbRecord>(
									config.unstableRecordCollection);

			BsonDocument allFilter = new BsonDocument();

			UnstableServiceDbRecord dbRecord = dbCollection
							.Find<UnstableServiceDbRecord>(allFilter)
							.SortByDescending(r => r.recordedTime)
							.First<UnstableServiceDbRecord>();

			return dbRecord;
		}
	}
}