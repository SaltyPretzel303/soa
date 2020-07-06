using System.Net.Sockets;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SensorService.Configuration;
using SensorService.Data;
using SensorService.Logger;

namespace SensorService
{
	public class Startup
	{

		private ILogger logger;

		private IReader reader;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			ServiceConfiguration config = ServiceConfiguration.read();

			services.AddMvc();

			services.AddSingleton<ILogger, BasicLogger>();

			services.AddSingleton<IReader, FileReader>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
		{

			this.logger = app.ApplicationServices.GetService<ILogger>();

			lifetime.ApplicationStopping.Register(this.onShutDown);

			// next line is necessary in order to start reading (create reader once)
			// it is added to the ServiceProvider but is not created at that moment (ConfiguraServices method)
			// another way is to just request it (IReader) as the parameter of this method, service provider will then create it (using passed factory method) and pass it as the method argument
			app.ApplicationServices.GetService<IReader>();


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
			if (responseMessage != null && responseMessage.IsSuccessStatusCode)
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

					this.logger.logError("Http response for sensorUnregister failed: " + responseMessage.ReasonPhrase);
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
