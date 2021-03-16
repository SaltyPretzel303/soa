using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;

namespace ServiceObserver.RuleEngine
{
	public class UnstableRecord
	{

		public DateTime time { get; set; }

		public string serviceId { get; set; }
		public int downCount { get; set; }

		public List<ServiceLifetimeEvent> downEvents;

		public UnstableRecord(string serviceId, int downCount, List<ServiceLifetimeEvent> downEvents)
		{
			this.serviceId = serviceId;
			this.downCount = downCount;
			this.downEvents = downEvents;

			this.time = DateTime.Now;
		}

		public UnstableRecord()
		{
			downEvents = new List<ServiceLifetimeEvent>();
		}
	}
}