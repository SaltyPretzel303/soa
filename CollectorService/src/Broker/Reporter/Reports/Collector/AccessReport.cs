using System.Runtime.CompilerServices;
using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Reporter.Reports.Collector
{
	public class AccessReport : Report
	{

		public string method;
		public string requestPath;
		public JObject query;

		public string sourceAddr;
		public int? sourcePort;

		public TimeSpan responseTime;

		public int statusCode;
		public string repsonseType;
		public long? responseLength;

		public AccessReport(HttpRequest request, HttpResponse response, DateTime requestTime)
		{

			this.query = new JObject();
			foreach (string key in request.Query.Keys)
			{
				query[key] = request.Query[key].ToString();
			}


			this.method = request.Method;

			this.requestPath = request.PathBase + request.Path;

			this.sourceAddr = request.Host.Host;
			this.sourcePort = request.Host.Port;

			this.responseTime = DateTime.Now - requestTime;

			this.statusCode = response.StatusCode;
			this.repsonseType = response.ContentType;
			this.responseLength = response.ContentLength;

		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}

		public string getReportType()
		{
			return "access_report";
		}

	}

}