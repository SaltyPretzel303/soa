using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CollectorService.Broker.Reporter
{
	public class ControllerReporter
	{

		private RequestDelegate next;

		private MessageBroker broker;

		public ControllerReporter(RequestDelegate next)
		{

			this.next = next;

		}

		public async Task Invoke(HttpContext context)
		{

			// this method is called on every http request 

			// AccessReport report = new AccessReport(context.Request, context.Response);
			// CollectorEvent c_event = new CollectorEvent(report.toJson());

			// this.broker.publishEvent(c_event);

			Console.WriteLine("IN MIDDLEWARE \n\n");

			await next.Invoke(context);

		}

	}
}