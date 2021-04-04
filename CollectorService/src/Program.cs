using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using CollectorService.Configuration;

namespace CollectorService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{

			ConfigFields config = ServiceConfiguration.Instance;

			return WebHost.CreateDefaultBuilder(args)
				   .UseStartup<Startup>()
				   .UseUrls("http://+:" + config.port + "/");

		}
	}

}
