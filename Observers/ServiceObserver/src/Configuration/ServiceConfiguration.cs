using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ServiceObserver.Storage;
using ServiceObserver.Report;

namespace ServiceObserver.Configuration
{

	public class ServiceConfiguration
	{

		public JObject rawJConfig { get; private set; }

		// configuration variables
		public string stage { get; set; }

		public int port { get; set; }
		public string dbAddress { get; set; }
		public string dbName { get; set; }
		public string serviceEventsCollections { get; set; }
		public string fieldWithRecords { get; set; }
		public string configurationBackupCollection { get; set; }

		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }

		public string serviceReportTopic { get; set; }
		public string serviceReportFilter { get; set; }
		public string configurationTopic { get; set; }
		public string serviceQosTopic { get; set; }
		public string targetConfiguration { get; set; }

		public List<Pattern> patters { get; set; }

		// ent of the configuration variables

		private static string CONFIGURATION_PATH = "./service_config.json";
		private static string STAGE_VAR_NAME = "stage";
		private static string rawConfig;


		// singleton specific
		private static ServiceConfiguration instance;
		public static ServiceConfiguration Instance
		{
			get
			{

				if (ServiceConfiguration.instance == null)
				{
					ServiceConfiguration.instance = ServiceConfiguration.readFromFile();
				}
				else
				{
					Console.WriteLine("Reading cached configuration ... ");
				}

				return ServiceConfiguration.instance;
			}
			private set { ServiceConfiguration.instance = value; }
		}

		private static ServiceConfiguration readFromFile()
		{

			Console.WriteLine("Reading configuration file ... ");

			string rawConfig = File.ReadAllText(ServiceConfiguration.CONFIGURATION_PATH);
			JObject json_config = JObject.Parse(rawConfig);

			return ServiceConfiguration.extractFromJson(json_config);
		}

		private static ServiceConfiguration extractFromJson(JObject jConfig)
		{

			string stage = jConfig.GetValue(ServiceConfiguration.STAGE_VAR_NAME).ToString();
			JObject conf_stage = (JObject)jConfig.GetValue(stage);

			ServiceConfiguration config_o = conf_stage.ToObject<ServiceConfiguration>();
			config_o.stage = stage;
			config_o.rawJConfig = jConfig;

			return config_o;

		}

		private void writeToFile()
		{

			File.WriteAllText(ServiceConfiguration.CONFIGURATION_PATH, this.rawJConfig.ToString());

		}

		// reload specific methods

		private static List<IReloadable> reloadableTargets;

		public static void reload(JObject newConfig, IDatabaseService backupDatabase = null)
		{

			// backup old configuration
			if (backupDatabase != null)
			{
				backupDatabase.backupConfiguration(ServiceConfiguration.Instance.rawJConfig);
			}

			ServiceConfiguration.Instance = ServiceConfiguration.extractFromJson(newConfig);

			ServiceConfiguration.Instance.writeToFile();

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

		public override string ToString()
		{
			return JObject.FromObject(this).ToString();
		}

	}
}