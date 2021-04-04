using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ServiceObserver.Data;
using Newtonsoft.Json;

namespace ServiceObserver.Configuration
{
	public class ServiceConfiguration
	{

		private static string CONFIGURATION_PATH = "./service_config.json";

		private static string DEVELOPMENT_VALUE = "Development";
		private static string PRODUCTION_VALUE = "Production";

		// public JObject rawJConfig { get; private set; }
		public string stage { get; set; }

		public ConfigFields Production { get; set; }
		public ConfigFields Development { get; set; }

		// singleton specific
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

		private static ServiceConfiguration readFromFile()
		{
			Console.WriteLine($"Reading configuration file {ServiceConfiguration.CONFIGURATION_PATH} ... ");

			string txtConfig = File.ReadAllText(ServiceConfiguration.CONFIGURATION_PATH);
			JObject jConfig = JObject.Parse(txtConfig);

			return jConfig.ToObject<ServiceConfiguration>();
		}

		private void writeToFile()
		{
			File.WriteAllText(
				ServiceConfiguration.CONFIGURATION_PATH,
				JsonConvert.SerializeObject(instance));
		}

		#region config reload

		// reload specific methods

		private static List<IReloadable> reloadableTargets;

		public static void reload(ServiceConfiguration newConfig, IDatabaseService db = null)
		{
			if (db != null)
			{
				db.BackupConfiguration(ServiceConfiguration.instance);
			}

			ServiceConfiguration.instance = newConfig;
			ServiceConfiguration.instance.writeToFile();

			foreach (IReloadable target in ServiceConfiguration.reloadableTargets)
			{
				target.reload(ServiceConfiguration.Instance);
			}
		}

		public static void subscribeForReload(IReloadable reloadableTarget)
		{

			if (ServiceConfiguration.reloadableTargets == null)
			{
				ServiceConfiguration.reloadableTargets = new List<IReloadable>();
			}

			ServiceConfiguration.reloadableTargets.Add(reloadableTarget);

		}

		#endregion

	}
}