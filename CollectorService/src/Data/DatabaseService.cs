using System.Linq;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SensorService.src;

namespace CRUDService.Data
{
	public class DatabaseService : IDatabaseService
	{

		static private String DATABASE_NAME = "soa";
		static private String COLLECTION_NAME = "Users";
		static private String USER_ARRAY = "values";

		public DatabaseService(MongoClient client, IMongoDatabase database)
		{
			this.client = client;
			this.database = database;

		}
		public MongoClient client { get; private set; }
		public IMongoDatabase database { get; private set; }
		public IMongoCollection<BsonDocument> collection { get; private set; }

		public DatabaseService()
		{

			DatabaseService.DATABASE_NAME = ServiceConfiguration.Instance.configRow("db_name");
			DatabaseService.COLLECTION_NAME = ServiceConfiguration.Instance.configRow("collection_name");
			DatabaseService.USER_ARRAY = ServiceConfiguration.Instance.configRow("user_array");

			this.client = new MongoClient();
			this.database = this.client.GetDatabase(DatabaseService.DATABASE_NAME);
			this.collection = this.database.GetCollection<BsonDocument>(DatabaseService.COLLECTION_NAME);

		}

		public void pushToUser(String user_name, JArray rows)
		{

			IMongoCollection<BsonDocument> collection = this.database.GetCollection<BsonDocument>(DatabaseService.COLLECTION_NAME);

			collection.UpdateOne("{\"user_name\":\"" + user_name + "\"}",
								"{$push: { \"" + DatabaseService.USER_ARRAY + "\": { $each : " + rows.ToString() + " } }}",
								 new UpdateOptions { IsUpsert = true });
			// upsert true -> create document if doesn't exists

		}

		// returns all data for all users
		public String getAllRecords()
		{
			return this.collection.Find(_ => true).ToListAsync().ToString();
		}

		// returns all recors for specified user (index)
		public String getDatFromUser(int index)
		{

			return "";
		}

		// returns records from every user on [index] position
		public String getRecordsAt(int index)
		{

			return "";
		}

		public String getRecordFromUser(int user_index, int record_index)
		{

			return "";
		}

	}
}