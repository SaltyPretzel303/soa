using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SensorService.Configuration;

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

			ServiceConfiguration config = ServiceConfiguration.read();

			return WebHost.CreateDefaultBuilder(args)
				   .UseStartup<Startup>()
				   .UseUrls("http://+:" + config.port + "/");

		}
	}

}
