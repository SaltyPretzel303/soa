using System;
using MediatR;
using ServiceObserver.RuleEngine;

namespace ServiceObserver.MediatrRequests
{
	public class UnstableServiceRequest : IRequest
	{

		public UnstableRecord record;

		public UnstableServiceRequest(UnstableRecord record)
		{
			this.record = record;
		}
	}

	public class UnstableServiceRequestHandler :
		RequestHandler<UnstableServiceRequest>
	{
		protected override void Handle(UnstableServiceRequest request)
		{
			Console.WriteLine($"Service {request.record.serviceId} is unstable ... ");
		}
	}

}