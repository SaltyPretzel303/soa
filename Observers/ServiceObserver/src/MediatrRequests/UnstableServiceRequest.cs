using System;
using CommunicationModel.BrokerModels;
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
			Console.WriteLine($"Handling {request.record.serviceId} unstable record ... ");

			db.SaveUnstableRecord(request.record);

		}
	}

}