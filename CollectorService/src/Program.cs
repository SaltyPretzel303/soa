using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SensorService.src;

namespace CRUDService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{

			string s_port = ServiceConfiguration.Instance.configRow("port");

			return WebHost.CreateDefaultBuilder(args)
				   .UseStartup<Startup>()
				   .UseUrls("http://+:" + s_port + "/");

		}
	}

}
