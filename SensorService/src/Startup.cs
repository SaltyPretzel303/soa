using System.Net.Sockets;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SensorService.Configuration;
using SensorService.Data;

namespace SensorService
{
	public class Startup
	{

		private string interruptMessage;
		private bool servicesConfigured;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			servicesConfigured = false;

			services.AddMvc();

			ServiceConfiguration config = ServiceConfiguration.read();

			string path = config.dataPath;
			string prefix = config.dataPrefix;
			string extension = config.sampleExtension;
			FromTo samples_range = config.samplesRange;
			int read_interval = config.readInterval;

			services.AddSingleton(new Reader(config.dataPath, config.dataPrefix, config.sampleExtension, config.samplesRange, config.readInterval));


			try
			{

				this.registerSensor();
				servicesConfigured = true;
				Console.WriteLine("Sensor successful registered ... ");

			}
			catch (AggregateException e)
			{

				this.servicesConfigured = false;

				e.Handle((exc) =>
				{

					if (exc is SocketException || exc is HttpRequestException)
					{

						this.interruptMessage = String.Format($"Registry service not found\nException: {e.ToString()}\n");


						return true;
					}

					return false;

				});

			}
			catch (Exception e)
			{
				this.servicesConfigured = false;
				Console.WriteLine("Unknown exception: " + e.ToString());
			}

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
		{

			lifetime.ApplicationStopping.Register(this.onShutDown);

			if (!this.servicesConfigured)
			{

				Console.WriteLine(this.interruptMessage);
				lifetime.StopApplication();

			}

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();

		}

		// maybe http exception could be handled inside this method and not outside of it (current implementation) ... 
		private void registerSensor()
		{

			ServiceConfiguration conf = ServiceConfiguration.read();

			// e.g. http://localhost/sensor/registry/registerSensor?sensorName="sensor_1"&portNum=5050
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.registerSensorPath}?{conf.sensorNameField}={conf.sensorName}&{conf.portNumField}={conf.listeningPort}";

			Console.WriteLine("Going to register service on address: " + addr);

			HttpClient httpClient = new HttpClient();
			// this method could possibly throw exception 
			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result; // .Result is going to force blocking execution

			if (responseMessage.IsSuccessStatusCode)
			{

				this.servicesConfigured = true;

			}
			else
			{

				this.servicesConfigured = false;
				this.interruptMessage = responseMessage.Content.ToString();

			}

		}

		private void onShutDown()
		{

			Console.WriteLine("Senor is going down ... ");

			this.unregisterSensor();


		}

		private void unregisterSensor()
		{
			ServiceConfiguration conf = ServiceConfiguration.read();

			// e.g. http://localhost/sensor/registry/unregisterSensor?sensorName="sensor_1"
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.unregisterSensorPath}?{conf.sensorNameField}={conf.sensorName}";

			HttpClient httpClient = new HttpClient();
			// this method could possibly throw exception 
			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result; // .Result is going to force blocking execution

			// TODO 
			// no point to handler unregistartion failure, this sensor is going down any way ... dont hold it back ... 

			if (responseMessage.IsSuccessStatusCode)
			{

				this.servicesConfigured = true;

			}
			else
			{

				this.servicesConfigured = false;
				this.interruptMessage = responseMessage.Content.ToString();

			}
		}

	}

}
