using System.Collections.Generic;
using CommunicationModel.BrokerModels;

namespace ServiceObserver.Data
{
	public interface IEventsCache
	{

		void SaveEvent(ServiceEvent newEvent);

		List<ServiceEvent> GetEvents();

	}
}