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
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddMvc();

			ServiceConfiguration config = ServiceConfiguration.read();

			string path = config.dataPath;
			string prefix = config.dataPrefix;
			string extension = config.sampleExtension;
			FromTo samples_range = config.samplesRange;
			int read_interval = config.readInterval;

			services.AddSingleton(new Reader(config.dataPath, config.dataPrefix, config.sampleExtension, config.samplesRange, config.readInterval));

			this.registerSensor();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();

		}

		private void registerSensor()
		{

			ServiceConfiguration conf = ServiceConfiguration.read();

			// e.g. http://localhost/sensor/registry/registerSensor?sensorName="sensor_1"&portNum=5050
			string addr = $"http://{conf.registryAddress}:{conf.registryPort}/{conf.registerSensorPath}?{conf.sensorNameField}={conf.sensorName}&{conf.portNumField}={conf.listeningPort}";

			HttpClient httpClient = new HttpClient();
			HttpResponseMessage responseMessage = httpClient.GetAsync(addr).Result;

			if (responseMessage.IsSuccessStatusCode)
			{
				Console.WriteLine("Sensor successful registered ... ");
			}
			else
			{
				Console.WriteLine("Error in sensor registration ... ");
				Console.WriteLine(responseMessage.Content.ToString());
			}

		}

	}
}
