using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SensorService.Configuration;

namespace SensorService
{
	public class Program
	{
		public static void Main(string[] args)
		{

			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{

			ServiceConfiguration config = ServiceConfiguration.Instance;
			int portNum = config.listeningPort;

			return WebHost.CreateDefaultBuilder(args)
					.UseStartup<Startup>()
					.UseUrls("http://+:" + portNum + "/");

		}
	}
}
