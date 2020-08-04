using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using SensorService.Configuration;
using SensorService.Logger;

namespace SensorService
{
	public class RegistrationService : IHostedService
	{

		private List<string> waitingSensors;

		private System.Timers.Timer timer;

		private HttpClient httpClient;

		private ILogger logger;
		private IDataCacheManager dataCache;

		public RegistrationService(ILogger logger,
								IDataCacheManager dataCache)
		{
			this.logger = logger;
			this.dataCache = dataCache;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			this.waitingSensors = new List<string>();
			for (int sensorIndex = conf.sensorsRange.From;
					sensorIndex < conf.sensorsRange.To;
					sensorIndex++)
			{
				this.waitingSensors.Add(conf.sensorNamePrefix + sensorIndex);
			}

			this.httpClient = new HttpClient();

			if (this.waitingSensors.Count > 0)
			{
				this.timer = new System.Timers.Timer();
				this.timer.Elapsed += this.TimerEvent;
				this.timer.Interval = conf.registerSensorDelay;
				this.timer.AutoReset = true;
				this.timer.Start();
			}

			return Task.CompletedTask;
		}


		private void TimerEvent(Object source, ElapsedEventArgs args)
		{

			Console.WriteLine("register timer event ... ");

			bool regResult = false;
			for (int i = 0; i < waitingSensors.Count; i++)
			{
				string currentSensor = waitingSensors[i];

				regResult = this.registerSensor(currentSensor);

				if (regResult == true)
				{
					// sensor successfully registered
					this.logger.logMessage($"Sensor {currentSensor} successfully registred ... ");
					this.waitingSensors.Remove(currentSensor);
					i--;
				}

			}

			if (this.waitingSensors.Count == 0)
			{
				this.timer.Stop();
			}

		}

		private bool registerSensor(string sensorName)
		{
			ServiceConfiguration conf = ServiceConfiguration.Instance;

			int lastReadIndex = this.dataCache.GetLastReadIndex(sensorName);

			// e.g. http://localhost/sensor/registry/registerSensor?sensorName="sensor_1"&portNum=5050&lastReadIndex=12
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.registerSensorPath}?{conf.sensorNameField}={sensorName}&{conf.portNumField}={conf.listeningPort}&{conf.readIndexField}={lastReadIndex}";
			// port on which this sensor is serving http requests

			// this method could possibly throw exception 
			HttpResponseMessage responseMessage = null;
			try
			{
				responseMessage = this.httpClient.GetAsync(addr).Result; // .Result is going to force blocking execution
			}
			catch (Exception e)
			{
				this.logger.logError($"Failed to register sensor on: {addr}, reason: {e.Message}");
				return false;
			}

			if (responseMessage != null &&
				responseMessage.IsSuccessStatusCode)
			{
				return true;
			}
			else
			{

				if (responseMessage == null)
				{
					// no response ... just failure 
					this.logger.logError("Http request for sensor registration failed ... ");
				}
				else
				{
					// status code is not success
					this.logger.logError("Http response for registration failed: " + responseMessage.ReasonPhrase);
				}
				return false;
			}

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{

			if (this.timer != null)
			{
				this.timer.Stop();
			}

			if (this.httpClient != null)
			{
				this.httpClient.Dispose();
			}

			Console.WriteLine("registrator down ... ");

			return Task.CompletedTask;
		}

	}

}