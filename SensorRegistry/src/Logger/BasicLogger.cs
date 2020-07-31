using System;
using CommunicationModel.BrokerModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SensorRegistry.Broker;

namespace SensorRegistry.Logger
{
	public class BasicLogger : ILogger
	{

		private IMessageBroker broker;

		public BasicLogger(IMessageBroker broker)
		{
			this.broker = broker;
		}

		public string LogError(string error)
		{
			ServiceLog log = new ServiceLog(error, LogLevel.Error);
			this.broker.publishLog(log);

			return JsonConvert.SerializeObject(log, Formatting.Indented);
		}

		public string LogMessage(string message)
		{
			ServiceLog log = new ServiceLog(message, LogLevel.Message);
			this.broker.publishLog(log);

			return JsonConvert.SerializeObject(log, Formatting.Indented);
		}
	}
}