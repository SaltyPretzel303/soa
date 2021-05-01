using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace SensorRegistry.Configuration
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
					ServiceConfiguration.instance = ServiceConfiguration.read();
				}

				return ServiceConfiguration.instance;
			}
			private set { ServiceConfiguration.instance = value; }
		}

		#endregion

		private static List<IReloadable> ChangeListeners;

		public JObject rawJsonConfig { get; private set; }

		#region mapped fields

		public string stage { get; private set; }

		public int listeningPort { get; set; }
		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }

		public int connectionRetryDelay { get; set; }

		public string sensorRegistryTopic { get; set; }
		public string configUpdateTopic { get; set; }
		public string configFilter { get; set; }
		public string serviceLifetimeTopic { get; set; }
		public string registryLifetimeFilter { get; set; }
		public string serviceLogTopic { get; set; }
		public string registryLogFilter { get; set; }
		public string newSensorFilter { get; set; }
		public string sensorRemovedFilter { get; set; }
		public string sensorUpdateFilter { get; set; }

		public string sensorEventTopic { get; set; }
		public string sensorReadEventFilter { get; set; }
		public string sensorLifetimeFilter { get; set; }

		public string sensorInfoPath { get; set; }
		public string sensorInfoNameArg { get; set; }

		#endregion

		private static ServiceConfiguration read()
		{

			string strConfig = File.ReadAllText(ServiceConfiguration.CONFIGURATION_PATH);
			JObject jsonConfig = JObject.Parse(strConfig);
			string stage = jsonConfig.GetValue(ServiceConfiguration.STAGE_VAR_NAME).ToString();

			JObject confStage = (JObject)jsonConfig.GetValue(stage);

			ServiceConfiguration objConfig = confStage.ToObject<ServiceConfiguration>();
			objConfig.stage = stage;

			return objConfig;
		}

		public void UpdateConfig(JObject newConfig)
		{

			Console.WriteLine("Configuration update requested ... ");

			ServiceConfiguration.Instance = ServiceConfiguration.parseJson(newConfig);
			ServiceConfiguration.Instance.WriteToFile();

			foreach (IReloadable singleListener in ChangeListeners)
			{
				singleListener.reload(ServiceConfiguration.Instance);
			}
		}

		public void ListenForChange(IReloadable newListener)
		{
			if (ChangeListeners == null)
			{
				ChangeListeners = new List<IReloadable>();
			}

			ChangeListeners.Add(newListener);
		}

		public static ServiceConfiguration parseJson(JObject jsonConfig)
		{
			string stage = jsonConfig.GetValue(ServiceConfiguration.STAGE_VAR_NAME).ToString();

			JObject confStage = (JObject)jsonConfig.GetValue(stage);

			ServiceConfiguration objConfig = confStage.ToObject<ServiceConfiguration>();
			objConfig.stage = stage;
			objConfig.rawJsonConfig = jsonConfig;

			return objConfig;
		}

		private void WriteToFile()
		{
			File.WriteAllText(
				ServiceConfiguration.CONFIGURATION_PATH,
				this.rawJsonConfig.ToString());
		}

	}
}