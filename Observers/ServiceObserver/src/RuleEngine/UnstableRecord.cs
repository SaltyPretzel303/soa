using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;

namespace ServiceObserver.RuleEngine
{
	public class UnstableRecord
	{
		public string serviceId { get; set; }
		public int downCount { get; set; }

		public List<ServiceLifetimeEvent> downEvents;

		public UnstableRecord(string serviceId, int downCount)
		{
			this.serviceId = serviceId;
			this.downCount = downCount;
			downEvents = new List<ServiceLifetimeEvent>();
		}

		public UnstableRecord()
		{
			downEvents = new List<ServiceLifetimeEvent>();
		}
	}
}