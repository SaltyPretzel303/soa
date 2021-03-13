using System;
using System.Collections.Generic;
using System.Linq;
using CommunicationModel.BrokerModels;
using MediatR;
using Newtonsoft.Json;
using NRules.Fluent.Dsl;
using ServiceObserver.Configuration;
using ServiceObserver.MediatrRequests;

namespace ServiceObserver.RuleEngine
{
	public class UnstableServiceRule : Rule
	{

		// original template
		// When()
		// 	.Match<ServiceLifetimeEvent>(
		// 		() => newEvent,
		// 		e => e.lifeStage == LifetimeStages.Shutdown)
		// 	.Query(
		// 		() => oldEvents,
		// 		singleEvent => singleEvent.Match<ServiceLifetimeEvent>(
		// 			e => e.lifeStage == LifetimeStages.Shutdown,
		// 			e => e.source == newEvent.source,
		// 			e => e != newEvent
		// 		)
		// 		.Collect()
		// 	);

		// Then()
		// 	.Do(ctx => MultiPrint(newEvent, oldEvents));

		public override void Define()
		{
			ServiceLifetimeEvent singleEvent = null;
			IEnumerable<ServiceLifetimeEvent> listEvents = null;
			IEnumerable<UnstableRecord> listRecords = null;

			IMediator mediator = null;

			Dependency()
				.Resolve(() => mediator);

			When()
				.Match<ServiceLifetimeEvent>(
					() => singleEvent,
					e => e.lifeStage == LifetimeStages.Shutdown
				)
				.Query(
					() => listEvents,
					eventItem => eventItem.Match<ServiceLifetimeEvent>(
						e => e.source == singleEvent.source,
						e => e.lifeStage == LifetimeStages.Shutdown
					// e => e != singleEvent
					)
					.Collect()
				)
				.Query(
					() => listRecords,
					recordItem => recordItem.Match<UnstableRecord>(
						r => r.serviceId == singleEvent.source
					)
					.Collect()
				);

			Then()
				.Do(ctx => PrintSingleDownEvent(singleEvent))
				.Do(ctx => ProcessAllDownEvents(listEvents))

				.Do(ctx => ProcessUnstableRecords(listRecords,
											listEvents,
											ctx,
											mediator));

			// .Do(ctx => PrintSeparator());

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

		private static void ProcessUnstableRecords(IEnumerable<UnstableRecord> recordsList,
										IEnumerable<ServiceLifetimeEvent> oldEvents,
										NRules.RuleModel.IContext ctx,
										IMediator mediator)
		{
			if (recordsList == null)
			{
				Console.WriteLine("We got null for ListRecords ..");
				return;
			}

			if (recordsList.Count() > 0)
			{
				if (recordsList.First().downCount < oldEvents.Count())
				{
					ctx.Retract(recordsList.First());
				}
				else
				{
					Console.WriteLine("Record is already updated ... ");
					return;
				}
			}

			UnstableRecord record = new UnstableRecord(oldEvents.First().source,
													oldEvents.Count());
			ctx.Insert(record);
			Console.WriteLine("\tRecord update: "
							+ $"s.ID:{record.serviceId} been down "
							+ $"{record.downCount}x ... ");


			ServiceConfiguration config = ServiceConfiguration.Instance;
			if (record.downCount >= config.unstableRecordsLimit)
			{
				mediator.Send(new UnstableServiceRequest(record));
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
							newEvent.source,
							newEvent.time.Hour,
							newEvent.time.Minute,
							newEvent.time.Second);
		}

	}
}