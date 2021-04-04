using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CommunicationModel.BrokerModels;
using Microsoft.Extensions.Hosting;
using NRules;
using NRules.Fluent;
using ServiceObserver.Configuration;
using ServiceObserver.Data;

namespace ServiceObserver.RuleEngine
{
	public class PeriodicRuleEngine : IHostedService, IReloadable
	{

		private System.Timers.Timer timer;

		private ISession engineSession;

		private IEventsCache eventsCache;

		private IServiceProvider serviceProvider;

		private ConfigFields config;

		public PeriodicRuleEngine(IEventsCache eventsCache,
							IServiceProvider provider)
		{
			this.eventsCache = eventsCache;
			this.serviceProvider = provider;

			this.config = ServiceConfiguration.Instance;

			if (this.serviceProvider == null)
			{
				Console.WriteLine("Provider is null ... :(");
			}

		}

		public Task StartAsync(CancellationToken cancellationToken)
		{

			ServiceConfiguration.subscribeForReload((IReloadable)this);

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
			List<ServiceEvent> cacheContent = eventsCache.GetEvents();

			foreach (ServiceEvent singleEvent in cacheContent)
			{
				engineSession.Insert(singleEvent);
			}

			if (cacheContent.Count > 0)
			{
				Console.WriteLine($"Rule engine started with {cacheContent.Count} new items ... ");
			}
			// else // handy in development 
			// {
			// 	Console.WriteLine(". ");
			// }

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

		public void reload(ConfigFields newConfig)
		{

			if (timer != null)
			{
				timer.Stop();
			}

			// currently there is no point in recreating ruleEngine
			// it doesn't depend on current configuration
			// rules can't be changed (each rule is a single class)
			// setupRuleEngine();

			this.config = newConfig;

			timer = new System.Timers.Timer();
			timer.Interval = config.ruleEngineTriggerInterval;
			timer.Elapsed += this.timerEvent;
			timer.AutoReset = true;

			timer.Start();

			Console.WriteLine("PeriodicRuleEngine reloaded using new config ... ");

		}
	}
}