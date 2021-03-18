using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;

namespace CommunicationModel.RestModels
{
	// this is kinda same as UnstableRecord class 
	// but this one is used as a part of "communication protocol"
	public class UnstableServiceRecord
	{
		public DateTime recordedTime { get; set; }

		public string serviceId { get; set; }
		public int downCount { get; set; }

		public List<ServiceLifetimeEvent> downEvents { get; set; }

		public UnstableServiceRecord(string serviceId,
							int downCount,
							List<ServiceLifetimeEvent> downEvents,
							DateTime time)
		{
			this.recordedTime = time;

			this.serviceId = serviceId;
			this.downCount = downCount;
			this.downEvents = downEvents;

		}

	}
}