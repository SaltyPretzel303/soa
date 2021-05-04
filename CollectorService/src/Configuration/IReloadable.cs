
using System.Threading.Tasks;

namespace CollectorService.Configuration
{
	public interface IReloadable
	{
		Task reload(ConfigFields newConfiguration);
	}
}