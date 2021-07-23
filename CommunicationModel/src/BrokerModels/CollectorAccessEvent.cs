using System;

namespace CommunicationModel.BrokerModels
{
	public class CollectorAccessEvent : ServiceEvent
	{
		public string method { get; set; }
		public string requestPath { get; set; }
		public string query { get; set; }

		public string sourceAddr { get; set; }
		public int? sourcePort { get; set; }

		public DateTime requestReceivedTime { get; set; }
		public DateTime responseSendTime { get; set; }

		public int statusCode { get; set; }
		public string responseType { get; set; }
		public long? responseLength { get; set; }

		public CollectorAccessEvent(
			string serviceId,
			string method,
			string requestPath,
			string query,
			string sourceAddr,
			int? sourcePort,
			DateTime requestReceivedTime,
			int statusCode,
			string responseType,
			long? responseLength,
			string additionalDesc = "")

			: base(serviceId, ServiceType.DataCollector, additionalDesc)
		{
			this.method = method;
			this.requestPath = requestPath;
			this.query = query;
			this.sourceAddr = sourceAddr;
			this.sourcePort = sourcePort;
			this.requestReceivedTime = requestReceivedTime;
			this.responseSendTime = DateTime.Now;
			this.statusCode = statusCode;
			this.responseType = responseType;
			this.responseLength = responseLength;
		}
	}
}