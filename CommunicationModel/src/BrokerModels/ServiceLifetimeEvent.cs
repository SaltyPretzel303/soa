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
		public LifetimeStages lifeStage;

		public ServiceLifetimeEvent(LifetimeStages eventStage, string customMessage = "") :
				base(customMessage)
		{
			this.lifeStage = eventStage;
			this.customMessage = customMessage;
		}
	}
}