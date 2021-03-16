using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;

namespace ServiceObserver.Data
{
	public class InMemoryEventsCache : IEventsCache
	{

		private static List<ServiceEvent> Cache;

		private static object rwLock;

		public InMemoryEventsCache()
		{
			if (Cache == null)
			{
				Cache = new List<ServiceEvent>();
			}

			if (rwLock == null)
			{
				rwLock = new object();
			}
		}

		public void SaveEvent(ServiceEvent newEvent)
		{
			lock (rwLock)
			{
				Cache.Add(newEvent);
			}
		}

		public List<ServiceEvent> GetEvents()
		{
			lock (rwLock)
			{
				List<ServiceEvent> retList = new List<ServiceEvent>(Cache);
				Cache.Clear();

				return retList;
			}
		}


	}
}