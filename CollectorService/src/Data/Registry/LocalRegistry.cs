using System.Collections.Generic;
using CollectorService.Configuration;
using System.Net.Http;
using CollectorService.Broker;
using System;

namespace CollectorService.Data.Registry
{

	public class LocalRegistry
	{

		private IDatabaseService databse;

		public LocalRegistry(IDatabaseService database)
		{

			this.databse = database;

			this.pullRecords();

			MessageBroker.Instance.subscribeForSensorRegistry(this.handleNewSensor, this.handleSensorRemoval);

		}

		private List<SensorRecord> sensors;

		// TODO handle socket exception if sensor registry is not active
		public void pullRecords()
		{

			// clear previous sensors list
			this.sensors = new List<SensorRecord>();

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			string addr = $"http://{conf.sensorRegistryAddr}:{conf.sensorRegistryPort}/{conf.sensorListReqPath}";

			HttpClient httpClient = new HttpClient();
			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result;

			responseMessage.sta
			HttpStatusCode

			if (responseMessage.IsSuccessStatusCode)
			{

				this.sensors = responseMessage.Content.ReadAsAsync<List<SensorRecord>>().Result;
				if (this.sensors == null)
				{
					Console.WriteLine("Pulled sensors list was null ...");
					this.sensors = new List<SensorRecord>();
				}

				Console.WriteLine($"Registry returned {this.sensors.Count} sensor records ... ");

				foreach (SensorRecord sensor in this.sensors)
				{

					sensor.lastReadIndex = this.databse.getRecordsCount(sensor.name);
					Console.WriteLine("sensor: " + sensor.name + " last read index: " + sensor.lastReadIndex);

				}

			}

		}

		public List<SensorRecord> getRecords()
		{
			return this.sensors;
		}

		private void handleNewSensor(SensorRecord newRecord)
		{

			newRecord.lastReadIndex = this.databse.getRecordsCount(newRecord.name);
			this.sensors.Add(newRecord);

		}

		private void handleSensorRemoval(SensorRecord oldRecord)
		{

			this.removeRecord(oldRecord);

		}

		private void removeRecord(SensorRecord oldRecord)
		{
			foreach (SensorRecord singleRecord in this.sensors)
			{
				if (singleRecord.name == oldRecord.name)
				{
					this.sensors.Remove(singleRecord);
					return;
				}
			}
		}

		public SensorRecord getSensor(string sensorName)
		{

			foreach (SensorRecord singleSensor in this.sensors)
			{
				if (singleSensor.name == sensorName)
				{
					return singleSensor;
				}
			}

			return null;

		}

	}
}