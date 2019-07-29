using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SensorService.Configuration
{

	public class ServiceConfiguration
	{

		private static ServiceConfiguration cache;

		private static string CONFIGURATION_PATH = "./service_config.json";
		private static string STAGE_VAR_NAME = "stage";

		// configuration variables

		public string stage { get; set; }

		public int port { get; set; }
		public string dbAddress { get; set; }
		public string dbName { get; set; }
		public string collectionName { get; set; }
		public string dbUserArray { get; set; }
		public List<string> sensorsList { get; set; }

		public string headerUrl { get; set; }
		public string dataRangeUrl { get; set; }

		public int readInterval { get; set; }
		public string samplePrefix { get; set; }


		public static ServiceConfiguration read()
		{

			if (ServiceConfiguration.cache != null)
			{
				Console.WriteLine("Reading configuration from cache ... ");
				return ServiceConfiguration.cache;
			}

			Console.WriteLine("Reading configuration file ... ");

			string raw_config = File.ReadAllText(ServiceConfiguration.CONFIGURATION_PATH);
			JObject json_config = JObject.Parse(raw_config);
			string stage = json_config.GetValue(ServiceConfiguration.STAGE_VAR_NAME).ToString();

			JObject conf_stage = (JObject)json_config.GetValue(stage);

			ServiceConfiguration config_o = conf_stage.ToObject<ServiceConfiguration>();
			config_o.stage = stage;

			ServiceConfiguration.cache = config_o;

			return config_o;
		}

	}
}