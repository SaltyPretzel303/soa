namespace CommunicationModel.BrokerModels
{
	public enum LifetimeStages
	{
		Startup,
		Shutdown
	}

	public class ServiceLifetimeEvent : ServiceEvent
	{
		public LifetimeStages lifeStage;

		public ServiceLifetimeEvent(LifetimeStages eventStage,
								ServiceType type,
								string customMessage = "") :
				base(type, customMessage)
		{
			this.lifeStage = eventStage;
		}
	}
}