using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ServiceObserver.Broker;
using ServiceObserver.Report;
using ServiceObserver.Report.Processor;

namespace ServiceObserver
{
	public class Startup
	{



		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddMvc();

			ReportProcessor reportProcessor = new RuleBasedProcessor();

			MessageBroker.Instance.subscribeForReports(reportProcessor);

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
		{

			lifetime.ApplicationStopping.Register(this.onShutdown);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();

		}

		public void onShutdown()
		{

			MessageBroker.Instance.shutDown();

		}

	}
}
