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

namespace CollectorService.Data
{
	public class MongoDatabaseService : IDatabaseService
	{

		public MongoClient client { get; private set; }
		public IMongoDatabase database { get; private set; }

		public MongoDatabaseService()
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

		// interface implementation

		public void addToSensor(string sensorName, SensorValues newValues)
		{
			if (!this.createConnection())
			{
				return;
			}

			string collectionName = ServiceConfiguration.Instance.sensorsCollection;

			IMongoCollection<SensorModel> collection =
				this.database.GetCollection<SensorModel>(collectionName);

			var sensorFilter = Builders<SensorModel>
				.Filter
				.Eq(s => s.sensorName, sensorName);

			var update = Builders<SensorModel>
				.Update
				.Push<SensorValues>(s => s.records, newValues);

			collection.UpdateOne(
				sensorFilter,
				update,
				new UpdateOptions { IsUpsert = true });
		}

		public void addToSensor(String sensorName, List<SensorValues> newRecords)
		{

			if (!this.createConnection())
			{
				return;
			}

			string collectionName = ServiceConfiguration.Instance.sensorsCollection;

			IMongoCollection<SensorModel> collection =
					this.database.GetCollection<SensorModel>(collectionName);

			var sensorFilter = Builders<SensorModel>
					.Filter
					.Eq(s => s.sensorName, sensorName);

			var update = Builders<SensorModel>
					.Update
					.PushEach<SensorValues>(s => s.records, newRecords);

			collection.UpdateOne(sensorFilter,
					update,
					new UpdateOptions { IsUpsert = true });
		}

		public List<SensorModel> getAllSamples()
		{

			if (!this.createConnection())
			{
				return null;
			}

			string collectionName = ServiceConfiguration.Instance.sensorsCollection;
			IMongoCollection<SensorModel> collection =
					this.database.GetCollection<SensorModel>(collectionName);

			List<SensorModel> dbResult = collection
					.FindSync(_ => true)
					.ToList();

			return dbResult;
		}

		public SensorModel getRecordRange(string sensorName,
					 long fromTimestamp,
					 long toTimestamp = long.MaxValue)
		{

			if (!this.createConnection())
			{
				return null;
			}

			string collection = ServiceConfiguration.Instance.sensorsCollection;
			IMongoCollection<SensorModel> sensorsCollection =
					this.database.GetCollection<SensorModel>(collection);

			SensorModel dbResult = sensorsCollection
				.AsQueryable<SensorModel>()
				.Where(s => s.sensorName == sensorName)
				.Select(r => new
				{
					sensorName = r.sensorName,
					records = r.records.Where(sv =>
										sv.timestamp >= fromTimestamp
										&& sv.timestamp <= toTimestamp)
				})
				.AsEnumerable()
				.Select(r => new SensorModel
				{
					sensorName = r.sensorName,
					records = r.records.ToList()
				})
				.First();

			return dbResult;
		}

		public SensorModel getRecordsList(string sensorName, List<long> timestamps)
		{
			if (!this.createConnection())
			{
				return null;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<SensorModel> collection =
				this.database.GetCollection<SensorModel>(config.sensorsCollection);

			SensorModel dbResult = collection
				.AsQueryable<SensorModel>()
				.Where(rec => rec.sensorName == sensorName)
				.Select(rec => new
				{
					sensorName = rec.sensorName,
					records = rec
							.records
							.Where(r => timestamps.Contains(r.timestamp))
				})
				.AsEnumerable()
				.Select(model => new SensorModel
				{
					sensorName = model.sensorName,
					records = model.records.ToList()
				})
				.First();

			return dbResult;
		}

		public bool updateRecord(string sensorName,
			long timestamp,
			string field,
			string newValue)
		{

			if (!this.createConnection())
			{
				return false;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<SensorModel> collection =
					this.database.GetCollection<SensorModel>(config.sensorsCollection);

			SensorValues record = collection
				.AsQueryable<SensorModel>()
				.Where(s => s.sensorName == sensorName)
				.Select(s => s.records.Where(r => r.timestamp == timestamp).First())
				.First();

			// I could also try to serialize obj to json/bson 
			// then change field's value (it is accessed by string) 
			// then deserialize it back to obj 
			record.GetType().GetProperty(field).SetValue(record, newValue);

			var nameFilter = Builders<SensorModel>
				.Filter
				.Where(s => s.sensorName == sensorName &&
					s.records.Any(r => r.timestamp == timestamp));

			var update = Builders<SensorModel>
				.Update
				.Set(s => s.records[-1], record);
			// -1 means index of previously (from the last query) matched element

			var result = collection.UpdateOne(nameFilter, update);

			return (result.MatchedCount == 1);

		}

		public bool deleteRecord(string sensorName, long timestamp)
		{

			if (!this.createConnection())
			{
				return false;
			}

			string collectionName = ServiceConfiguration.Instance.sensorsCollection;
			IMongoCollection<SensorModel> collection =
					this.database.GetCollection<SensorModel>(collectionName);

			var nameFilter = Builders<SensorModel>
				.Filter
				.Eq(rec => rec.sensorName, sensorName);

			var timestampFilter = Builders<SensorValues>
				.Filter
				.Eq(rec => rec.timestamp, timestamp);

			var update = Builders<SensorModel>
				.Update
				.PullFilter(rec => rec.records, timestampFilter);

			UpdateResult result = collection.UpdateOne(nameFilter, update);

			return (result.MatchedCount == 1);
		}

		public bool deleteSensorData(string sensorName)
		{

			if (!this.createConnection())
			{
				return false;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<SensorModel> collection =
					this.database.GetCollection<SensorModel>(config.sensorsCollection);

			var nameFilter = Builders<SensorModel>
					.Filter
					.Eq(rec => rec.sensorName, sensorName);

			DeleteResult result = collection.DeleteOne(nameFilter);

			return (result.DeletedCount == 1);
		}

		public void backupConfiguration(JObject oldJConfig)
		{

			if (!this.createConnection())
			{
				return;
			}

			string serviceAddr = NetworkInterface.
								GetAllNetworkInterfaces().
								Where(nic => nic.OperationalStatus == OperationalStatus.Up
											&& nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).
								Select(nic => nic.GetPhysicalAddress().ToString()).
								FirstOrDefault();

			ServiceConfiguration oldOConfig = ServiceConfiguration.Instance;

			IMongoCollection<BsonDocument> configCollection =
								database.GetCollection<BsonDocument>(oldOConfig.configurationBackupCollection);

			oldJConfig[oldOConfig.configBackupDateField] = DateTime.Now.ToString();

			string matchQuery = String.Format(@"{{service_name: '{0}'}}", serviceAddr);
			string updateQuery = String.Format(@"{{$push: {{{0}: {1}}}}}", "old_configs", oldJConfig.ToString());

			try
			{

				configCollection.UpdateOne(matchQuery, updateQuery, new UpdateOptions { IsUpsert = true });
				// upsert true -> create document if doesn't exists
				Console.WriteLine("Configuration backup done ... ");

			}
			catch (FormatException e)
			{
				Console.WriteLine(e.StackTrace);
			}

		}

		public int getRecordsCount(string sensorName)
		{

			if (!this.createConnection())
			{
				return 0;
			}

			Console.WriteLine($"Gonna try to get last index for {sensorName}");

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<SensorModel> sensorsCollection = this.database.GetCollection<SensorModel>(config.sensorsCollection);


			FilterDefinition<SensorModel> nameFilter = Builders<SensorModel>
					.Filter.Eq(rec => rec.sensorName, sensorName);

			ProjectionDefinition<SensorModel, int> countQuery = Builders<SensorModel>
					.Projection.Expression(rec => rec.records.Count);

			int count = 0;
			try
			{

				count = sensorsCollection
						.Find<SensorModel>(nameFilter)
						.Project<int>(countQuery)
						.Single<int>();
			}
			catch (InvalidOperationException)
			{

				// if there is no records for given sensorName
				// this exception will happen 

				count = 0;
			}
			Console.WriteLine($"LastReadIndex: {count}");

			return count;

		}

	}
}