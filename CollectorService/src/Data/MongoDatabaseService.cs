using System.Numerics;
using System.Runtime.Serialization.Json;
using System.Globalization;
using System.Linq;
using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using SensorService.Configuration;
using System.Collections.Generic;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace CollectorService.Data
{
	public class DatabaseService : IDatabaseService
	{

		private ServiceConfiguration configuration;

		public MongoClient client { get; private set; }
		public IMongoDatabase database { get; private set; }
		public IMongoCollection<BsonDocument> sensorsCollection { get; private set; }
		public IMongoCollection<BsonDocument> recordsCollection { get; private set; }

		// constructors

		public DatabaseService()
		{

			// TODO inject configuration somehow
			this.configuration = ServiceConfiguration.read();

			this.client = new MongoClient(this.configuration.dbAddress);
			this.database = this.client.GetDatabase(configuration.dbName);
			this.sensorsCollection = this.database.GetCollection<BsonDocument>(configuration.sensorsCollection);
			this.recordsCollection = this.database.GetCollection<BsonDocument>(configuration.recordsCollection);

		}

		// todo not used, but should be ... 
		public DatabaseService(MongoClient client, IMongoDatabase database)
		{

			this.configuration = ServiceConfiguration.read();

			this.client = client;
			this.database = database;

		}

		// methods

		public void pushToSensor(String sensorName, JArray rows)
		{

			// IMongoCollection<BsonDocument> collection = this.database.GetCollection<BsonDocument>(configuration.collectionName);

			// records from this sensor doesn't exit in current database
			// if (this.sensorsCollection.FindSync("{sensor_name:'" + sensorName + "'}").ToList().Count == 0)
			// {
			// 	// add sensor name to collection of sensors
			// 	// this could be some additional data about sensor

			// 	this.sensorsCollection.InsertOne(new BsonDocument("sensor_name", sensorName));

			// }

			// BsonDocument newRecord = new BsonDocument();
			// newRecord.Add("sensor_name", sensorName);
			// newRecord.Add("values", rows.ToString());

			// this.recordsCollection.InsertOne(newRecord);

			sensorsCollection.UpdateOne("{\"sensor_name\":\"" + sensorName + "\"}",
			"{$push: { \"" + this.configuration.fieldWithRecords + "\": { $each : " + rows.ToString() + " } }}",
			 new UpdateOptions { IsUpsert = true });
			// upsert true -> create document if doesn't exists

		}

		// returns all samples
		/*
				jarray=
				[
					{ user_name: user_xx, ... },
					{ user_name: user_xy, ... },
					{ user_name: user_xz, ... },
						.
						.
						.
				]
		 */
		public List<JObject> getAllSamples()
		{

			List<JObject> ret_list = new List<JObject>();

			List<BsonDocument> temp_cache = this.sensorsCollection.FindSync(_ => true).ToList();

			JsonWriterSettings json_settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
			// use this settings to deal with bson document specific "objects"

			foreach (BsonDocument bson_sample in temp_cache)
			{

				JObject json_sample = JObject.Parse(bson_sample.ToJson<BsonDocument>(json_settings));

				ret_list.Add(json_sample);

			}

			return ret_list;
		}

		public List<JObject> getRecordsFromSensor(string sensorName = "sensor_55", long fromTimestamp = 0, long toTimestamp = long.MaxValue)
		{

			string match_q = "{$match: {sensor_name: '" + sensorName.Replace("\"", "") + "'}}";

			string filter_array = "records";
			string single_item = "record";
			string comp_field = "timestamp";

			string gte_q = string.Format(@"{{$gte:['$${0}.{1}','{2}']}}", single_item, comp_field, fromTimestamp);
			string lte_q = string.Format(@"{{$lte:['$${0}.{1}','{2}']}}", single_item, comp_field, toTimestamp);

			string and_q = string.Format(@"{{$and: [{0},{1}]}}", gte_q, lte_q);

			string project_q = string.Format(@"{{$project: {{ {0}: {{ $filter: {{ input: '${0}',as: '{1}',cond: {2} }} }} }} }}", filter_array, single_item, and_q);

			BsonDocument[] agregate_array = new BsonDocument[]{
				BsonDocument.Parse(match_q),
				BsonDocument.Parse(project_q)
				};

			List<BsonDocument> query_result = this.sensorsCollection.Aggregate<BsonDocument>(agregate_array).ToList();

			JsonWriterSettings json_settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
			List<JObject> result = new List<JObject>();
			foreach (BsonDocument single_bson in query_result)
			{

				JObject single_json = JObject.Parse(single_bson.ToJson<BsonDocument>(json_settings));

				result.Add(single_json);

			}

			return result;

		}
	}
}