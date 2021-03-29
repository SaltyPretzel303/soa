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
using CommunicationModel.BrokerModels;
using MediatR;
using SensorService.MediatorRequests;

namespace SensorService
{
	public class Startup
	{

		private IServiceProvider serviceProvider;

		public void ConfigureServices(IServiceCollection services)
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;

			services.AddControllers();

			services.AddSingleton<ILogger, BasicLogger>();
			services.AddSingleton<IDataCacheManager, InMemoryCache>();

			services.AddTransient<IMessageBroker, RabbitMqBroker>();

			services.AddMediatR(typeof(Startup));

			services.AddTransient<RequestHandler<NewDataReadRequest>,
						NewDataReadRequestHandler>();

			// sensor is gonna be registered once registry recives SensorLifetimeEvent
			// or any other sensorReader update
			// trying to register sensor using restApi is not necessary
			// services.AddHostedService<RegistrationService>();

			services.AddHostedService<SensorReader>();
		}

		public void Configure(IApplicationBuilder app,
							IWebHostEnvironment env,
							IHostApplicationLifetime lifetime)
		{

			serviceProvider = app.ApplicationServices;

			lifetime.ApplicationStarted.Register(this.onStarted);
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

		private void onStarted()
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMessageBroker broker = serviceProvider.GetService<IMessageBroker>();

			for (int sensorIndex = config.sensorsRange.From;
					sensorIndex < config.sensorsRange.To;
					sensorIndex++)
			{
				string sensorName = config.sensorNamePrefix + sensorIndex;
				var newEvent = new SensorLifetimeEvent(LifetimeStages.Startup,
											sensorName,
											config.hostIP,
											config.listeningPort,
											0);

				broker.PublishLifetimeEvent(newEvent);
			}

		}

		private void onShutDown()
		{
			this.serviceProvider.GetService<ILogger>()
								?.logMessage("Sensor is going down ... ");

			ServiceConfiguration config = ServiceConfiguration.Instance;
			IMessageBroker broker = serviceProvider.GetService<IMessageBroker>();
			IDataCacheManager cache = serviceProvider.GetService<IDataCacheManager>();

			for (int sensorIndex = config.sensorsRange.From;
					sensorIndex < config.sensorsRange.To;
					sensorIndex++)
			{
				string sensorName = config.sensorNamePrefix + sensorIndex;
				var newEvent = new SensorLifetimeEvent(LifetimeStages.Shutdown,
											sensorName,
											config.hostIP,
											config.listeningPort,
											cache.GetLastReadIndex(sensorName));

				broker.PublishLifetimeEvent(newEvent);
			}


			// next line will unregister this sensor using sensorRegistry restAPI
			// it is not necessary because sensor is gonna be unregistered once 
			// registry receives it's lifetime event with shutdown 'flag'

			// result is not used (potential exception is handled inside the method)
			// bool unregResult = this.unregisterSensors();
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
					this.serviceProvider.GetService<ILogger>()
						?.logError($"EXCEPTION in sensor: {sensorName} unregistration: {e.Message}\n");
				}

			}

			return true;
		}

		// NOTE throws socket exception
		private bool unregisterSensor(string sensorName)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			// e.g. http://localhost/sensor/registry/unregisterSensor?sensorName="sensor_1"
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/"
								+ $"{conf.unregisterSensorPath}?"
								+ $"{conf.sensorNameField}={sensorName}";

			HttpClient httpClient = new HttpClient();

			HttpResponseMessage responseMessage = httpClient.DeleteAsync(addr).Result;
			// .Result is going to force blocking/sync execution	

			bool retValue = false;
			if (responseMessage != null
				&& responseMessage.IsSuccessStatusCode)
			{
				Console.WriteLine("Sensor successfully unregistered ...  ");
				retValue = true;
			}
			else
			{
				retValue = false;

				if (responseMessage == null)
				{
					this.serviceProvider
						.GetService<ILogger>()
						?.logError("Http request for sensor unregister failed ... ");
				}
				else
				{
					// status code is not success
					this.serviceProvider
						.GetService<ILogger>()
						?.logError("Http response for sensorUnregister filed: "
							+ responseMessage.ReasonPhrase);
				}

			}

			return retValue;
		}

	}
}
