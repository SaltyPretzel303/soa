using System.Threading;
using System.Threading.Tasks;
using CommunicationModel.BrokerModels;
using MediatR;
using ServiceObserver.Broker;
using ServiceObserver.Configuration;
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

		public ConfigFields config;

		public UnstableServiceRequestHandler(IDatabaseService db,
			IMessageBroker broker)
		{
			this.db = db;
			this.broker = broker;
			this.config = ServiceConfiguration.Instance;
		}

		public async Task<Unit> Handle(
			UnstableServiceRequest request,
			CancellationToken token)
		{
			await db.SaveUnstableRecord(request.record);

			broker.PublishObserverReport(new UnstableServiceReport(
						config.serviceId,
						ServiceObserverResultType.UnstableService,
						request.record.downCount,
						request.record.downEvents),
						config.unstableServiceFilter);

			return Unit.Value;
		}
	}

}