using System.IO;
using System;
using Newtonsoft.Json.Linq;

namespace SensorRegistry.Configuration
{

	public class ServiceConfiguration
	{

		private static string CONFIGURATION_PATH = "./service_config.json";
		private static string STAGE_VAR_NAME = "stage";

		private static ServiceConfiguration cache;

		// configuration fileds

		public string stage { get; private set; }

		public int listeningPort { get; set; }
		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }

		public string serviceReportTopic { get; set; }
		public string registryReportFilter { get; set; }
		public string configurationTopic { get; set; }
		public string targetConfiguration { get; set; }

		public string sensorRegistryTopic { get; set; }
		public string newSensorFilter { get; set; }
		public string sensorRemovedFilter { get; set; }

		public string logMsgTimestampField { get; set; }
		public string logMsgContentField { get; set; }

		public string logErrorTimestampField { get; set; }
		public string logErrorContentField { get; set; }


		// methods

		public static ServiceConfiguration read()
		{

			if (ServiceConfiguration.cache != null)
			{
				return ServiceConfiguration.cache;
			}

			Console.WriteLine("Reading configuration file ... ");

			string s_config = File.ReadAllText(ServiceConfiguration.CONFIGURATION_PATH);
			JObject json_config = JObject.Parse(s_config);
			string stage = json_config.GetValue(ServiceConfiguration.STAGE_VAR_NAME).ToString();

			JObject conf_stage = (JObject)json_config.GetValue(stage);

			ServiceConfiguration config_o = conf_stage.ToObject<ServiceConfiguration>();
			config_o.stage = stage;

			ServiceConfiguration.cache = config_o;

			return config_o;
		}

	}
}