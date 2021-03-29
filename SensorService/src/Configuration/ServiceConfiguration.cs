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

		#region  singleton specific
		private static ServiceConfiguration instance;
		public static ServiceConfiguration Instance
		{
			get
			{
				if (ServiceConfiguration.instance == null)
				{
					ServiceConfiguration.instance = ServiceConfiguration.Read();
				}

				return ServiceConfiguration.instance;
			}
			private set { ServiceConfiguration.instance = value; }
		}

		#endregion

		private static ServiceConfiguration cache;

		#region mapped fields
		public string stage { get; private set; }

		public string dataPath { get; set; }
		public string samplePrefix { get; set; }
		public string sampleExtension { get; set; }
		public FromTo sensorsRange { get; set; }
		public int readInterval { get; set; }

		public int listeningPort { get; set; }
		public string hostIP { get; set; }

		public string sensorNamePrefix { get; set; }
		public int registerSensorDelay { get; set; }
		public string registryAddress { get; set; }
		public string registryPort { get; set; }
		public string registerSensorPath { get; set; }
		public string unregisterSensorPath { get; set; }
		public string updateSensorPath { get; set; }
		public string sensorNameField { get; set; }

		public string consoleLogLevel { get; set; }
		public string logErrorLevel { get; set; }
		public string logMessageLevel { get; set; }

		public string logErrorDest { get; set; }
		public string logMsgDest { get; set; }

		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }
		public string serviceLogTopic { get; set; }
		public string sensorLogFilter { get; set; }
		public string sensorReaderEventTopic { get; set; }
		public string sensorReadEventFilter { get; set; }
		public string brokerConfigTopic { get; set; }
		public string brokerConfigFilter { get; set; }
		public string serviceLifetimeTopic { get; set; }
		public string sensorLifetimeFilter { get; set; }

		#endregion

		private static ServiceConfiguration Read()
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