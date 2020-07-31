using System;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using CollectorService.Broker;
using CollectorService.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using CommunicationModel.BrokerModels;
using CommunicationModel;
using System.Collections.Generic;

namespace CollectorService.Data
{
	public class DataPuller : IHostedService, IReloadable
	{

		private IDatabaseService database;
		private IRegistryCache localRegistry;
		private IMessageBroker messageBroker;

		private System.Timers.Timer timer;
		private HttpClient httpClient;

		public DataPuller(IDatabaseService databse, IRegistryCache localRegistry, IMessageBroker messageBroker)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			this.database = databse;
			this.localRegistry = localRegistry;
			this.messageBroker = messageBroker;

			this.timer = new System.Timers.Timer();
			this.timer.Elapsed += new ElapsedEventHandler(this.timerEvent);
			this.timer.Interval = config.readInterval;

			this.httpClient = new HttpClient();
		}

		#region IHostedService methods

		public Task StartAsync(CancellationToken cancellationToken)
		{
			startReading();
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			stopReading();
			return Task.CompletedTask;
		}

		#endregion

		public void startReading()
		{
			Console.WriteLine("Started reading with interval: " + this.timer.Interval + "ms");
			this.timer.Start();
		}

		public void stopReading()
		{
			this.timer.Stop();
		}

		private void timerEvent(Object source, ElapsedEventArgs arg)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			List<SensorRegistryRecord> availableSensors = this.localRegistry.GetAllSensors();

			Console.WriteLine($"Ready to pull from: {availableSensors.Count} sensors ... ");
			foreach (SensorRegistryRecord singleSensor in availableSensors)
			{

				int sensorLastReadIndex = singleSensor.LastReadIndex;

				string sensorAddr = $"http://{singleSensor.Address}:{singleSensor.Port}";
				string api_url = $"{sensorAddr}/{config.dataRangeUrl}?sensorName={singleSensor.Name}&index={sensorLastReadIndex}";

				Uri sensorUri = new Uri(api_url);
				Console.WriteLine("Pulling from: " + api_url);

				HttpResponseMessage responseMessage = null;
				try
				{
					responseMessage = this.httpClient.GetAsync(sensorUri).Result;
				}
				catch (AggregateException e)
				{
					if (e.InnerException is HttpRequestException)
					{
						string message = ((HttpRequestException)e.InnerException).Message;

						Console.WriteLine($"Http req. exception, message: {message} ... sensor may be down.\nSensor addr. : {sensorUri.ToString()}\n");

						CollectorPullEvent newEvent = new CollectorPullEvent(sensorUri.ToString(),
																		false,
																		e.Message);

						this.messageBroker.PublishCollectorPullEvent(newEvent);

						continue; // pull from next sensor
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"UNKNOWN EXCEPTION in gettin data: {e.ToString()}");
					continue; // pull from next sensor
				}

				if (responseMessage == null ||
				!responseMessage.IsSuccessStatusCode)
				{
					Console.WriteLine($"Sensor ({sensorUri.ToString()}) returned bad response ... ");

					CollectorPullEvent newEvent = new CollectorPullEvent(sensorUri.ToString(),
																	false,
																	"Sensor returned bad response.");

					this.messageBroker.PublishCollectorPullEvent(newEvent);


					continue; // pull from next sensor
				}

				string txtResponseContent = responseMessage.Content.ReadAsStringAsync().Result;
				// SensorDataRecords dataRecords = System.Text.Json.JsonSerializer.Deserialize<SensorDataRecords>(txtResponseContent);
				// newtonsoft serializes properties to camelCase so when they get pulled from sensor
				// system.text.json can't deserialize it because class properties are actually in PascalCase 
				// that is the reason to use newtonsoft - easier than forcing it to serialize in PascalCase
				SensorDataRecords dataRecords = JsonConvert.DeserializeObject<SensorDataRecords>(txtResponseContent);

				// deserialize all records from response
				// ListdataRecords.Records -> list of strings each representing one row from csv
				JArray dataArray = new JArray();
				foreach (string txtData in dataRecords.Records)
				{
					JObject tempData = JObject.Parse(txtData);
					dataArray.Add(tempData);
				}

				this.database.pushToSensor(singleSensor.Name, dataArray);

				// update read index for every sensor
				this.localRegistry.GetSingleSensor(singleSensor.Name).LastReadIndex += dataRecords.RecordsCount;

				Console.WriteLine($"Sensor {singleSensor.Name} returned {dataRecords.RecordsCount} rows ... ");

			}

		}

		public void shutDown()
		{
			this.timer.Stop();
		}

		public void reload(ServiceConfiguration newConfiguration)
		{
			Console.WriteLine("Reloading data puller: " + newConfiguration.ToString());
		}

	}

}