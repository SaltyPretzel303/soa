using System.Linq;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace CRUDService.Data
{
	public class DatabaseService : IDatabaseService
	{
		// externalize this somehow (appsettings.json maybe)
		static private String DATABASE_NAME = "soa";
		static private String COLLECTION_NAME = "Users";
		static private String USER_ARRAY = "values";

		public MongoClient client { get; private set; }
		public IMongoDatabase database { get; private set; }

		public DatabaseService()
		{

			this.client = new MongoClient();
			this.database = this.client.GetDatabase(DatabaseService.DATABASE_NAME);

		}

		public void pushToUser(String user_name, JArray rows)
		{

			IMongoCollection<BsonDocument> collection = this.database.GetCollection<BsonDocument>(DatabaseService.COLLECTION_NAME);
			collection.UpdateOne("{\"user_name\":\"" + user_name + "\"}", "{$push: { \"" + DatabaseService.USER_ARRAY + "\": { $each : " + rows.ToString() + " } }}", new UpdateOptions { IsUpsert = true });
			// upsert true -> create document if doesn't exists

		}

	}
}