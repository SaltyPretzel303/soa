using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SensorService.Configuration
{

	public class ServiceConfiguration
	{

		private static string CONFIGURATION_PATH = "./service_config.json";
		private static string STAGE_VAR_NAME = "stage";

		private static ServiceConfiguration cache;

		// configuration fileds

		public string stage { get; private set; }

		public string dataPath { get;  set; }
		public string samplePrefix { get;  set; }
		public string sampleExtension { get;  set; }
		public FromTo samplesRange { get;  set; }
		public int readInterval { get;  set; }
		public int port { get;  set; }

		// methods

		public static ServiceConfiguration read()
		{

			if (ServiceConfiguration.cache != null)
			{
				Console.WriteLine("Reading configuration from cache ... ");
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