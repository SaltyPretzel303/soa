using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using CollectorService.Data;

namespace CollectorService.Configuration
{
	public class ServiceConfiguration
	{

		public JObject rawJConfig { get; private set; }
		public string stage { get; set; }

		#region  mapped properties

		public int port { get; set; }

		public string dbAddress { get; set; }
		public string dbName { get; set; }
		public string sensorsCollection { get; set; }
		public string fieldWithRecords { get; set; }
		public string configurationBackupCollection { get; set; }
		public string configBackupField { get; set; }

		public string headerUrl { get; set; }
		public string dataRangeUrl { get; set; }

		public int readInterval { get; set; }

		public string sensorRegistryAddr { get; set; }
		public string sensorRegistryPort { get; set; }
		public string sensorAddrResolutionPath { get; set; }
		public string sensorListReqPath { get; set; }

		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }
		public int retryConnectionDelay { get; set; }

		public string collectorTopic { get; set; }
		public string accessEventFilter { get; set; }
		public string pullEventFilter { get; set; }

		public string serviceLogTopic { get; set; }
		public string serviceLifetimeTopic { get; set; }

		public string serviceTypeFilter { get; set; }
		public string configurationTopic { get; set; }
		public string targetConfiguration { get; set; }

		public string allFilter { get; set; }

		public string sensorRegistryTopic { get; set; }
		public string newSensorFilter { get; set; }
		public string sensorRemovedFilter { get; set; }

		#endregion

		private static string CONFIGURATION_PATH = "./service_config.json";
		private static string STAGE_VAR_NAME = "stage";

		#region  singleton specific
		private static ServiceConfiguration instance;
		public static ServiceConfiguration Instance
		{
			get
			{
				if (ServiceConfiguration.instance == null)
				{
					ServiceConfiguration.instance = ServiceConfiguration.readFromFile();
				}

				return ServiceConfiguration.instance;
			}
			private set { ServiceConfiguration.instance = value; }
		}

		#endregion

		private static ServiceConfiguration readFromFile()
		{

			Console.WriteLine($"Reading configuration file {ServiceConfiguration.CONFIGURATION_PATH} ... ");

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

		// write current active configuration to the config file
		private void writeToFile()
		{
			File.WriteAllText(ServiceConfiguration.CONFIGURATION_PATH, this.rawJConfig.ToString());
		}

		#region reload configuration specific

		private static List<IReloadable> reloadableTargets;

		public static void reload(JObject newConfig, IDatabaseService backupDatabase = null)
		{
			Console.WriteLine("Configuration reload requestd  ... ");

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

		public static void subscribeForChange(IReloadable reloadableTarget)
		{

			if (ServiceConfiguration.reloadableTargets == null)
			{
				ServiceConfiguration.reloadableTargets = new List<IReloadable>();
			}

			ServiceConfiguration.reloadableTargets.Add(reloadableTarget);

		}

		#endregion

		// never used 
		// public override string ToString()
		// {
		// 	return JObject.FromObject(this).ToString();
		// }

	}

}