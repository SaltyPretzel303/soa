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

		public ServiceLifetimeEvent(
			string sourceId,
			LifetimeStages eventStage,
			ServiceType type,
			string customMessage = "")

			: base(sourceId, type, customMessage)
		{
			this.lifeStage = eventStage;
		}
	}
}