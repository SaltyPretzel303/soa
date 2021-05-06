using System.Threading.Tasks;

namespace ServiceObserver.Configuration
{
	public interface IReloadable
	{

		Task reload(ConfigFields newConfig);

	}
}