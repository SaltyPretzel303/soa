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

		public string dataPath { get; set; }
		public string responseTypeField { get; set; }
		public string validResponse { get; set; }
		public string dataPrefix { get; set; }
		public string sampleExtension { get; set; }
		public FromTo samplesRange { get; set; }
		public int readInterval { get; set; }
		public int listeningPort { get; set; }
		public string sensorNamePrefix { get; set; }
		public string registryAddress { get; set; }
		public string registryPort { get; set; }
		public string sensorName { get; set; }
		public string registerSensorPath { get; set; }
		public string unregisterSensorPath { get; set; }
		public string updateSensorPath { get; set; }
		public string sensorNameField { get; set; }
		public string sensorAddrField { get; set; }
		public string portNumField { get; set; }
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