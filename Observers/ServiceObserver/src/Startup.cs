using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceObserver.Broker;
using ServiceObserver.MediatrRequests;
using ServiceObserver.Data;
using ServiceObserver.RuleEngine;

namespace ServiceObserver
{
	public class Startup
	{

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddTransient<IDatabaseService, MongoStorage>();
			services.AddTransient<IMessageBroker, RabbitMqBroker>();

			services.AddTransient<IEventsCache, InMemoryEventsCache>();

			services.AddMediatR(typeof(Startup));

			// services.AddTransient<RequestHandler<ConfigChangeRequest>,
			// 					 ConfigChangeRequestHandler>();

			// services.AddTransient<RequestHandler<SaveEventRequest>,
			//  					SaveEventRequestHandler>();

			// services.AddTransient<RequestHandler<UnstableServiceRequest>,
			// 						UnstableServiceRequestHandler > ();

			services.AddHostedService<BrokerEventReceiver>();
			services.AddHostedService<PeriodicRuleEngine>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
							IWebHostEnvironment env,
							IHostApplicationLifetime lifetime)
		{

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseEndpoints((endpoints) =>
			{
				endpoints.MapControllers();
			});

		}

	}
}
