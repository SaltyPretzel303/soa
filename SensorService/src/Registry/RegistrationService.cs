using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CommunicationModel.RestModels;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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

			this.TryToRegister(null, null);

			if (this.waitingSensors.Count > 0)
			{
				this.timer = new System.Timers.Timer();
				this.timer.Elapsed += this.TryToRegister;
				this.timer.Interval = conf.registerSensorDelay;
				this.timer.AutoReset = true;
				this.timer.Start();
			}

			return Task.CompletedTask;
		}

		// this will be executed repeatedly until all of the "sensors" are registered
		private void TryToRegister(Object source, ElapsedEventArgs args)
		{

			bool regResult = false;
			for (int i = 0; i < waitingSensors.Count; i++)
			{
				string currentSensor = waitingSensors[i];

				regResult = this.registerSensor(currentSensor);

				if (regResult == true)
				{
					// sensor successfully registered
					this.logger.logMessage($"Sensor {currentSensor} successfully registered ... ");
					this.waitingSensors.Remove(currentSensor);
					i--;
				}
				else
				{
					this.logger.logError($"Failed to register {currentSensor} ... ");
				}
			}

			if (this.waitingSensors.Count == 0)
			{
				if (this.timer != null)
				{
					this.timer.Stop();
				}
			}

		}

		private bool registerSensor(string sensorName)
		{
			ServiceConfiguration conf = ServiceConfiguration.Instance;

			int lastReadIndex = this.dataCache.GetLastReadIndex(sensorName);

			// e.g. http://localhost/sensor/registry/addSensor
			string postAddr = $"http://{conf.registryAddress}:{conf.registryPort}/"
					+ $"{conf.registerSensorPath}";

			SensorDataArg reqArg = new SensorDataArg(sensorName,
													conf.hostIP,
													conf.listeningPort,
													lastReadIndex);
			string stringReqArg = JsonConvert.SerializeObject(reqArg);
			Console.WriteLine($"Trying to register with: {stringReqArg} on: {postAddr} ... ");

			// this method could possibly throw exception 
			HttpResponseMessage responseMessage = null;
			try
			{

				responseMessage = httpClient
						.PostAsync(postAddr, new StringContent(stringReqArg,
														Encoding.UTF8,
														"application/json"))
						.Result; // .Result is going to force blocking execution
			}
			catch (Exception e)
			{
				this.logger.logError($"Exception in sensor registration  on: {postAddr}, "
						+ $" reason: {e.Message}");
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
					this.logger.logError("Http response for registration failed: "
								+ responseMessage.ReasonPhrase);
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

			Console.WriteLine("Registration service down ... ");

			return Task.CompletedTask;
		}

	}
}