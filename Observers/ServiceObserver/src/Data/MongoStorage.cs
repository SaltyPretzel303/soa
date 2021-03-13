using System;
using System.Linq;
using System.Net.NetworkInformation;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ServiceObserver.Configuration;

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
				return;
			}

			String serviceAddr = NetworkInterface.
								GetAllNetworkInterfaces().
								Where(nic => nic.OperationalStatus == OperationalStatus.Up
											&& nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).
								Select(nic => nic.GetPhysicalAddress().ToString()).
								FirstOrDefault();

			ServiceConfiguration oldObjConfig = ServiceConfiguration.Instance;

			IMongoCollection<BsonDocument> configCollection = this.database.GetCollection<BsonDocument>(oldObjConfig.configurationBackupCollection);

			oldJConfig[oldObjConfig.configBackupDateField] = DateTime.Now.ToString();

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

	}
}