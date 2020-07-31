using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SensorService.Configuration;
using SensorService.Data;
using SensorService.Logger;
using Microsoft.Extensions.Hosting;
using SensorService.Broker;

namespace SensorService
{
	public class Startup
	{

		private ILogger logger;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			services.AddControllers();

			services.AddSingleton<ILogger, BasicLogger>();
			services.AddSingleton<IDataCacheManager, InMemoryCache>();

			services.AddSingleton<IMessageBroker, RabbitMqBroker>();

			services.AddHostedService<RegistrationService>();
			services.AddHostedService<SensorReader>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
							IWebHostEnvironment env,
							IHostApplicationLifetime lifetime)
		{

			this.logger = app.ApplicationServices.GetService<ILogger>();

			lifetime.ApplicationStopping.Register(this.onShutDown);

			// try
			// {
			// 	bool regResult = this.registerSensors();

			// 	if (regResult)
			// 	{
			// 		this.logger.logMessage("All sensors successfully registered ... ");
			// 	}
			// 	else
			// 	{
			// 		this.logger.logError("Failed to register sensors ... ");

			// 		lifetime.StopApplication();
			// 		return;
			// 	}

			// }
			// catch (Exception e)
			// {

			// 	this.logger.logError($"Exception in sensor registration: {e.Message} ");

			// 	lifetime.StopApplication();
			// 	return;
			// }

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

		}

		// move this code in to the hosted service
		// that way sensor can try to register itself again and again
		// after that it wont depend on soa-registry status (active/inactive)
		private bool registerSensors()
		{
			ServiceConfiguration conf = ServiceConfiguration.Instance;

			for (int sensorIndex = conf.sensorsRange.From;
					sensorIndex < conf.sensorsRange.To;
					sensorIndex++)
			{

				bool singleRegResult = this.registerSensor(conf.sensorNamePrefix + sensorIndex);

				// if some of the registration results are bad
				// unregister all currently registered sensors and return false
				// returning false will result in shutting down this service 
				if (singleRegResult != true)
				{
					for (int unregIndex = sensorIndex - 1;
							unregIndex >= conf.sensorsRange.From;
							unregIndex--)
					{
						this.unregisterSensor(conf.sensorNamePrefix + unregIndex);
					}

					return false;
				}

			}

			return true;
		}

		private bool registerSensor(string sensorName)
		{
			ServiceConfiguration conf = ServiceConfiguration.Instance;

			// e.g. http://localhost/sensor/registry/registerSensor?sensorName="sensor_1"&portNum=5050&lastReadIndex=12
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.registerSensorPath}?{conf.sensorNameField}={sensorName}&{conf.portNumField}={conf.listeningPort}&{conf.readIndexField}=0";
			// port on which this sensor is serving http requests

			this.logger.logMessage($"Going to register sensor: {sensorName} on address: {addr}");
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
					// no response ... just failure 
					this.logger.logError("Http request for sensor registration failed ... ");
				}
				else
				{
					// status code is not success
					this.logger.logError("Http response for registration failed: " + responseMessage.ReasonPhrase);
				}
			}

			return retValue;
		}

		private void onShutDown()
		{

			this.logger.logMessage("Sensor is going down ... ");
			// Console.WriteLine("Sensor is going down ... ");

			// result is not used (potential exception is handled inside method)
			bool unregResult = this.unregisterSensors();

		}

		private bool unregisterSensors()
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			for (int sensorIndex = conf.sensorsRange.From;
					sensorIndex < conf.sensorsRange.To;
					sensorIndex++)
			{

				string sensorName = conf.sensorNamePrefix + sensorIndex;
				try
				{
					unregisterSensor(sensorName);
				}
				catch (Exception e)
				{
					this.logger.logError($"EXCEPTION in sensor: {sensorName} unregistration: {e.Message}\n");
				}

			}

			return true;
		}

		// NOTE throws socket exception
		private bool unregisterSensor(string sensorName)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			// e.g. http://localhost/sensor/registry/unregisterSensor?sensorName="sensor_1"
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.unregisterSensorPath}?{conf.sensorNameField}={sensorName}";

			HttpClient httpClient = new HttpClient();

			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result; // .Result is going to force blocking execution

			bool retValue = false;
			if (responseMessage != null &&
				responseMessage.IsSuccessStatusCode)
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
