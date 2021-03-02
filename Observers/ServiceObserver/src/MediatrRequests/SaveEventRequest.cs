using System;
using CommunicationModel.BrokerModels;
using MediatR;
using Newtonsoft.Json;
using ServiceObserver.Data;

namespace ServiceObserver.MediatrRequests
{

	public class SaveEventRequest : IRequest
	{
		public ServiceEvent newEvent;

		public SaveEventRequest(ServiceEvent newEvent)
		{
			this.newEvent = newEvent;
		}
	}

	public class SaveEventRequestHandler : RequestHandler<SaveEventRequest>
	{

		private IEventsCache eventsCache;

		public SaveEventRequestHandler(IEventsCache eventsCache)
		{
			this.eventsCache = eventsCache;
		}

		protected override void Handle(SaveEventRequest request)
		{
			string txtContent = JsonConvert.SerializeObject(request.newEvent);

			this.eventsCache.SaveEvent(request.newEvent);

		}
	}

}