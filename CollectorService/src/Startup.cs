using CollectorService.Configuration;
using CollectorService.Data;
using CollectorService.Broker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CollectorService.Broker.Reporter;
using CollectorService.Data.Registry;
using Microsoft.Extensions.Hosting;
using CommunicationModel.BrokerModels;
using MediatR;
using CollectorService.MediatrRequests;
using System.Collections.Generic;
using CommunicationModel;

namespace CollectorService
{
	public class Startup
	{
		private IMessageBroker BrokerInstance;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddTransient<IRegistryCache, InMemoryRegistryCache>();
			services.AddTransient<IDatabaseService, MongoDatabaseService>();

			services.AddMediatR(typeof(Startup));

			// next services will be used by the mediatr
			services.AddTransient<RequestHandler<ConfigChangeRequest>, ConfigChangeRequestHandler>();
			services.AddTransient<RequestHandler<SensorRegistryUpdateRequest>, SensorRegistryUpdateRequestHandler>();

			services.AddTransient<RequestHandler<GetRecordsCountRequest, int>, GetRecordsCountRequestHandler>();
			services.AddTransient<RequestHandler<AddRecordsToSensorRequest>, AddRecordsToSensorRequestHandler>();

			services.AddTransient<RequestHandler<GetAllSensorsRequest, List<SensorRegistryRecord>>, GetAllSensorsRequestHandler>();
			services.AddTransient<RequestHandler<PublishCollectorPullEventRequest>, PublishCollectorPullEventRequestHandler>();

			services.AddTransient<IMessageBroker, RabbitMqBroker>();

			services.AddHostedService<DataPuller>();
			services.AddHostedService<BrokerEventReceiver>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
							IWebHostEnvironment env,
							IHostApplicationLifetime lifetime,
							IDatabaseService database,
							IMessageBroker messageBroker)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			// reference used in methods from this class
			this.BrokerInstance = messageBroker;

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

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

		}

		private void onStartup()
		{
			this.BrokerInstance.PublishLifetimeEvent(new ServiceLifetimeEvent(LifetimeStages.Startup));
		}

		private void onShutDown()
		{
			this.BrokerInstance.PublishLifetimeEvent(new ServiceLifetimeEvent(LifetimeStages.Shutdown));
		}

	}
}
