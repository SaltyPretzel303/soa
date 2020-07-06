using System.Net.Sockets;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SensorService.Configuration;
using SensorService.Data;
using SensorService.Logger;
using Microsoft.Extensions.Hosting;

namespace SensorService
{
	public class Startup
	{

		private ILogger logger;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddMvc();

			ServiceConfiguration config = ServiceConfiguration.read();

			if (config.test_list != null)
			{
				foreach (string item in config.test_list)
				{
					Console.WriteLine(item);
				}
			}

			string path = config.dataPath;
			string prefix = config.dataPrefix;
			string extension = config.sampleExtension;
			FromTo samples_range = config.samplesRange;
			int read_interval = config.readInterval;

			Reader sensorReader = new Reader(config.dataPath, config.dataPrefix, config.sampleExtension, config.samplesRange, config.readInterval, logger);
			services.AddSingleton(sensorReader);

			services.AddTransient<ILogger, BasicLogger>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
		{

			lifetime.ApplicationStopping.Register(this.onShutDown);

			this.logger = new BasicLogger(env);

			try
			{

				bool regResult = this.registerSensor();

				if (regResult)
				{
					// TODO maybe try to register in another sensor registry (or that should be done in register method)
					this.logger.logMessage("Sensor successfully registered ... ");
					// Console.WriteLine("Sensor successfully registered ... ");

				}
				else
				{
					// this.logger.logError("Sensor failed to register ")
					lifetime.StopApplication();
					return;
				}

			}
			catch (Exception e)
			{

				this.logger.logError("Exception in sensor registration: " + e.ToString());
				// Console.WriteLine("Unknown exception: " + e.ToString());

				lifetime.StopApplication();
				return;

			}


			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();

		}

		// NOTE throws socket exception
		private bool registerSensor()
		{

			ServiceConfiguration conf = ServiceConfiguration.read();

			// e.g. http://localhost/sensor/registry/registerSensor?sensorName="sensor_1"&portNum=5050
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.registerSensorPath}?{conf.sensorNameField}={conf.sensorName}&{conf.portNumField}={conf.listeningPort}";

			this.logger.logMessage("Going to register service on address: " + addr);
			// Console.WriteLine("Going to register service on address: " + addr);

			HttpClient httpClient = new HttpClient();
			// this method could possibly throw exception 
			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result; // .Result is going to force blocking execution

			bool retValue = false;
			if (responseMessage != null &&
				responseMessage.IsSuccessStatusCode)
			{

				return true;

			}
			else
			{

				retValue = false;

				if (responseMessage == null)
				{

					this.logger.logError("Http request for sensor unregister failed ... ");

				}
				else
				{
					// status code is not success

					this.logger.logError("Http response for sensorUnregister filed: " + responseMessage.ReasonPhrase);
				}

			}

			return retValue;

		}

		private void onShutDown()
		{

			this.logger.logMessage("Sensor is going down ... ");
			// Console.WriteLine("Sensor is going down ... ");

			try
			{

				// result is not used
				bool unregResult = this.unregisterSensor();

			}
			catch (Exception e)
			{
				this.logger.logError("Exception in sensor unregistration ... \n");
				this.logger.logError(e.ToString());
			}

		}

		// NOTE throws socket exception
		private bool unregisterSensor()
		{

			ServiceConfiguration conf = ServiceConfiguration.read();

			// e.g. http://localhost/sensor/registry/unregisterSensor?sensorName="sensor_1"
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.unregisterSensorPath}?{conf.sensorNameField}={conf.sensorName}";

			HttpClient httpClient = new HttpClient();

			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result; // .Result is going to force blocking execution

			bool retValue = false;
			if (responseMessage != null && responseMessage.IsSuccessStatusCode)
			{

				retValue = true;

			}
			else
			{

				retValue = false;

				if (responseMessage == null)
				{

					this.logger.logError("Http request for sensor unregister failed ... ");

				}
				else
				{
					// status code is not success

					this.logger.logError("Http response for sensorUnregister filed: " + responseMessage.ReasonPhrase);
				}

			}

			return retValue;


		}

	}

}
