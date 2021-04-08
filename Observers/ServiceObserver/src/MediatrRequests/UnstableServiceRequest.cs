using MediatR;
using ServiceObserver.Broker;
using ServiceObserver.Data;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.MediatrRequests
{
	public class UnstableServiceRequest : IRequest
	{
		public UnstableRuleRecord record;

		public UnstableServiceRequest(UnstableRuleRecord record)
		{
			this.record = record;
		}
	}

	public class UnstableServiceRequestHandler :
			RequestHandler<UnstableServiceRequest>
	{

		public IDatabaseService db;
		public IMessageBroker broker;

		public UnstableServiceRequestHandler(IDatabaseService db,
									IMessageBroker broker)
		{
			this.db = db;
			this.broker = broker;
		}

		protected override void Handle(UnstableServiceRequest request)
		{
			db.SaveUnstableRecord(request.record);
			// TODO maybe also publish it on the broker
		}
	}

}