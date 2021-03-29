namespace CommunicationModel.BrokerModels
{
	public class SensorLifetimeEvent : ServiceLifetimeEvent
	{
		public string SensorName { get; set; }
		public string IpAddress { get; set; }
		public int ListeningPort { get; set; }
		public int LastReadIndex { get; set; }

		public SensorLifetimeEvent(LifetimeStages eventStage,
								string SensorName,
								string IpAddress,
								int ListeningPort,
								int LastReadIndex,
								string customMessage = "")
		: base(eventStage, ServiceType.SensorReader, customMessage)
		{
			this.SensorName = SensorName;
			this.IpAddress = IpAddress;
			this.ListeningPort = ListeningPort;
			this.LastReadIndex = LastReadIndex;
		}
	}
}