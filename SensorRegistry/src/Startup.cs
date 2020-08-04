using System;
using CommunicationModel.BrokerModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SensorRegistry.Broker;
using SensorRegistry.Broker.EventHandlers;
using SensorRegistry.Configuration;
using SensorRegistry.Logger;
using SensorRegistry.Registry;

namespace SensorRegistry
{
	public class Startup
	{
		private IMessageBroker brokerInstance;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			// used for accessing address and port of http req. src
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			// services.AddSingleton<ISensorRegistry>(new MapRegistry());
			services.AddSingleton<ISensorRegistry, MapRegistry>();

			services.AddTransient<ILogger, BasicLogger>();

			services.AddTransient<IConfigEventHandler, NewConfigurationHandler>();
			services.AddTransient<ISensorEventHandler, NewSensorEventHandler>();

			services.AddTransient<IMessageBroker, RabbitMqBroker>();
			services.AddHostedService<BrokerEventsReceiver>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
							IWebHostEnvironment env,
							IHostApplicationLifetime lifetime)
		{
			this.brokerInstance = app.ApplicationServices.GetService<IMessageBroker>();

			lifetime.ApplicationStopping.Register(this.onShutDown);
			lifetime.ApplicationStarted.Register(this.onStart);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private void onStart()
		{
			ServiceConfiguration config = ServiceConfiguration.Instance;
			this.brokerInstance.publishLifetimeEvent(new ServiceLifetimeEvent(LifetimeStages.Startup));
		}

		private void onShutDown()
		{
			Console.WriteLine("Handling shutdown ... ");
			ServiceConfiguration config = ServiceConfiguration.Instance;
			this.brokerInstance.publishLifetimeEvent(new ServiceLifetimeEvent(LifetimeStages.Shutdown));

		}

	}

}
