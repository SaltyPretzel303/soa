using System.Diagnostics;
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SensorService.Configuration;
using System.Collections.Generic;

namespace CollectorService.Data
{
	public class DatabaseService : IDatabaseService
	{

		private ServiceConfiguration configuration;

		public MongoClient client { get; private set; }
		public IMongoDatabase database { get; private set; }
		public IMongoCollection<BsonDocument> collection { get; private set; }

		// constructors

		public DatabaseService()
		{

			this.configuration = ServiceConfiguration.read();

			this.client = new MongoClient(this.configuration.dbAddress);
			this.database = this.client.GetDatabase(configuration.dbName);
			this.collection = this.database.GetCollection<BsonDocument>(configuration.collectionName);

			this.getAllRecords();

		}

		public DatabaseService(MongoClient client, IMongoDatabase database)
		{

			this.configuration = ServiceConfiguration.read();

			this.client = client;
			this.database = database;

		}

		// methods

		public void pushToUser(String user_name, JArray rows)
		{

			IMongoCollection<BsonDocument> collection = this.database.GetCollection<BsonDocument>(configuration.collectionName);

			collection.UpdateOne("{\"user_name\":\"" + user_name + "\"}",
								"{$push: { \"" + this.configuration.dbUserArray + "\": { $each : " + rows.ToString() + " } }}",
								 new UpdateOptions { IsUpsert = true });
			// upsert true -> create document if doesn't exists

		}

		// returns all data for all users
		public string getAllRecords()
		{

			string cache = this.collection.FindSync(_ => true).ToList().ToString();
			Console.WriteLine("CACHE: " + cache);

			return null;
			// return cache;
		}

		// returns all recors for specified user (index)
		public String getDatFromUser(string user_name)
		{

			return this.collection.Find("{\"user_name\":\"" + user_name + "\"}").ToString();

		}

		// returns records from every user on [index] position
		public String getRecordsAt(int index)
		{

			return "";
		}

		public String getRecordFromUser(string user_name, int record_index)
		{

			return "";
		}

	}
}