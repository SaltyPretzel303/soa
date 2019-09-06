using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SensorRegistry.Configuration;

namespace SensorRegistry
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{

			int portNum = ServiceConfiguration.read().listeningPort;

			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseUrls("http://+:" + portNum + "/");
		}
	}
}
