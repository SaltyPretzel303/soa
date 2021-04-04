using System;
using System.Collections.Generic;
using System.Linq;
using CommunicationModel.BrokerModels;
using MediatR;
using NRules.Fluent.Dsl;
using ServiceObserver.Configuration;
using ServiceObserver.MediatrRequests;

namespace ServiceObserver.RuleEngine
{
	public class UnstableServiceRule : Rule
	{

		public override void Define()
		{
			ServiceLifetimeEvent singleEvent = null;
			IEnumerable<ServiceLifetimeEvent> oldDownEvents = null;
			IEnumerable<UnstableRuleRecord> oldDownRecords = null;

			IMediator mediator = null;

			Dependency()
				.Resolve(() => mediator);

			When()
				.Match<ServiceLifetimeEvent>(
					() => singleEvent,
					e => e.lifeStage == LifetimeStages.Shutdown
				)
				.Query(
					() => oldDownEvents,
					eventItem => eventItem.Match<ServiceLifetimeEvent>(
						e => e.lifeStage == LifetimeStages.Shutdown,
						e => e.sourceId == singleEvent.sourceId
					// e => e != singleEvent
					)
					.Collect()
				)
				.Query(
					() => oldDownRecords,
					recordItem => recordItem.Match<UnstableRuleRecord>(
						r => r.serviceId == singleEvent.sourceId
					)
					.Collect()
				);

			Then()
				.Do(ctx => PrintSingleDownEvent(singleEvent))
				.Do(ctx => ProcessAllDownEvents(oldDownEvents))

				.Do(ctx => ProcessUnstableRecords(oldDownRecords,
											oldDownEvents,
											ctx,
											mediator));
		}


		private static void PrintSingleDownEvent(ServiceLifetimeEvent singleEvent)
		{
			if (singleEvent == null)
			{
				Console.WriteLine("We got null as a SingleEvent ... ");
				return;
			}

			Console.WriteLine("\t" + IdTimeFormat(singleEvent));
		}

		private static void ProcessAllDownEvents(IEnumerable<ServiceLifetimeEvent> eventsList)
		{
			if (eventsList == null || eventsList.Count() == 0)
			{
				Console.WriteLine("-");
				return;
			}

			foreach (ServiceLifetimeEvent eventItem in eventsList)
			{
				Console.WriteLine(IdTimeFormat(eventItem));
			}

		}

		private static void ProcessUnstableRecords(IEnumerable<UnstableRuleRecord> oldRecords,
										IEnumerable<ServiceLifetimeEvent> oldEvents,
										NRules.RuleModel.IContext ctx,
										IMediator mediator)
		{
			if (oldRecords == null)
			{
				Console.WriteLine("We got null for ListRecords ..");
				return;
			}

			if (oldRecords.Count() > 0)
			{
				if (oldRecords.First().downCount < oldEvents.Count())
				{
					ctx.Retract(oldRecords.First());
				}
				else
				{
					Console.WriteLine("Record is already up to date ... ");
					return;
				}
			}

			UnstableRuleRecord newRecord = new UnstableRuleRecord(
													oldEvents.First().sourceId,
													oldEvents.Count(),
													oldEvents.ToList(),
													DateTime.Now);
			ctx.Insert(newRecord);
			Console.WriteLine("\tRecord update: "
							+ $"s.ID:{newRecord.serviceId} been down "
							+ $"{newRecord.downCount}x ... ");


			ConfigFields config = ServiceConfiguration.Instance;
			if (newRecord.downCount >= config.unstableRecordsLimit)
			{
				mediator.Send(new UnstableServiceRequest(newRecord));
			}

			return;
		}

		private static void PrintSeparator()
		{
			Console.WriteLine("===========================================");
		}

		private static string IdTimeFormat(ServiceLifetimeEvent newEvent)
		{
			return String.Format("ID:{0} -> TIME: {1}:{2}:{3}",
							newEvent.sourceId,
							newEvent.time.Hour,
							newEvent.time.Minute,
							newEvent.time.Second);
		}

	}
}