using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServiceObserver.Data
{
	public class UnstableServiceDbRecord
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public DateTime recordedTime { get; set; }

		public string serviceId { get; set; }
		public int downCount { get; set; }

		public List<ServiceLifetimeEvent> downEvents;

		public UnstableServiceDbRecord(string serviceId,
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