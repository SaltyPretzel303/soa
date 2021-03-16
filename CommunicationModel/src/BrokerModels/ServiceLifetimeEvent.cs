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

		public ServiceLifetimeEvent()
		{

		}

		public ServiceLifetimeEvent(LifetimeStages eventStage,
								ServiceType type,
								string customMessage = "") :
				base(type, customMessage)
		{
			this.lifeStage = eventStage;
		}
	}
}