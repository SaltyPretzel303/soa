using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;

namespace ServiceObserver.RuleEngine
{
	public class UnstableRuleRecord
	{

		public DateTime time { get; set; }

		public string serviceId { get; set; }
		public int downCount { get; set; }

		public List<ServiceLifetimeEvent> downEvents;

		public UnstableRuleRecord(string serviceId,
						int downCount,
						List<ServiceLifetimeEvent> downEvents,
						DateTime time)
		{
			this.serviceId = serviceId;
			this.downCount = downCount;
			this.downEvents = downEvents;

			this.time = time;
		}

	}
}