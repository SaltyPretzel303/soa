using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;

namespace ServiceObserver.Data
{
	public class InMemoryEventsCache : IEventsCache
	{

		private static List<string> Cache { get; set; }

		private static object rwLock;

		public InMemoryEventsCache()
		{
			if (Cache == null)
			{
				Cache = new List<string>();
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
				string txtEvent = JsonConvert.SerializeObject(newEvent);
				Cache.Add(txtEvent);
			}
		}

		public List<string> GetEvents()
		{
			lock (rwLock)
			{
				List<string> retList = new List<string>(Cache);
				Cache.Clear();

				return retList;
			}
		}


	}
}