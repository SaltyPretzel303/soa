using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using System.Collections.Generic;
using CollectorService.Configuration;
using System.Net.Http;
using CollectorService.Broker;
using System;
using Newtonsoft.Json.Linq;

namespace CollectorService.Data.Registry
{
	public class LocalRegistry

	{

		#region singleton_stuff
		private static LocalRegistry instance;
		public static LocalRegistry Instance
		{
			get
			{

				if (LocalRegistry.instance == null)
				{
					LocalRegistry.instance = new LocalRegistry();
				}

				return LocalRegistry.instance;
			}
			set
			{
				LocalRegistry.instance = value;
			}
		}

		protected LocalRegistry()
		{

			this.sensors = new List<SensorRecord>();

			this.pullRecords();

			MessageBroker.Instance.subscribeForSensorRegistry(this.handleNewSensor, this.handleSensorRemoval);

		}
		#endregion singleton_stuff

		private List<SensorRecord> sensors;

		public void pullRecords()
		{

			// clear previous sensors list
			this.sensors = new List<SensorRecord>();

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			string addr = $"http://{conf.sensorRegistryAddr}:{conf.sensorRegistryPort}/{conf.sensorListReqPath}";

			HttpClient httpClient = new HttpClient();
			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result;

			if (responseMessage.IsSuccessStatusCode)
			{

				this.sensors = responseMessage.Content.ReadAsAsync<List<SensorRecord>>().Result;
				if (this.sensors == null)
				{
					Console.WriteLine("Pulled sensors list was null ...");
					this.sensors = new List<SensorRecord>();
				}

				Console.WriteLine($"Registry returned {this.sensors.Count} sensor records ... ");

			}

		}

		public List<SensorRecord> getRecords()
		{
			return this.sensors;
		}

		private void handleNewSensor(SensorRecord newRecord)
		{

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