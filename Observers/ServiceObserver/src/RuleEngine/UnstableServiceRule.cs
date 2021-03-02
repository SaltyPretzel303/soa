using System;
using System.Collections.Generic;
using System.Linq;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;
using NRules.Fluent.Dsl;

namespace ServiceObserver.RuleEngine
{
	public class UnstableServiceRule : Rule
	{
		public override void Define()
		{
			ServiceLifetimeEvent newEvent = null;
			IEnumerable<ServiceLifetimeEvent> oldEvents = null;

			When()
				.Match<ServiceLifetimeEvent>(
					() => newEvent,
					e => e.lifeStage == LifetimeStages.Shutdown)
				.Query(
					() => oldEvents,
					singleEvent => singleEvent.Match<ServiceLifetimeEvent>(
						e => e.lifeStage == LifetimeStages.Shutdown,
						e => e.source == newEvent.source,
						e => e != newEvent
					)
					.Collect()
				// .Where(e => e.Any())
				);

			Then()
				.Do(ctx => PrintDeadService(newEvent))
				.Do(ctx => PrintStatistics(oldEvents));

		}

		private static void PrintDeadService(ServiceLifetimeEvent foundEvent)
		{
			string jsonStr = JsonConvert.SerializeObject(foundEvent);
			Console.WriteLine($"This service should be dead: {foundEvent.source} -> {foundEvent.time.Hour}:{foundEvent.time.Minute}:{foundEvent.time.Second}");
		}

		private static void PrintStatistics(IEnumerable<ServiceLifetimeEvent> oldEvents)
		{
			if (oldEvents != null)
			{
				Console.WriteLine($"Found {oldEvents.Count<ServiceLifetimeEvent>()} death cases ... ");
				foreach (ServiceLifetimeEvent singleEvent in oldEvents)
				{
					Console.Write(singleEvent.source + " + ");
				}
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine("No similar old cases ... ");
			}
		}

	}
}