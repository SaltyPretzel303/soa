using CollectorService.Broker.Events;
using CollectorService.Broker.Reporter.Reports.Collector;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CollectorService.Broker.Reporter
{
	public class ControllerReporter
	{

		private RequestDelegate next;

		public ControllerReporter(RequestDelegate next)
		{

			this.next = next;

		}

		public async Task Invoke(HttpContext context)
		{

			// this method is called on every http request 

			DateTime requestTime = DateTime.Now;

			await next.Invoke(context);

			AccessReport report = new AccessReport(context.Request, context.Response, requestTime);
			CollectorEvent c_event = new CollectorEvent(report);

			// MessageBroker.Instance.publishEvent(c_event);

		}

	}
}