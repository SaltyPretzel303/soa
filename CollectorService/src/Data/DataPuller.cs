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

		private Dictionary<string, long> lastReadIndexes;

		public DataPuller(IMediator mediator)
		{
			this.mediator = mediator;

			timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(this.timerEvent);
			timer.Interval = ServiceConfiguration.Instance.readInterval;
			timer.AutoReset = false;

			httpClient = new HttpClient();

			lastReadIndexes = new Dictionary<string, long>();
		}

		#region IHostedService methods

		public Task StartAsync(CancellationToken cancellationToken)
		{
			ServiceConfiguration.subscribeForChange((IReloadable)this);

			timer.Start();
			Console.WriteLine("Started pulling data with interval: "
				+ $"{timer.Interval} ms");

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			if (timer != null)
			{
				timer.Stop();
			}

			return Task.CompletedTask;
		}

		#endregion

		private async void timerEvent(Object source, ElapsedEventArgs arg)
		{
			// prevent calling this method again until the current execution is done
			// in case timer.Interval is shorter than the time required to pull data
			// from all the sensors
			timer.Stop();

			ConfigFields config = ServiceConfiguration.Instance;

			List<SensorRegistryRecord> availableSensors =
				await mediator.Send(new GetAllSensorsRequest());

			int triedToPullCount = 0;
			Console.WriteLine($"Available sensors count: {availableSensors.Count} ... ");
			foreach (var singleSensor in availableSensors)
			{
				long alreadyReadCount = 0;
				if (!lastReadIndexes.TryGetValue(
						singleSensor.Name,
						out alreadyReadCount))
				{
					// get the count of already read records from this sensor
					// db access
					alreadyReadCount = await mediator
						.Send(new GetRecordsCountRequest(singleSensor.Name));

					if (alreadyReadCount == -1)
					{
						Console.WriteLine("Failed to get read count for: "
							+ $"{singleSensor.Name}");
						// something went wrong
						// most possibly failed to connect with the db

						continue; // read from the next senor 
					}

					lastReadIndexes.Add(singleSensor.Name, alreadyReadCount);
				}

				if (alreadyReadCount >= singleSensor.AvailableRecords)
				{
					long diff = alreadyReadCount - singleSensor.AvailableRecords;
					Console.WriteLine($"{singleSensor.Name} -> no new records "
						+ $"waiting for: {diff} reads ... ");

					continue; // read from the next sensor 
				}

				string sensorAddr = $"http://{singleSensor.Address}:{singleSensor.Port}";
				string api_url = $"{sensorAddr}/{config.dataRangeUrl}?"
					+ $"sensorName={singleSensor.Name}&"
					+ $"index={alreadyReadCount}";

				Uri sensorUri = new Uri(api_url);
				Console.Write($"{singleSensor.Name}@{sensorAddr} "
					+ $" | from: {alreadyReadCount} -> ");

				HttpResponseMessage response = null;
				try
				{
					triedToPullCount++;
					response = await this.httpClient.GetAsync(sensorUri);
				}
				catch (AggregateException e)
				{
					// most possibly this exception happened because data pulling 
					// is started with old list of sensors
					// sensors are removed from registry (shutdown events are received)
					// after 'current sensors list is returned'
					if (e.InnerException is HttpRequestException)
					{
						string message =
							((HttpRequestException)e.InnerException).Message;

						Console.WriteLine(
							$"\nHttp req. exception, message: {message} ... "
							+ $"sensor may be down.\n"
							+ $"Sensor addr. : {sensorUri.ToString()}\n");

						var newEvent = new CollectorPullEvent(
							config.serviceId,
							sensorUri.ToString(),
							false,
							0,
							e.Message);

						// handler for this request is not async 
						// but i guess it is a good idea to await it anyway 
						await mediator.Send(new PublishCollectorPullEventRequest(newEvent));

						continue; // pull from the next sensor
					}
				}
				catch (HttpRequestException e)
				{
					// same handler as for above catch ... 
					// i guess aggregateException is removed after some update
					string message = e.Message;

					Console.WriteLine(
						$"\nHttp req. exception, message: {message} ... "
						+ $"sensor may be down.\n"
						+ $"Sensor addr. : {sensorUri.ToString()}\n");

					var newEvent = new CollectorPullEvent(
						config.serviceId,
						sensorUri.ToString(),
						false,
						0,
						e.Message);

					// handler for this request is not async 
					// but i guess it is a good idea to await it anyway 
					await mediator.Send(new PublishCollectorPullEventRequest(newEvent));

					continue; // pull from the next sensor
				}
				catch (Exception e)
				{
					Console.WriteLine($"Unexpected exception while pulling data: "
						+ $" {e.ToString()}");
					continue; // pull from the next sensor
				}

				if (response == null || !response.IsSuccessStatusCode)
				{
					// this will continue previous Console.Write( ... );
					Console.WriteLine($" bad response ... ");

					var newEvent = new CollectorPullEvent(
						config.serviceId,
						sensorUri.ToString(),
						false,
						0,
						"Sensor returned bad response.");

					// handler for this request is not async 
					// but i guess it is a good idea to await it anyway 
					await mediator.Send(new PublishCollectorPullEventRequest(newEvent));

					continue; // pull from the next sensor
				}

				string txtContent = await response.Content.ReadAsStringAsync();
				// var dataRecords = System
				// 		.Text
				// 		.Json
				// 		.JsonSerializer
				// 		.Deserialize<SensorDataRecords>(txtResponseContent);
				// newtonsoft serializes properties to camelCase so when they get
				// pulled from sensor system.text.json can't deserialize it because
				// class properties are actually in PascalCase, that is the reason 
				// to use newtonsoft - easier than forcing it to serialize in PascalCase

				var dataRecords = JsonConvert
						.DeserializeObject<SensorDataRecords>(txtContent);

				Console.WriteLine($"returned {dataRecords.RecordsCount} rows ... ");

				var pullEvent = new CollectorPullEvent(
					config.serviceId,
					sensorUri.ToString(),
					true,
					dataRecords.RecordsCount);

				await mediator.Send(new PublishCollectorPullEventRequest(pullEvent));

				var addResult = await mediator.Send(
					new AddRecordsToSensorRequest(
						singleSensor.Name,
						dataRecords.CsvHeader,
						dataRecords.CsvValues)
					);

				if (addResult != true)
				{
					// adding the new records was not successfull
					// most possibly database is down ... 
					// this records will have to be pulled again

					continue;
				}

				// update read index for every sensor that returned records
				// at this point lastReadIndex[singleSensor.Name] has to exists 
				// (look at the beginning of this for loop)
				lastReadIndexes[singleSensor.Name] =
					alreadyReadCount + dataRecords.RecordsCount;

			}

			Console.WriteLine($"Tried to pull from: {triedToPullCount} ...");
			// 'schedule' the next data pulling
			timer.Start();
		}

		public Task reload(ConfigFields newConfig)
		{
			// with every timerEvent configuration is read again
			// only readInterval is kept from the initial service construction

			if (timer.Interval != newConfig.readInterval)
			{
				bool timerWasEnabled = timer.Enabled;
				timer.Stop();

				timer.Interval = newConfig.readInterval;

				if (timerWasEnabled)
				{
					timer.Start();
				}
			}

			Console.WriteLine("Data puller reloaded ...  ");

			return Task.CompletedTask;
		}

	}
}