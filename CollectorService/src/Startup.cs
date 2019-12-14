using CollectorService.Configuration;
using CollectorService.Data;
using CollectorService.Broker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CollectorService.Broker.Reporter;
using System;
using CollectorService.Broker.Events;
using Newtonsoft.Json.Linq;
using CollectorService.Data.Registry;
using CollectorService.Broker.Reporter.Reports.Collector;

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

			services.AddTransient<IDatabaseService, MongoDatabaseService>();

			ServiceConfiguration conf = ServiceConfiguration.Instance;
			IDatabaseService database = new MongoDatabaseService();
			// data puller is started inside constructor
			Console.WriteLine("Read interval: " + conf.readInterval);
			this.data_puller = new DataPuller(database, MessageBroker.Instance, conf.readInterval, conf.dataRangeUrl, conf.headerUrl);

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
		{

			lifetime.ApplicationStopping.Register(this.onShutDown);
			lifetime.ApplicationStarted.Register(this.onStartup);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseMiddleware<ControllerReporter>();

			app.UseMvc();


		}

		private void onStartup()
		{

			MessageBroker.Instance.publishEvent(new CollectorEvent(new LifeCycleReport("startup")));
			MessageBroker.Instance.subscribeForConfiguration(this.handleNewConfiguration);

		}

		private void onShutDown()
		{

			Console.Write("Handling shutdown ... ");

			MessageBroker.Instance.publishEvent(new CollectorEvent(new LifeCycleReport("shutdown")));

			MessageBroker.Instance.shutDown();

		}

		// TODO refactor configuration changing
		// new configuration is accpeted trough message broker
		// much better option may be simple rest request
		private void handleNewConfiguration(JObject newConfig)
		{

			Console.WriteLine("Handling new configuration ... \n\n");

			// TODO database is coupled with the single implementation
			// extract database from service provider
			// or create database abstarct factory 
			ServiceConfiguration.reload(newConfig, new MongoDatabaseService());

		}

	}
}
