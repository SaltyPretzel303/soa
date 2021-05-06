using System.Threading;
using System.Threading.Tasks;
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

	public class UnstableServiceRequestHandler
		: IRequestHandler<UnstableServiceRequest, Unit>
	{

		public IDatabaseService db;
		public IMessageBroker broker;

		public UnstableServiceRequestHandler(IDatabaseService db,
			IMessageBroker broker)
		{
			this.db = db;
			this.broker = broker;
		}

		public async Task<Unit> Handle(UnstableServiceRequest request,
			CancellationToken token)
		{
			await db.SaveUnstableRecord(request.record);
			// TODO maybe also publish it on the broker

			return Unit.Value;
		}
	}

}