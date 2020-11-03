using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using ServiceObserver.Configuration;

namespace ServiceObserver
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{

			int portToUse = ServiceConfiguration.Instance.port;

			return WebHost.CreateDefaultBuilder(args)
				  .UseStartup<Startup>()
				  .UseUrls($"http://localhost:{portToUse}");
		}
	}
}
