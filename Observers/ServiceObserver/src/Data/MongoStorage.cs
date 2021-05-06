using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
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
			client = new MongoClient(config.dbAddress);

			if (client != null)
			{
				database = this.client.GetDatabase(config.dbName);

				if (database != null)
				{
					return true;
				}
			}

			return false;
		}

		public async Task<bool> BackupConfiguration(ServiceConfiguration oldConfig)
		{
			if (!createConnection())
			{
				return false;
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
			var collection = database
				.GetCollection<ConfigBackupRecord>(collectionName);

			var newRecord = new DatedConfigRecord(oldConfig, DateTime.Now);

			var update = Builders<ConfigBackupRecord>
				.Update
				.Push<DatedConfigRecord>(r => r.oldConfigs, newRecord);

			try
			{
				var updateResult = await collection.UpdateOneAsync(
					c => c.serviceId == serviceId,
					update,
					new UpdateOptions { IsUpsert = true });

				return (updateResult.ModifiedCount > 0);
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}

		}

		public async Task<ConfigBackupRecord> GetConfigs()
		{
			if (!createConnection())
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

			var collection = database
				.GetCollection<ConfigBackupRecord>(config.configBackupCollection);

			IAsyncCursor<ConfigBackupRecord> dbCursor = await collection
				.FindAsync(r => r.serviceId == serviceAddr);

			// await dbCursor.AnyAsync<ConfigBackupRecord>()
			// cursor.FirstAsync() called after prev. line will throw
			// invalid operation exception: Cannot access a disposed object

			if (dbCursor != null)
			{
				try
				{
					var record = await dbCursor.FirstAsync<ConfigBackupRecord>();

					return record;
				}
				catch (InvalidOperationException)
				{
					// most likely sequence contains no elements

					return new ConfigBackupRecord(serviceAddr,
						new List<DatedConfigRecord>());
				}
				catch (TimeoutException)
				{
					Console.WriteLine($"Failed to connect with the database on: "
						+ $"{config.dbAddress} ... ");

					return null;
				}
			}

			return null;
		}


		public async Task<bool> SaveUnstableRecord(UnstableRuleRecord newRecord)
		{
			if (!createConnection())
			{
				// failed to establish connection 
				return false;
			}

			var dbRecord = new UnstableServiceDbRecord(
				newRecord.serviceId,
				newRecord.downCount,
				newRecord.downEvents,
				newRecord.time);

			var collection = database.GetCollection<UnstableServiceDbRecord>(
				config.unstableRecordCollection);

			try
			{
				await collection.InsertOneAsync(dbRecord);

				return true;
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}
		}

		public async Task<List<UnstableServiceDbRecord>> GetAllUnstableRecords()
		{
			if (!createConnection())
			{
				return null;
			}

			var collection = database.GetCollection<UnstableServiceDbRecord>(
					config.unstableRecordCollection);

			try
			{
				IAsyncCursor<UnstableServiceDbRecord> dbCursor = await collection
					.FindAsync(_ => true);

				return await dbCursor.ToListAsync<UnstableServiceDbRecord>();
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return null;
			}
		}

		public async Task<List<UnstableServiceDbRecord>> GetUnstableRecordsForService(string serviceId)
		{
			if (!createConnection())
			{
				return null;
			}

			var collection = database.GetCollection<UnstableServiceDbRecord>(
				config.unstableRecordCollection);

			try
			{
				IAsyncCursor<UnstableServiceDbRecord> dbCursor = await collection
					.FindAsync<UnstableServiceDbRecord>(r => r.serviceId == serviceId);

				return await dbCursor.ToListAsync<UnstableServiceDbRecord>();
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return null;
			}
		}

		public async Task<UnstableServiceDbRecord> GetLatestRecord()
		{
			if (!createConnection())
			{
				return null;
			}

			string collectionName = config.unstableRecordCollection;
			var collection = database
				.GetCollection<UnstableServiceDbRecord>(collectionName);

			try
			{
				var dbRecord = await collection
					.Find<UnstableServiceDbRecord>(_ => true)
					.SortByDescending(r => r.recordedTime)
					.FirstAsync<UnstableServiceDbRecord>();

				return dbRecord;
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return null;
			}

		}
	}
}