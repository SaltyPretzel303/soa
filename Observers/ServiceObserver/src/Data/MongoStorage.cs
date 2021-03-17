using System;
using System.Linq;
using System.Net.NetworkInformation;
using CollectorService.Data;
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

		public MongoStorage()
		{
		}

		private bool createConnection()
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

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

			String serviceAddr = NetworkInterface.
								GetAllNetworkInterfaces()
								.Where(nic => nic.OperationalStatus == OperationalStatus.Up
											&& nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
								.Select(nic => nic.GetPhysicalAddress()
								.ToString())
								.FirstOrDefault();

			ServiceConfiguration oldObjConfig = ServiceConfiguration.Instance;

			// IMongoCollection<BsonDocument> configCollection = database.GetCollection<BsonDocument>(oldObjConfig.configurationBackupCollection);
			IMongoCollection<ConfigBackupRecord> configCollection =
								 database.GetCollection<ConfigBackupRecord>(oldObjConfig.configBackupCollection);

			// string oldTxtConfig = oldJConfig.ToString();

			DatedConfigRecord newRecord = new DatedConfigRecord(oldJConfig, DateTime.Now);

			FilterDefinition<ConfigBackupRecord> filter = Builders<ConfigBackupRecord>
											.Filter.Eq(r => r.serviceId, serviceAddr);

			UpdateDefinition<ConfigBackupRecord> update = Builders<ConfigBackupRecord>
											.Update
											.Push<DatedConfigRecord>(r => r.oldConfigs,
																	newRecord);

			configCollection.UpdateOne(filter,
									update,
									new UpdateOptions { IsUpsert = true });
			// upsert -> create if doesn't exists


			// oldJConfig[oldObjConfig.configBackupDateField] = DateTime.Now.ToString();

			// string matchQuery = String.Format(@"{{service_name: '{0}'}}", serviceAddr);
			// string updateQuery = String.Format(@"{{$push: {{{0}: {1}}}}}",
			// 							"old_configs",
			// 							oldJConfig.ToString());

			// try
			// {

			// 	configCollection.UpdateOne(matchQuery, updateQuery, new UpdateOptions { IsUpsert = true });
			// 	// upsert true -> create document if doesn't exists
			// 	Console.WriteLine("Configuration backup done ... ");

			// }
			// catch (FormatException e)
			// {
			// 	Console.WriteLine(e.StackTrace);
			// }

		}

		public void SaveUnstableRecord(UnstableRecord newRecord)
		{
			if (!this.createConnection())
			{
				// failed to establish connection 
				return;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;

			IMongoCollection<UnstableRecord> dbCollection =
						database.GetCollection<UnstableRecord>(config.unstableRecordCollection);

			dbCollection.InsertOne(newRecord);

		}

		public ConfigBackupRecord getConfigs()
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
	}
}