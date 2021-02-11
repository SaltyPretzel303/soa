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

		// this one is ok to be kept here
		// it is singleton anyway ... 
		private ILogger logger;

		public void ConfigureServices(IServiceCollection services)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			services.AddControllers();

			services.AddSingleton<ILogger, BasicLogger>();
			services.AddSingleton<IDataCacheManager, InMemoryCache>();

			services.AddTransient<IMessageBroker, RabbitMqBroker>();

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

		private void onShutDown()
		{
			this.logger.logMessage("Sensor is going down ... ");

			// result is not used (potential exception is handled inside the method)
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

			Console.WriteLine("Unregister request sent ... ");
			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result; // .Result is going to force blocking execution
			Console.WriteLine("Sensor successfully unregistered ...  ");

			bool retValue = false;
			if (responseMessage != null
				&& responseMessage.IsSuccessStatusCode)
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
