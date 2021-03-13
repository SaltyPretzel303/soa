using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CommunicationModel.BrokerModels;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NRules;
using NRules.Fluent;
using ServiceObserver.Configuration;
using ServiceObserver.Data;

namespace ServiceObserver.RuleEngine
{
	public class PeriodicRuleEngine : IHostedService
	{

		private System.Timers.Timer timer;

		private ISession engineSession;

		private IEventsCache eventsCache;

		private IServiceProvider serviceProvider;

		public PeriodicRuleEngine(IEventsCache eventsCache,
							IServiceProvider provider)
		{
			this.eventsCache = eventsCache;
			this.serviceProvider = provider;

			if (this.serviceProvider == null)
			{
				Console.WriteLine("Provider is null ... :(");
			}

		}

		public Task StartAsync(CancellationToken cancellationToken)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;

			setupRuleEngine();

			timer = new System.Timers.Timer();
			timer.Interval = config.ruleEngineTriggerInterval;
			timer.Elapsed += this.timerEvent;
			timer.AutoReset = true;

			timer.Start();

			return Task.CompletedTask;
		}

		private void setupRuleEngine()
		{
			var ruleRepo = new RuleRepository();
			ruleRepo.Load(x => x.From(typeof(PeriodicRuleEngine).Assembly));
			// rules are (should be ?) in the same assembly as PeriodicRuleEngine

			ISessionFactory factory = ruleRepo.Compile();

			engineSession = factory.CreateSession();
			engineSession.DependencyResolver = new AspNetCoreDepResolver(this.serviceProvider);

		}

		private void timerEvent(Object source, ElapsedEventArgs arg)
		{
			List<string> cacheContent = eventsCache.GetEvents();

			foreach (string strItem in cacheContent)
			{
				ServiceLifetimeEvent objItem = JsonConvert.DeserializeObject<ServiceLifetimeEvent>(strItem);
				engineSession.Insert(objItem);
			}

			if (cacheContent.Count > 0)
			{
				Console.WriteLine($"Rule engine started with {cacheContent.Count} new items ... ");
			}
			else
			{
				Console.Write(". ");
			}

			engineSession.Fire();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			if (timer != null)
			{
				timer.Stop();
			}

			return Task.CompletedTask;
		}
	}
}