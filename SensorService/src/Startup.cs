using System.ComponentModel.Design;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DataCollector.Data;
using System.IO;
using SensorService.src;

namespace DataCollector
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddMvc();

			string path = ServiceConfiguration.Instance.configRow("data_path");
			string prefix = ServiceConfiguration.Instance.configRow("sample_prefix");
			string extension = ServiceConfiguration.Instance.configRow("sample_extension");
			int users_count = int.Parse(ServiceConfiguration.Instance.configRow("users_count"));
			int read_interval = int.Parse(ServiceConfiguration.Instance.configRow("read_interval"));


			services.AddSingleton(new Reader(path, prefix, extension, users_count, read_interval));

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
