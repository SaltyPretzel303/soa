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
			string prefix = config.samplePrefix;
			string extension = config.sampleExtension;
			FromTo samples_range = config.samplesRange;
			int read_interval = config.readInterval;


			services.AddSingleton(new Reader(config.dataPath, config.samplePrefix, config.sampleExtension, config.samplesRange, config.readInterval));

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
	}
}
