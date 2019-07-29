using CollectorService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SensorService.Configuration;

namespace CollectorService
{
	public class Startup
	{

		private DataPuller data_puller;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddMvc();

			services.AddTransient<IDatabaseService, DatabaseService>();

			ServiceConfiguration conf = ServiceConfiguration.read();

			// this.data_puller = new DataPuller(new DatabaseService(), conf.readInterval, conf.sensorsList, conf.dataRangeUrl, conf.headerUrl);

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseMvc();

		}
	}
}
