using System.Linq;
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using MongoDB.Bson.IO;
using CollectorService.Configuration;
using System.Net.NetworkInformation;
using CommunicationModel;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;

namespace CollectorService.Data
{
	public class MongoDatabaseService : IDatabaseService
	{

		private MongoClient client;
		private IMongoDatabase database;

		private ConfigFields config;

		public MongoDatabaseService()
		{
			this.config = ServiceConfiguration.Instance;
		}

		// TODO this does not have to be async actually
		// connection is gonna be established once query is run 
		// new MongoClient and getDatabase are just ... offline I guess ...  
		private Task<bool> createConnection()
		{
			return Task.Run(() =>
				{
					var settings = new MongoClientSettings()
					{
						Server = new MongoServerAddress(config.dbAddress),
						ServerSelectionTimeout = new TimeSpan(0, 0, 10)
					};

					client = new MongoClient(settings);

					// not sure but his can't be null I think
					// either exceptions is gonna be throwh
					// or a valid object is gonna be created
					// same with .GetDatabase ...  
					if (client != null)
					{
						database = client.GetDatabase(config.dbName);

						if (database != null)
						{
							return true;
						}
					}

					Console.Write("Failed to establish connection with mongo ... ");
					return false;
				});
		}

		// interface implementation

		public async Task<bool> AddToSensor(string sensorName,
			SensorValues newValues)
		{
			if (!await createConnection())
			{
				return false;
			}

			string collectionName = config.sensorsCollection;

			var collection = database.GetCollection<SensorModel>(collectionName);

			var sensorFilter = Builders<SensorModel>
				.Filter
				.Eq(s => s.sensorName, sensorName);

			var update = Builders<SensorModel>
				.Update
				.Push<SensorValues>(s => s.records, newValues);

			UpdateResult addResult = null;
			try
			{
				addResult = await collection.UpdateOneAsync(
					sensorFilter,
					update,
					new UpdateOptions { IsUpsert = true });

			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}

			return (addResult.ModifiedCount > 0);
		}

		public async Task<bool> AddToSensor(String sensorName,
			List<SensorValues> newRecords)
		{
			if (!await createConnection())
			{
				return false;
			}

			string collectionName = config.sensorsCollection;

			var collection = database.GetCollection<SensorModel>(collectionName);

			var sensorFilter = Builders<SensorModel>
					.Filter
					.Eq(s => s.sensorName, sensorName);

			var update = Builders<SensorModel>
					.Update
					.PushEach<SensorValues>(s => s.records, newRecords);

			UpdateResult addResult = null;
			try
			{
				addResult = await collection.UpdateOneAsync(sensorFilter,
						update,
						new UpdateOptions { IsUpsert = true });
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}

			return (addResult.ModifiedCount > 0);
		}

		public async Task<List<SensorModel>> GetAllSamples()
		{
			if (!await createConnection())
			{
				return null;
			}

			string collectionName = config.sensorsCollection;
			var collection = database.GetCollection<SensorModel>(collectionName);

			IAsyncCursor<SensorModel> cursor = null;
			try
			{
				cursor = await collection.FindAsync(_ => true);
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return null;
			}

			return await cursor.ToListAsync();
		}

		public async Task<SensorModel> getRecordRange(string sensorName,
			long fromTimestamp,
			long toTimestamp = long.MaxValue)
		{

			if (!await createConnection())
			{
				return null;
			}

			string collectionName = config.sensorsCollection;
			var collection = database.GetCollection<SensorModel>(collectionName);

			try
			{
				var dbResult = await collection.Aggregate()
					.Match(s => s.sensorName == sensorName)
					.Project(s => new
					{
						sensorName = s.sensorName,
						records = s.records
							.Where(r =>
								r.timestamp >= fromTimestamp
								&& r.timestamp <= toTimestamp)
					})
					.FirstAsync();

				if (dbResult != null)
				{
					return new SensorModel(dbResult.sensorName,
						dbResult.records.ToList()
					);
				}
				else
				{
					return null;
				}
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return null;
			}

		}

		public async Task<SensorModel> getRecordsList(string sensorName,
			List<long> timestamps)
		{
			if (!await createConnection())
			{
				return null;
			}

			var collection = database.GetCollection<SensorModel>(config.sensorsCollection);

			try
			{
				var dbResult = await collection
					.Aggregate()
					.Match(s => s.sensorName == sensorName)
					.Project(s => new
					{
						sensorName = s.sensorName,
						records = s.records
							.AsQueryable()
							.Where(r => timestamps.Contains(r.timestamp))
					})
					.FirstAsync();

				if (dbResult != null)
				{
					return new SensorModel(
						dbResult.sensorName,
						dbResult.records.ToList()
					);
				}
				else
				{
					return null;
				}
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return null;
			}
		}

		public async Task<bool> updateRecord(string sensorName,
			long timestamp,
			string field,
			string newValue)
		{

			if (!await createConnection())
			{
				return false;
			}

			var collection = database.GetCollection<SensorModel>(config.sensorsCollection);

			try
			{
				var findResult = await collection
					.Aggregate()
					.Match(s => s.sensorName == sensorName)
					.Project(s => new
					{
						record = s.records
							.Where(r => r.timestamp == timestamp)
							.First()
					})
					.FirstAsync();

				if (findResult == null)
				{
					// TODO log some message ... 
					return false;
				}

				SensorValues record = findResult.record;

				// I could also try to serialize obj to json/bson 
				// then change field's value (accessed by string) 
				// then deserialize it back to obj 
				record.GetType().GetProperty(field).SetValue(record, newValue);

				var nameFilter = Builders<SensorModel>
					.Filter
					.Where(s =>
						s.sensorName == sensorName
						&& s.records.Any(r => r.timestamp == timestamp));

				var update = Builders<SensorModel>
					.Update
					.Set(s => s.records[-1], record);
				// -1 means index of previously (from the last query) matched element

				var result = await collection.UpdateOneAsync(nameFilter, update);

				return (result.ModifiedCount == 1);
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}
		}

		public async Task<bool> deleteRecord(string sensorName, long timestamp)
		{

			if (!await createConnection())
			{
				return false;
			}

			string collectionName = config.sensorsCollection;
			var collection = database.GetCollection<SensorModel>(collectionName);

			var nameFilter = Builders<SensorModel>
				.Filter
				.Eq(rec => rec.sensorName, sensorName);

			var timestampFilter = Builders<SensorValues>
				.Filter
				.Eq(rec => rec.timestamp, timestamp);

			var update = Builders<SensorModel>
				.Update
				.PullFilter(rec => rec.records, timestampFilter);
			try
			{
				UpdateResult result =
					await collection.UpdateOneAsync(nameFilter, update);

				return (result.ModifiedCount == 1);
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}
		}

		public async Task<bool> deleteSensorData(string sensorName)
		{
			if (!await createConnection())
			{
				return false;
			}

			string collectionName = config.sensorsCollection;
			var collection = database.GetCollection<SensorModel>(collectionName);

			var nameFilter = Builders<SensorModel>
					.Filter
					.Eq(rec => rec.sensorName, sensorName);

			try
			{

				DeleteResult result = await collection.DeleteOneAsync(nameFilter);

				return (result.DeletedCount == 1);
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}
		}

		public async Task<int> getRecordsCount(string sensorName)
		{

			if (!await createConnection())
			{
				return -1;
			}

			string collectionName = config.sensorsCollection;
			var collection = database.GetCollection<SensorModel>(collectionName);

			try
			{
				var dbResult = await collection
					.Find(s => s.sensorName == sensorName)
					.Project(s => new
					{
						count = s.records.Count
					})
					.FirstAsync();

				if (dbResult != null)
				{
					return dbResult.count;
				}
				else
				{
					// somthing went wrong
					// don't know when is dbResult gonna be null ... 
					return -1;
				}
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return -1;
			}
			catch (InvalidOperationException)
			{
				// if there are no records for the given sensorName
				// this exception will be thrown 

				return 0;
			}

		}

		public async Task backupConfiguration(ServiceConfiguration oldConfig)
		{

			if (!await createConnection())
			{
				return;
			}

			// TODO maybe this id should be read from config ... 
			string serviceId = NetworkInterface
				.GetAllNetworkInterfaces()
				.Where(nic =>
					nic.OperationalStatus == OperationalStatus.Up
					&& nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
				.Select(nic => nic.GetPhysicalAddress().ToString())
				.FirstOrDefault();

			ConfigFields config = ServiceConfiguration.Instance;

			string collectionName = config.configurationBackupCollection;
			var collection = database
				.GetCollection<ConfigBackupRecord>(collectionName);

			var newRecord = new DatedConfigRecord(oldConfig, DateTime.Now);

			var update = Builders<ConfigBackupRecord>
				.Update
				.Push(r => r.oldConfigs, newRecord);

			try
			{
				collection.UpdateOne(
					r => r.serviceId == serviceId,
					update,
					new UpdateOptions { IsUpsert = true });

				return;
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return;
			}

		}

	}
}