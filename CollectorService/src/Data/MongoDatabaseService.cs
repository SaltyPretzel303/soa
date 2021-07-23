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

		// this does not have to be async actually
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
					// either exceptions is gonna be thrown
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

		public async Task<bool> AddToSensor(
			string sensorName,
			SensorValues newValues)
		{
			if (!await createConnection())
			{
				return false;
			}

			string sensorsCollName = config.sensorsCollection;
			string valuesCollName = config.sensorValuesCollection;

			var sensorColl = database.GetCollection<SensorModel>(sensorsCollName);

			try
			{
				SensorModel parentSensor = null;

				var sensorCursor = await sensorColl.FindAsync<SensorModel>(
					(record) => record.sensorName == sensorName);

				await sensorCursor.MoveNextAsync();
				if (sensorCursor.Current.Count() > 0)
				{
					parentSensor = sensorCursor.Current.First();
				}
				else
				{
					sensorCursor.Dispose();
					parentSensor = new SensorModel(sensorName);
					await sensorColl.InsertOneAsync(parentSensor);
				}

				var valuesModel = getValuesModel(newValues, parentSensor.id);

				var valuesColl =
					database.GetCollection<SensorValuesModel>(valuesCollName);

				await valuesColl.InsertOneAsync(valuesModel);
			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return false;
			}

			// var sensorFilter = Builders<SensorModel>
			// 	.Filter
			// 	.Eq(s => s.sensorName, sensorName);

			// var update = Builders<SensorModel>
			// 	.Update
			// 	.Push<SensorValues>(s => s.records, newValues);

			// UpdateResult addResult = null;
			// try
			// {
			// 	addResult = await sensorColl.UpdateOneAsync(
			// 		sensorFilter,
			// 		update,
			// 		new UpdateOptions { IsUpsert = true });

			// }
			// catch (TimeoutException)
			// {
			// 	Console.WriteLine($"Failed to connect with the database on: "
			// 		+ $"{config.dbAddress} ... ");

			// 	return false;
			// }

			return true;
		}

		public async Task<bool> AddToSensor(
			String sensorName,
			List<SensorValues> newRecords)
		{
			if (!await createConnection())
			{
				return false;
			}

			foreach (var record in newRecords)
			{
				await this.AddToSensor(sensorName, record);
			}

			return true;
		}

		private SensorValuesModel getValuesModel(SensorValues values, ObjectId parentId)
		{
			var str_values = Newtonsoft.Json.JsonConvert.SerializeObject(values);
			var obj_values = (JObject.Parse(str_values)).ToObject<SensorValuesModel>();
			obj_values.sensorId = parentId;

			return obj_values;
		}

		public async Task<List<SensorModel>> GetAllValues()
		{
			if (!await createConnection())
			{
				return null;
			}

			string sensorCollName = config.sensorsCollection;
			var sensorsColl = database.GetCollection<SensorModel>(sensorCollName);

			try
			{
				var sensorCursor = await sensorsColl.FindAsync(_ => true);

				var valuesCollName = config.sensorValuesCollection;
				var valuesColl =
					database.GetCollection<SensorValuesModel>(valuesCollName);

				var sensors = new List<SensorModel>();

				while (await sensorCursor.MoveNextAsync())
				{
					foreach (var sensor in sensorCursor.Current)
					{
						var valuesCursor = await valuesColl.FindAsync(
							(value) => value.sensorId == sensor.id);

						while (await valuesCursor.MoveNextAsync())
						{
							sensor.values.AddRange(valuesCursor.Current.ToList());
						}

						sensors.Add(sensor);
					}
				}

				return sensors;

			}
			catch (TimeoutException)
			{
				Console.WriteLine($"Failed to connect with the database on: "
					+ $"{config.dbAddress} ... ");

				return null;
			}

		}

		public async Task<SensorModel> getRecordRange(string sensorName,
			long fromTimestamp,
			long toTimestamp)
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
						records = s.values
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
						records = s.values
							.AsQueryable()
							.Where(r => timestamps.Contains(r.timestamp))
					})
					.FirstAsync();

				if (dbResult != null)
				{
					return new SensorModel(
						dbResult.sensorName,
						dbResult.records.ToList());
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

		public async Task<bool> updateRecord(
			string sensorName,
			long timestamp,
			string field,
			string newValue)
		{

			if (!await createConnection())
			{
				return false;
			}

			var sensorColl = database.GetCollection<SensorModel>(config.sensorsCollection);

			try
			{

				var sensorCursor = await sensorColl.FindAsync(
					(sensor) => sensor.sensorName == sensorName);

				await sensorCursor.MoveNextAsync();
				if (sensorCursor.Current.Count() == 0)
				{
					return false;
				}

				var sensor = sensorCursor.Current.First();

				var valuesCollName = config.sensorValuesCollection;
				var valuesColl =
					database.GetCollection<SensorValuesModel>(valuesCollName);

				var sensorValues = await valuesColl.FindOneAndDeleteAsync(
					(value) =>
						value.sensorId == sensor.id
						&& value.timestamp == timestamp
				);

				sensorValues
					.GetType()
					.GetProperty(field)
					.SetValue(sensorValues, newValue);

				await valuesColl.InsertOneAsync(sensorValues);

				return true;
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

			string sensorCollName = config.sensorsCollection;
			var sensorColl = database.GetCollection<SensorModel>(sensorCollName);

			// var nameFilter = Builders<SensorModel>
			// 	.Filter
			// 	.Eq(rec => rec.sensorName, sensorName);

			// var timestampFilter = Builders<SensorValues>
			// 	.Filter
			// 	.Eq(rec => rec.timestamp, timestamp);

			// var update = Builders<SensorModel>
			// 	.Update
			// 	.PullFilter(rec => rec.records, timestampFilter);
			try
			{
				var sensorCursor = await sensorColl.FindAsync(
					(sensor) => sensor.sensorName == sensorName
				);
				await sensorCursor.MoveNextAsync();
				if (sensorCursor.Current.Count() == 0)
				{
					return false;
				}

				var sensor = sensorCursor.Current.First();

				var valuesCollName = config.sensorValuesCollection;
				var valuesColl =
					database.GetCollection<SensorValuesModel>(valuesCollName);

				var res = await valuesColl.DeleteOneAsync(
					(value) =>
						value.sensorId == sensor.id
						&& value.timestamp == timestamp);

				return (res.DeletedCount == 1);

				// UpdateResult result =
				// 	await collection.UpdateOneAsync(nameFilter, update);

				// return (result.ModifiedCount == 1);
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

		public async Task<long> getRecordsCount(string sensorName)
		{

			if (!await createConnection())
			{
				return -1;
			}

			string sensorCollName = config.sensorsCollection;
			var sensorColl = database.GetCollection<SensorModel>(sensorCollName);

			try
			{
				var sensorCursor = await sensorColl.FindAsync(
					(sensor) => sensor.sensorName == sensorName);

				await sensorCursor.MoveNextAsync();
				if (sensorCursor.Current.Count() == 0)
				{
					return 0;
				}

				var sensor = sensorCursor.Current.First();

				var valuesCollName = config.sensorValuesCollection;
				var valuesColl =
					database.GetCollection<SensorValuesModel>(valuesCollName);

				return await valuesColl.CountDocumentsAsync(
					(values) => values.sensorId == sensor.id);

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
					r => r.serviceId == config.serviceId,
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