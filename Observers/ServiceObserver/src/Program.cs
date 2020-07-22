﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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