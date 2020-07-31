namespace CommunicationModel.BrokerModels
{

	public enum LogLevel
	{
		Error,
		Warning,
		Message
	}

	public class ServiceLog : ServiceEvent
	{

		public string Content { get { return base.customMessage; } }

		public LogLevel logLevel;

		public ServiceLog(string log, LogLevel logLevel = LogLevel.Message) :
			base(log)
		{
			this.logLevel = logLevel;
		}

	}
}