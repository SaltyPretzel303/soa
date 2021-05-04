using System.Collections.Generic;
using CollectorService.Configuration;
using System.Net.Http;
using System;
using CommunicationModel;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CollectorService.Data.Registry
{
	public class InMemoryRegistryCache : IRegistryCache
	{
		// static so it can be transient instead of singleton service
		private static ConcurrentDictionary<string, SensorRegistryRecord> Records { get; set; }

		// used to get last read index for specific sensor
		private IDatabaseService database;

		private ConfigFields config;

		public InMemoryRegistryCache(IDatabaseService database)
		{
			this.database = database;
			this.config = ServiceConfiguration.Instance;

			if (Records == null)
			{
				Records = new ConcurrentDictionary<string, SensorRegistryRecord>();
			}
		}

		public async Task<List<SensorRegistryRecord>> GetAllRecords()
		{
			if (Records == null || Records.Keys.Count == 0)
			{
				await pullRecords();
			}

			return new List<SensorRegistryRecord>(Records.Values);

			// var retList = new List<SensorRegistryRecord>();
			// foreach (string recordKey in Records.Keys)
			// {
			// 	SensorRegistryRecord tempRecord = null;
			// 	Records.TryGetValue(recordKey, out tempRecord);
			// 	retList.Add(tempRecord);
			// }

			// return retList;
		}

		private async Task pullRecords()
		{
			string addr = $"http://{config.sensorRegistryAddr}"
				+ $":{config.sensorRegistryPort}"
				+ $"/{config.sensorListReqPath}";

			HttpClient httpClient = new HttpClient();
			HttpResponseMessage responseMessage = null;
			try
			{
				responseMessage = await httpClient.GetAsync(addr);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception occurred while pulling active sensors records: {e.Message}");
				return;
			}

			if (responseMessage != null && responseMessage.IsSuccessStatusCode)
			{
				var txtContent = await responseMessage
					.Content
					.ReadAsStringAsync();

				var newRecords = JsonConvert
					.DeserializeObject<List<SensorRegistryRecord>>(txtContent);

				if (newRecords.Count > 0)
				{
					Console.WriteLine($"Registry returned {newRecords.Count} sensor records ... ");
				}

				foreach (SensorRegistryRecord newSensor in newRecords)
				{
					await AddNewRecord(newSensor);
				}
			}
			else
			{
				Console.WriteLine("Failed to pull sensorRegistryRecords ... ");
				return;
			}
		}

		public SensorRegistryRecord GetRecord(string sensorName)
		{
			SensorRegistryRecord reqRecord = null;

			Records.TryGetValue(sensorName, out reqRecord);

			return reqRecord;
		}

		public void RemoveRecord(SensorRegistryRecord oldRecord)
		{
			SensorRegistryRecord outRecord = new SensorRegistryRecord();

			if (Records.ContainsKey(oldRecord.Name))
			{
				Records.TryRemove(oldRecord.Name, out outRecord);
			}
		}

		public async Task AddNewRecord(SensorRegistryRecord newRecord)
		{
			SensorRegistryRecord outRecord = new SensorRegistryRecord();
			if (Records.ContainsKey(newRecord.Name))
			{
				Records.TryRemove(newRecord.Name, out outRecord);
			}

			newRecord.AvailableRecords = await database.getRecordsCount(newRecord.Name);

			Records.TryAdd(newRecord.Name, newRecord);
		}

		public void UpdateRecord(SensorRegistryRecord newRecord)
		{
			SensorRegistryRecord outRecord = new SensorRegistryRecord();
			if (Records.ContainsKey(newRecord.Name))
			{
				Records.TryRemove(newRecord.Name, out outRecord);
			}

			Records.TryAdd(newRecord.Name, newRecord);
		}
	}
}