using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using CollectorService.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace CollectorService.Configuration
{
	public class ServiceConfiguration
	{

		private static string CONFIGURATION_PATH = "./service_config.json";

		private static string DEVELOPMENT_VALUE = "Development";
		private static string PRODUCTION_VALUE = "Production";

		public string stage { get; set; }

		public ConfigFields Production { get; set; }
		public ConfigFields Development { get; set; }

		#region  singleton specific

		private static ServiceConfiguration instance;
		public static ConfigFields Instance
		{
			get
			{
				if (instance == null)
				{
					instance = ServiceConfiguration.readFromFile();
				}

				if (instance.stage == DEVELOPMENT_VALUE)
				{
					return instance.Development;
				}
				else if (instance.stage == PRODUCTION_VALUE)
				{
					return instance.Production;
				}
				else
				{
					Console.WriteLine("Config. Stage field is in invalid format ... ");
					return null;
				}

			}
		}

		#endregion

		private static ServiceConfiguration readFromFile()
		{
			Console.WriteLine($"Reading configuration file {ServiceConfiguration.CONFIGURATION_PATH} ... ");

			string txtConfig = File.ReadAllText(ServiceConfiguration.CONFIGURATION_PATH);
			JObject jConfig = JObject.Parse(txtConfig);

			return jConfig.ToObject<ServiceConfiguration>();
		}

		private async Task writeToFile()
		{
			await File.WriteAllTextAsync(
				ServiceConfiguration.CONFIGURATION_PATH,
				JsonConvert.SerializeObject(instance));
		}

		#region reload configuration specific

		private static List<IReloadable> ReloadableTargets;

		public static async Task reload(
			ServiceConfiguration newConfig,
		 	IDatabaseService db = null)
		{
			Console.WriteLine("Configuration reload requested  ... ");

			if (db != null)
			{
				await db.backupConfiguration(ServiceConfiguration.instance);
			}

			ServiceConfiguration.instance = newConfig;

			// this is new config
			await ServiceConfiguration.instance.writeToFile();

			foreach (IReloadable target in ServiceConfiguration.ReloadableTargets)
			{
				await target.reload(ServiceConfiguration.Instance);
			}
		}

		public static void subscribeForChange(IReloadable reloadableTarget)
		{

			if (ServiceConfiguration.ReloadableTargets == null)
			{
				ServiceConfiguration.ReloadableTargets = new List<IReloadable>();
			}

			ServiceConfiguration.ReloadableTargets.Add(reloadableTarget);

		}

		#endregion

	}
}