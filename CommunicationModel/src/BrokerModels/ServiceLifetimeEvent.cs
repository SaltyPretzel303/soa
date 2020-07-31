namespace CommunicationModel.BrokerModels
{
	public enum LifetimeStages
	{
		Startup,
		Shutdown,
		SensorRegistration,
		SensorUnregistration
	}

	public class ServiceLifetimeEvent : ServiceEvent
	{
		public LifetimeStages eventStage;

		public ServiceLifetimeEvent(LifetimeStages eventStage, string customMessage = "") :
				base(customMessage)
		{
			this.eventStage = eventStage;
			this.customMessage = customMessage;
		}
	}
}