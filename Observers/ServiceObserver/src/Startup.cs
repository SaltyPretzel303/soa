using System;
using CommunicationModel.BrokerModels;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceObserver.Broker;
using ServiceObserver.MediatrRequests;
using ServiceObserver.Storage;

namespace ServiceObserver
{
	public class Startup
	{

		private IServiceProvider provider;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddTransient<IDatabaseService, MongoStorage>();
			services.AddTransient<IMessageBroker, RabbitMqBroker>();

			services.AddMediatR(typeof(Startup));
			services.AddTransient<RequestHandler<ConfigChangeRequest>, ConfigChangeRequestHandler>();

			services.AddHostedService<BrokerEventReceiver>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
							IWebHostEnvironment env,
							IHostApplicationLifetime lifetime)

		{

			this.provider = app.ApplicationServices;

			lifetime.ApplicationStopping.Register(this.onShutdown);
			lifetime.ApplicationStarted.Register(this.onStartup);

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

		public void onStartup()
		{
			IMessageBroker broker = this.provider.GetService<IMessageBroker>();
			if (broker != null)
			{
				broker.PublishLifetimeEvent(new ServiceLifetimeEvent(LifetimeStages.Startup));
			}
		}

		public void onShutdown()
		{
			IMessageBroker broker = this.provider.GetService<IMessageBroker>();
			if (broker != null)
			{
				broker.PublishLifetimeEvent(new ServiceLifetimeEvent(LifetimeStages.Shutdown));
			}
		}

	}
}
