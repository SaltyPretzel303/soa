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

		// methods

		public void pushToSensor(String sensorName, JArray rows)
		{

			if (!this.createConnection())
			{
				return;
			}

			ServiceConfiguration configuration = ServiceConfiguration.Instance;

			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(configuration.sensorsCollection);

			sensorsCollection.UpdateOne("{\"sensor_name\":\"" + sensorName + "\"}",
								"{$push: { \"" + configuration.fieldWithRecords + "\": { $each : " + rows.ToString() + " } }}",
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

			if (!this.createConnection())
			{
				return null;
			}

			List<JObject> ret_list = new List<JObject>();

			ServiceConfiguration config = ServiceConfiguration.Instance;

			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);
			List<BsonDocument> temp_cache = sensorsCollection.FindSync(_ => true).ToList();

			JsonWriterSettings json_settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
			// use this settings to deal with bson document specific "objects"

			foreach (BsonDocument bson_sample in temp_cache)
			{
				JObject json_sample = JObject.Parse(bson_sample.ToJson<BsonDocument>(json_settings));

				ret_list.Add(json_sample);
			}

			return ret_list;
		}

		public SensorDataRecords getRecordRange(string sensorName = ".", long fromTimestamp = 0, long toTimestamp = long.MaxValue)
		{

			if (!this.createConnection())
			{
				return null;
			}

			// TODO deal with 'sql (mongo) ijection' through sensorName, it is used as regex for mongo query

			// TODO why is there regex ... ? 
			string match_q = "{$match: {sensor_name: {$regex: '" + sensorName.Replace("\"", "") + "'}}}";

			string filter_array = "records";
			string single_item = "record";
			string comp_field = "timestamp";

			string gte_q = string.Format(@"{{$gte:[{{$convert: {{ input: '$${0}.{1}',to: 'long' }} }}, NumberLong({2})]}}",
									single_item,
									comp_field,
									fromTimestamp);
			string lte_q = string.Format(@"{{$lte:[{{$convert: {{ input: '$${0}.{1}',to: 'long' }} }}, NumberLong({2})]}}",
									single_item,
									comp_field,
									toTimestamp);

			string and_q = string.Format(@"{{$and: [{0},{1}]}}",
									gte_q,
									lte_q);

			string project_q = string.Format(@"{{$project: {{ {0}: {{ $filter: {{ input: '${0}',as: '{1}',cond: {2} }} }},sensor_name:1,_id:0 }} }}",
										filter_array,
										single_item,
										and_q);

			BsonDocument[] agregate_array = new BsonDocument[]{
					BsonDocument.Parse(match_q),
					BsonDocument.Parse(project_q)
			};

			ServiceConfiguration config = ServiceConfiguration.Instance;

			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);
			List<BsonDocument> query_result = sensorsCollection.Aggregate<BsonDocument>(agregate_array).ToList();

			SensorDataRecords resultRecords = new SensorDataRecords()
			{
				SensorName = sensorName,
				RecordsCount = query_result.Count
			};


			JsonWriterSettings json_settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
			foreach (BsonDocument single_bson in query_result)
			{
				JObject single_json = JObject.Parse(single_bson.ToJson<BsonDocument>(json_settings));
				resultRecords.Records.Add(single_json.ToString(Formatting.None).Replace("\\n", " "));
			}

			return resultRecords;

		}

		public SensorDataRecords getRecordsList(string sensorName, List<string> timestamps)
		{
			if (!this.createConnection())
			{
				return null;
			}

			// TODO deal with 'sql (mongo) ijection' through sensorName, it is used as regex for mongo query

			// TODO why is there regex ... ? 
			string match_q = "{$match: {sensor_name: {$regex: '" + sensorName.Replace("\"", "") + "'}}}";

			string filter_array = "records";
			string single_item = "record";
			string comp_field = "timestamp";

			string s_timestamps = "";
			for (int i = 0; i < timestamps.Count; i++)
			{
				if (i != 0)
				{
					s_timestamps += ", ";
				}
				s_timestamps += $"\"{timestamps[i]}\"";
			}

			string in_q = string.Format(@"{{$in: ['$${0}.{1}',[{2}]] }}", single_item, comp_field, s_timestamps);

			string project_q = string.Format(@"{{$project: {{ {0}: {{ $filter: {{ input: '${0}',as: '{1}',cond: {2} }} }},_id:0 }} }}",
										filter_array,
										single_item,
										in_q);

			Console.WriteLine($"Query: {project_q}");

			BsonDocument[] agregate_array = new BsonDocument[]{
					BsonDocument.Parse(match_q),
					BsonDocument.Parse(project_q)
			};

			ServiceConfiguration config = ServiceConfiguration.Instance;

			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);
			List<BsonDocument> query_result = sensorsCollection.Aggregate<BsonDocument>(agregate_array).ToList();

			SensorDataRecords resultRecords = new SensorDataRecords()
			{
				SensorName = sensorName,
				RecordsCount = query_result.Count
			};

			JsonWriterSettings json_settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
			foreach (BsonDocument single_bson in query_result)
			{
				JObject single_json = JObject.Parse(single_bson.ToJson<BsonDocument>(json_settings));
				resultRecords.Records.Add(single_json.ToString(Formatting.None).Replace("\\n", " "));
			}

			return resultRecords;
		}

		public bool updateRecord(string sensorName, string timestamp, string field, string value)
		{

			if (!this.createConnection())
			{
				return false;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);

			string s_filter = String.Format(@"{{ 'sensor_name': '{0}', 'records.timestamp':'{1}' }}", sensorName, timestamp);
			string s_command = String.Format(@"{{ $set: {{ 'records.$.{0}': '{1}' }} }}", field, value);
			BsonDocument b_filter = BsonDocument.Parse(s_filter);
			BsonDocument b_command = BsonDocument.Parse(s_command);

			UpdateResult query_result = sensorsCollection.UpdateOne((FilterDefinition<BsonDocument>)b_filter,
																(UpdateDefinition<BsonDocument>)b_command);

			if (query_result != null &&
				query_result.MatchedCount > 0 &&
				query_result.ModifiedCount > 0)
			{
				return true;
			}

			return false; ;
		}

		public bool deleteRecord(string sensorName, string timestamp)
		{

			if (!this.createConnection())
			{
				return false;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);

			string s_filter = String.Format(@"{{ 'sensor_name': '{0}'}}", sensorName);
			string s_command = String.Format(@"{{ $pull: {{ records: {{ 'timestamp':'{0}' }} }} }}", timestamp);
			BsonDocument b_filter = BsonDocument.Parse(s_filter);
			BsonDocument b_command = BsonDocument.Parse(s_command);

			UpdateResult query_result = sensorsCollection.UpdateOne((FilterDefinition<BsonDocument>)b_filter,
																(UpdateDefinition<BsonDocument>)b_command);

			if (query_result != null &&
				query_result.MatchedCount > 0 &&
				query_result.ModifiedCount > 0)
			{
				return true;
			}

			return false;
		}

		public bool deleteSensorData(string sensorName)
		{

			if (!this.createConnection())
			{
				return false;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);

			string s_filter = String.Format(@"{{ 'sensor_name': '{0}'}}", sensorName);
			BsonDocument b_filter = BsonDocument.Parse(s_filter);

			DeleteResult query_result = sensorsCollection.DeleteOne((FilterDefinition<BsonDocument>)b_filter);

			if (query_result != null &&
				query_result.DeletedCount > 0)
			{
				return true;
			}

			return false;
		}

		public void backupConfiguration(JObject oldJConfig)
		{

			if (!this.createConnection())
			{
				return;
			}

			String serviceAddr = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

			ServiceConfiguration oldOConfig = ServiceConfiguration.Instance;

			IMongoCollection<BsonDocument> configCollection = this.database.GetCollection<BsonDocument>(oldOConfig.configurationBackupCollection);

			oldJConfig[oldOConfig.configBackupField] = DateTime.Now.ToString();

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

		public List<JObject> customQuery()
		{

			if (!this.createConnection())
			{
				return null;
			}

			List<JObject> retList = new List<JObject>();

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);
			List<BsonDocument> temp_cache = sensorsCollection.FindSync("{}").ToList();

			Console.WriteLine("\n\nGot: " + temp_cache.Count + " lines ... \n\n");

			JsonWriterSettings json_settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

			foreach (BsonDocument bson_sample in temp_cache)
			{

				JObject jsample = JObject.Parse(bson_sample.ToJson<BsonDocument>(json_settings));

				JArray array = (JArray)jsample["records"];

				Console.WriteLine("\n\n\nCOUNT : " + array.Count + "\n\n\n");

				retList.Add(jsample);

			}

			return retList;

		}

		public int getRecordsCount(string sensorName)
		{

			if (!this.createConnection())
			{
				return 0;
			}

			ServiceConfiguration config = ServiceConfiguration.Instance;
			string countFieldString = "records_count";
			int count = 0;

			string sMatchQuery = "{$match: {sensor_name:\"" + sensorName + "\" }}";
			string sCountQuery = "{$project: { " + countFieldString + ": {$size: \"$" + config.fieldWithRecords + "\" } }}";

			BsonDocument[] aggregateArray = new BsonDocument[]{
				BsonDocument.Parse(sMatchQuery),
				BsonDocument.Parse(sCountQuery)
			};

			IMongoCollection<BsonDocument> sensorsCollection = this.database.GetCollection<BsonDocument>(config.sensorsCollection);
			List<BsonDocument> queryResult = sensorsCollection.Aggregate<BsonDocument>(aggregateArray).ToList();

			if (queryResult != null && queryResult.Count > 0)
			{

				JsonWriterSettings jsonSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

				JObject jsonResult = JObject.Parse(queryResult[0].ToJson<BsonDocument>(jsonSettings));

				count = jsonResult[countFieldString].Value<int>();

				Console.WriteLine("Got count: " + count);

				return count;

			}

			return count;
		}

	}

}