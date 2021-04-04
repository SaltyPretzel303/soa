
namespace CollectorService.Configuration
{
	public interface IReloadable
	{
		void reload(ConfigFields newConfiguration);
	}
}