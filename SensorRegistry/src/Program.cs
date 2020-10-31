using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
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

			int portNum = ServiceConfiguration.Instance.listeningPort;

			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseUrls("http://+:" + portNum + "/");
		}
	}
}
