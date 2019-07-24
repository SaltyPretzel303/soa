using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SensorService.src;

namespace DataCollector
{
	public class Program
	{
		public static void Main(string[] args)
		{

			CliArgsParser parsed_args = new CliArgsParser(args);
			if (!parsed_args.success)
			{
				return;
			}

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
