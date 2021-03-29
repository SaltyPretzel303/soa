using System.Collections.Generic;
using CollectorService.Configuration;
using System.Net.Http;
using System;
using CommunicationModel;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace CollectorService.Data.Registry
{
	public class InMemoryRegistryCache : IRegistryCache
	{

		// static so it can be transient instead of singleton service
		private static ConcurrentDictionary<string, SensorRegistryRecord> Records { get; set; }

		// used to get last read index for specific sensor
		private IDatabaseService database;

		public InMemoryRegistryCache(IDatabaseService database)
		{
			this.database = database;

			ServiceConfiguration config = ServiceConfiguration.Instance;

			if (Records == null)
			{
				Records = new ConcurrentDictionary<string, SensorRegistryRecord>();
				this.pullRecords();
			}
		}

		private void pullRecords()
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			string addr = $"http://{conf.sensorRegistryAddr}:{conf.sensorRegistryPort}/{conf.sensorListReqPath}";

			HttpClient httpClient = new HttpClient();
			HttpResponseMessage responseMessage = null;
			try
			{
				responseMessage = httpClient.GetAsync(addr).Result;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception occured while pulling active sensors records: {e.Message}");
			}

			if (responseMessage != null
			&& responseMessage.IsSuccessStatusCode)
			{
				string txtContent = responseMessage.Content.ReadAsStringAsync().Result;

				List<SensorRegistryRecord> newRecords = JsonConvert.DeserializeObject<List<SensorRegistryRecord>>(txtContent);

				// Console.WriteLine($"Registry returned {newRecords.Count} sensor records ... ");

				foreach (SensorRegistryRecord newSensor in newRecords)
				{
					this.AddNewRecord(newSensor);
				}
			}
			else
			{
				Console.WriteLine("Failed to pull sensorRegistryRecords ... ");
			}

		}

		public List<SensorRegistryRecord> GetAllSensors()
		{

			if (Records == null ||
			Records.Keys.Count == 0)
			{
				this.pullRecords();
			}

			List<SensorRegistryRecord> retList = new List<SensorRegistryRecord>();
			foreach (string recordKey in Records.Keys)
			{
				SensorRegistryRecord tempRecord = null;
				Records.TryGetValue(recordKey, out tempRecord);
				retList.Add(tempRecord);
			}

			return retList;
		}

		public SensorRegistryRecord GetSingleSensor(string sensorName)
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

		public void AddNewRecord(SensorRegistryRecord newRecord)
		{
			SensorRegistryRecord outRecord = new SensorRegistryRecord();
			if (Records.ContainsKey(newRecord.Name))
			{
				Records.TryRemove(newRecord.Name, out outRecord);
			}

			newRecord.AvailableRecords = this.database.getRecordsCount(newRecord.Name);
			Records.TryAdd(newRecord.Name, newRecord);

			Console.WriteLine("sensor: " + newRecord.Name + " last read index: " + newRecord.AvailableRecords);
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