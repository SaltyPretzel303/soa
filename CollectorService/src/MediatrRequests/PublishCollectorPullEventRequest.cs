using CollectorService.Broker;
using CommunicationModel.BrokerModels;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class PublishCollectorPullEventRequest : IRequest
	{
		public CollectorPullEvent pullEvent { get; private set; }

		public PublishCollectorPullEventRequest(CollectorPullEvent pullEvent)
		{
			this.pullEvent = pullEvent;
		}
	}

	public class PublishCollectorPullEventRequestHandler : RequestHandler<PublishCollectorPullEventRequest>
	{

		private IMessageBroker broker;

		public PublishCollectorPullEventRequestHandler(IMessageBroker broker)
		{
			this.broker = broker;
		}

		protected override void Handle(PublishCollectorPullEventRequest request)
		{
			this.broker.PublishCollectorPullEvent(request.pullEvent);
		}
	}

}