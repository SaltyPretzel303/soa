﻿using System;
using CommunicationModel.BrokerModels;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SensorRegistry.Broker;
using SensorRegistry.Configuration;
using SensorRegistry.Logger;
using SensorRegistry.Registry;

namespace SensorRegistry
{
	public class Startup
	{

		private IServiceProvider provider;

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddTransient<ISensorRegistry, InMemoryRegistry>();

			services.AddTransient<ILogger, BasicLogger>();

			services.AddMediatR(typeof(Startup));

			// services.AddTransient<RequestHandler<ConfigUpdateRequest>,
			// 	ConfigUpdateRequestHandler>();
			// services.AddTransient<RequestHandler<SensorLifetimeRequest>,
			// 	SensorLifetimeRequestHandler>();
			// services.AddTransient<IRequestHandler<SensorUpdateRequest>,
			// 	SensorUpdateRequestHandler>();
			// services.AddTransient<IRequestHandler<CheckSensorInfoRequest>,
			// 	CheckSensorInfoRequestHandler>();

			services.AddTransient<IMessageBroker, RabbitMqBroker>();
			services.AddHostedService<BrokerEventsReceiver>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
			IWebHostEnvironment env,
			IHostApplicationLifetime lifetime)
		{
			this.provider = app.ApplicationServices;

			lifetime.ApplicationStarted.Register(onStart);
			lifetime.ApplicationStopping.Register(onShutDown);

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
			var config = ServiceConfiguration.Instance;
			var broker = this.provider.GetService<IMessageBroker>();

			if (broker != null)
			{
				broker.publishLifetimeEvent(
					new ServiceLifetimeEvent(
						config.serviceId,
						LifetimeStages.Startup,
						ServiceType.SensorRegistry));
			}
		}

		private void onShutDown()
		{
			var config = ServiceConfiguration.Instance;
			var broker = provider.GetService<IMessageBroker>();

			if (broker != null)
			{
				broker.publishLifetimeEvent(
					 new ServiceLifetimeEvent(
						config.serviceId,
						LifetimeStages.Shutdown,
						ServiceType.SensorRegistry));
			}
		}

	}
}
