using System;

namespace CommunicationModel.BrokerModels
{
	public class CollectorAccessEvent : ServiceEvent
	{
		public string method;
		public string requestPath;
		public string query;

		public string sourceAddr;
		public int? sourcePort;

		public DateTime requestReceivedTime;
		public DateTime responseSendTime;

		public int statusCode;
		public string repsonseType;
		public long? responseLength;

		public CollectorAccessEvent(string method,
							string requestPath,
							string query,
							string sourceAddr,
							int? sourcePort,
							DateTime requestReceivedTime,
							int statusCode,
							string repsonseType,
							long? responseLength,
							string additionalDesc = "")
				: base(additionalDesc)
		{
			this.method = method;
			this.requestPath = requestPath;
			this.query = query;
			this.sourceAddr = sourceAddr;
			this.sourcePort = sourcePort;
			this.requestReceivedTime = requestReceivedTime;
			this.responseSendTime = DateTime.Now;
			this.statusCode = statusCode;
			this.repsonseType = repsonseType;
			this.responseLength = responseLength;
		}
	}
}