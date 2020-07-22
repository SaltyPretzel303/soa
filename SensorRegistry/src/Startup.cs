using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SensorRegistry.Broker;
using SensorRegistry.Registry;

namespace SensorRegistry
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddControllers();

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<ISensorRegistry>(new MapRegistry());

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
							IWebHostEnvironment env,
							IHostApplicationLifetime lifetime)
		{

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


		private void onShutDown()
		{

			Console.WriteLine("Handling shutdown ... ");
			MessageBroker.Instance.shutDown();

		}

		private void onStart()
		{

			Console.WriteLine("Handling startup ... ");

		}

	}
}
