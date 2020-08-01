using System.Collections.Generic;
using CollectorService.Configuration;
using System.Net.Http;
using System;
using CommunicationModel;
using Newtonsoft.Json;

namespace CollectorService.Data.Registry
{
	public class InMemoryRegistryCache : IRegistryCache
	{

		private Dictionary<string, SensorRegistryRecord> records;

		private IDatabaseService databse;

		public InMemoryRegistryCache(IDatabaseService database)
		{
			this.databse = database;

			ServiceConfiguration config = ServiceConfiguration.Instance;

			this.records = new Dictionary<string, SensorRegistryRecord>();

			this.pullRecords();
		}

		#region brokerEventHandlers
		private void NewSensorHandler(SensorRegistryRecord newRecord)
		{
			newRecord.LastReadIndex = this.databse.getRecordsCount(newRecord.Name);
			this.records.Add(newRecord.Name, newRecord);
		}

		private void SensorRemovedHandler(SensorRegistryRecord oldRecord)
		{
			this.RemoveRecord(oldRecord);
		}

		#endregion

		public List<SensorRegistryRecord> GetAllSensors()
		{

			if (this.records == null ||
			this.records.Keys.Count == 0)
			{
				this.pullRecords();
			}

			List<SensorRegistryRecord> retList = new List<SensorRegistryRecord>();
			foreach (string recordKey in this.records.Keys)
			{
				SensorRegistryRecord tempRecord = null;
				this.records.TryGetValue(recordKey, out tempRecord);
				retList.Add(tempRecord);
			}

			return retList;
		}

		public SensorRegistryRecord GetSingleSensor(string sensorName)
		{
			SensorRegistryRecord reqRecord = null;

			this.records.TryGetValue(sensorName, out reqRecord);

			return reqRecord;
		}

		public void pullRecords()
		{
			if (this.records == null)
			{
				this.records = new Dictionary<string, SensorRegistryRecord>();
			}

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
				Console.WriteLine($"Exception occured while pulling sensor records: {e.Message}");
			}

			if (responseMessage != null &&
			 responseMessage.IsSuccessStatusCode)
			{
				string txtContent = responseMessage.Content.ReadAsStringAsync().Result;

				List<SensorRegistryRecord> newRecords = JsonConvert.DeserializeObject<List<SensorRegistryRecord>>(txtContent);

				Console.WriteLine($"Registry returned {newRecords.Count} sensor records ... ");

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

		public void RemoveRecord(SensorRegistryRecord oldRecord)
		{
			if (this.records.ContainsKey(oldRecord.Name))
			{
				this.records.Remove(oldRecord.Name);
			}
		}

		public void AddNewRecord(SensorRegistryRecord newRecord)
		{
			if (this.records.ContainsKey(newRecord.Name))
			{
				this.records.Remove(newRecord.Name);
			}

			newRecord.LastReadIndex = this.databse.getRecordsCount(newRecord.Name);
			this.records.Add(newRecord.Name, newRecord);

			Console.WriteLine("sensor: " + newRecord.Name + " last read index: " + newRecord.LastReadIndex);
		}

		public void UpdateRecord(SensorRegistryRecord newRecord)
		{
			if (this.records.ContainsKey(newRecord.Name))
			{
				this.records.Remove(newRecord.Name);
			}

			this.records.Add(newRecord.Name, newRecord);
		}
	}

}