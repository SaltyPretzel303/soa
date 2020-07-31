using CommunicationModel.BrokerModels;
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

		public async Task Invoke(HttpContext context, IMessageBroker messageBroker)
		{

			// this method is called on every http request 

			DateTime requestTime = DateTime.Now;

			await next.Invoke(context);

			CollectorAccessEvent newEvent = new CollectorAccessEvent(context.Request.Method,
																context.Request.Path,
																context.Request.QueryString.ToString(),
																context.Connection.RemoteIpAddress.ToString(),
																context.Connection.RemotePort,
																requestTime,
																context.Response.StatusCode,
																context.Response.ContentType,
																context.Response.ContentLength);

			messageBroker.PublishCollectorAccessEvent(newEvent);

		}

	}
}
