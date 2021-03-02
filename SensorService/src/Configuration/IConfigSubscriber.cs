namespace SensorService.Configuration
{
	public interface IConfigSubscriber
	{
		void update(ServiceConfiguration newConfig);
	}
}