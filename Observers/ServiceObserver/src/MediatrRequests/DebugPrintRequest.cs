using System;
using MediatR;

// TODO remove at some point 
// used just for testing purpose 
namespace ServiceObserver.MediatrRequests
{
	public class DebugPrintRequest : IRequest
	{
		public string Message { get; set; }

		public DebugPrintRequest(string message)
		{
			Message = message;
		}
	}

	public class DebugPrintRequestHandler : RequestHandler<DebugPrintRequest>
	{
		protected override void Handle(DebugPrintRequest request)
		{
			Console.WriteLine($"SUCCESS ... {request.Message} ");
		}
	}

}