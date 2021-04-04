namespace ServiceObserver.Configuration
{
	public interface IReloadable
	{

		void reload(ConfigFields newConfig);

	}
}