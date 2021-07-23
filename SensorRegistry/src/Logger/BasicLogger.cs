using CommunicationModel.BrokerModels;
using Newtonsoft.Json;
using SensorRegistry.Broker;
using SensorRegistry.Configuration;

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
			var log = new ServiceLog(
				ServiceConfiguration.Instance.serviceId,
				ServiceType.SensorRegistry,
				error,
				LogLevel.Error);
				
			broker.publishLog(log);

			return JsonConvert.SerializeObject(log, Formatting.Indented);
		}

		public string LogMessage(string message)
		{
			var log = new ServiceLog(
				ServiceConfiguration.Instance.serviceId,
				ServiceType.SensorRegistry,
				message,
				LogLevel.Message);

			broker.publishLog(log);

			return JsonConvert.SerializeObject(log, Formatting.Indented);
		}
	}
}