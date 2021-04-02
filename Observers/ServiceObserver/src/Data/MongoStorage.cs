using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using CollectorService.Data;
using CommunicationModel.RestModels;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceObserver.Configuration;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.Data
{
	public class MongoStorage : IDatabaseService
	{

		public MongoClient client { get; private set; }
		public IMongoDatabase database { get; private set; }

		public ServiceConfiguration config;

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

		public void BackupConfiguration(JObject oldJConfig)
		{
			if (!this.createConnection())
			{
				// if connection fails 
				return;
			}

			String serviceAddr = NetworkInterface
					.GetAllNetworkInterfaces()
					.Where(nic => nic.OperationalStatus == OperationalStatus.Up
								&& nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
					.Select(nic => nic.GetPhysicalAddress()
					.ToString())
					.FirstOrDefault();

			// at this point this.config still contains the old config. 
			IMongoCollection<ConfigBackupRecord> configCollection =
								 database.GetCollection<ConfigBackupRecord>(
									 			config.configBackupCollection);

			DatedConfigRecord newRecord = new DatedConfigRecord(oldJConfig, DateTime.Now);

			FilterDefinition<ConfigBackupRecord> filter = Builders<ConfigBackupRecord>
											.Filter.Eq(r => r.serviceId, serviceAddr);

			UpdateDefinition<ConfigBackupRecord> update = Builders<ConfigBackupRecord>
											.Update
											.Push<DatedConfigRecord>(r => r.oldConfigs,
																	newRecord);

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

			ServiceConfiguration config = ServiceConfiguration.Instance;

			IMongoCollection<ConfigBackupRecord> collection =
							database.GetCollection<ConfigBackupRecord>(config.configBackupCollection);
			FilterDefinition<ConfigBackupRecord> filter = Builders<ConfigBackupRecord>
											.Filter.Eq(r => r.serviceId, serviceAddr);

			ConfigBackupRecord match = collection.Find(filter).First();

			return match;
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