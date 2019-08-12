using System.Runtime.CompilerServices;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CollectorService.Configuration
{

	public class ServiceConfiguration
	{

		private static ServiceConfiguration cache;

		private static string CONFIGURATION_PATH = "./service_config.json";
		private static string STAGE_VAR_NAME = "stage";

		private static bool shouldBeReloaded = false;

		// configuration variables

		public string stage { get; set; }

		public int port { get; set; }
		public string dbAddress { get; set; }
		public string dbName { get; set; }
		public string sensorsCollection { get; set; }
		public string recordsCollection { get; set; }
		public string fieldWithRecords { get; set; }
		public List<string> sensorsList { get; set; }

		public string headerUrl { get; set; }
		public string dataRangeUrl { get; set; }

		public int readInterval { get; set; }
		public string samplePrefix { get; set; }

		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }

		public string controllerReportExchange { get; set; }

		public static ServiceConfiguration read()
		{

			if (ServiceConfiguration.cache != null && ServiceConfiguration.shouldBeReloaded != true)
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
			// reset flag in case that reading is initiated because of it
			ServiceConfiguration.shouldBeReloaded = false;

			return config_o;
		}

		public static void markForReload()
		{
			ServiceConfiguration.shouldBeReloaded = true;
		}

	}
}