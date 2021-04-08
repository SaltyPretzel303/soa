using System;
using System.Timers;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using CollectorService.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using CommunicationModel.BrokerModels;
using CommunicationModel;
using System.Collections.Generic;
using MediatR;
using CollectorService.MediatrRequests;

namespace CollectorService.Data
{
	public class DataPuller : IHostedService, IReloadable
	{

		private IMediator mediator;

		private System.Timers.Timer timer;
		private HttpClient httpClient;

		private Dictionary<string, int> lastReadIndex;

		public DataPuller(IMediator mediator)
		{
			this.mediator = mediator;

			this.timer = new System.Timers.Timer();
			this.timer.Elapsed += new ElapsedEventHandler(this.timerEvent);
			this.timer.Interval = ServiceConfiguration.Instance.readInterval;

			this.httpClient = new HttpClient();

			this.lastReadIndex = new Dictionary<string, int>();
		}

		#region IHostedService methods

		public Task StartAsync(CancellationToken cancellationToken)
		{
			ServiceConfiguration.subscribeForChange((IReloadable)this);

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
			Console.WriteLine("Started pulling data with interval: " + this.timer.Interval + "ms");
			this.timer.Start();
		}

		public void stopReading()
		{
			Console.WriteLine("Stopped pulling data ...");
			this.timer.Stop();
		}

		private void timerEvent(Object source, ElapsedEventArgs arg)
		{

			ConfigFields config = ServiceConfiguration.Instance;

			List<SensorRegistryRecord> availableSensors = mediator
				.Send(new GetAllSensorsRequest())
				.Result;

			Console.WriteLine($"Pulling data from: {availableSensors.Count} sensors ... ");
			foreach (SensorRegistryRecord singleSensor in availableSensors)
			{

				int alreadyReadCount = 0;
				if (!this.lastReadIndex.TryGetValue(
						singleSensor.Name,
						out alreadyReadCount))
				{
					// initialize lastReadIndex if it doesn't exists for this sensor
					alreadyReadCount = mediator
						.Send(new GetRecordsCountRequest(singleSensor.Name))
						.Result;

					this.lastReadIndex.Add(singleSensor.Name, alreadyReadCount);
				}

				if (alreadyReadCount >= singleSensor.AvailableRecords)
				{
					Console.WriteLine($"No new records in : {singleSensor.Name} ... ");
					continue; // read from the next sensor 
				}

				string sensorAddr = $"http://{singleSensor.Address}:{singleSensor.Port}";
				string api_url = $"{sensorAddr}/{config.dataRangeUrl}?"
					+ $"sensorName={singleSensor.Name}&"
					+ $"index={alreadyReadCount}";

				Uri sensorUri = new Uri(api_url);
				Console.WriteLine($"Pulling from: {sensorAddr}, "
					+ $"name: {singleSensor.Name}, "
					+ $"index: {alreadyReadCount}");

				HttpResponseMessage response = null;
				try
				{
					response = this.httpClient.GetAsync(sensorUri).Result;
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

						this.mediator.Send(new PublishCollectorPullEventRequest(newEvent));

						continue; // pull from the next sensor
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Exception in gettin data: {e.ToString()}");
					continue; // pull from the next sensor
				}

				if (response == null || !response.IsSuccessStatusCode)
				{
					Console.WriteLine($"Sensor (name: {singleSensor.Name})"
						+ "returned bad response ... ");

					CollectorPullEvent newEvent = new CollectorPullEvent(
														sensorUri.ToString(),
														false,
														"Sensor returned bad response.");

					mediator.Send(new PublishCollectorPullEventRequest(newEvent));

					continue; // pull from the next sensor
				}

				string txtResponseContent = response.Content.ReadAsStringAsync().Result;
				// SensorDataRecords dataRecords = System.Text.Json.JsonSerializer.Deserialize<SensorDataRecords>(txtResponseContent);
				// newtonsoft serializes properties to camelCase so when they get pulled from sensor
				// system.text.json can't deserialize it because class properties are actually in PascalCase 
				// that is the reason to use newtonsoft - easier than forcing it to serialize in PascalCase

				SensorDataRecords dataRecords = JsonConvert
						.DeserializeObject<SensorDataRecords>(txtResponseContent);

				Console.WriteLine($"Sensor {singleSensor.Name}"
					+ $" returned {dataRecords.RecordsCount} rows ... ");

				// // deserialize all records from response
				// // ListdataRecords.Records -> list of strings each representing one row from csv
				// JArray dataArray = new JArray();
				// foreach (string txtData in dataRecords.Records)
				// {
				// 	JObject tempData = JObject.Parse(txtData);
				// 	dataArray.Add(tempData);
				// }

				this.mediator.Send(new AddRecordsToSensorRequest(
											singleSensor.Name,
											dataRecords.Records));

				// update read index for every sensor
				// at this point lastReadIndex[singleSensor.Name] has to exists (look at the beginning of this for loop)
				this.lastReadIndex[singleSensor.Name] = alreadyReadCount + dataRecords.RecordsCount;

			}

		}

		public void reload(ConfigFields newConfig)
		{
			// with every timerEvent configuration is read again
			// only readInterval is kept from the initial service construction

			if (this.timer.Interval != newConfig.readInterval)
			{
				this.timer.Stop();
				this.timer.Interval = newConfig.readInterval;
				this.timer.Start();
			}

			Console.WriteLine("Data puller reloaded ...  ");
		}

	}

}