using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

		private List<IReloadable> changeListeners;

		#region mapped fields

		public string stage { get; private set; }

		public int listeningPort { get; set; }
		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }

		public string sensorRegistryTopic { get; set; }
		public string configUpdateTopic { get; set; }
		public string configFilter { get; set; }
		public string serviceLifetimeTopic { get; set; }
		public string collectorLifetimeFilter { get; set; }
		public string serviceLogTopic { get; set; }
		public string collectorLogFilter { get; set; }
		public string newSensorFilter { get; set; }
		public string sensorRemovedFilter { get; set; }
		public string sensorUpdatedFilter { get; set; }
		public string sensorEventTopic { get; set; }

		#endregion

		private static ServiceConfiguration read()
		{

			string s_config = File.ReadAllText(ServiceConfiguration.CONFIGURATION_PATH);
			JObject json_config = JObject.Parse(s_config);
			string stage = json_config.GetValue(ServiceConfiguration.STAGE_VAR_NAME).ToString();

			JObject conf_stage = (JObject)json_config.GetValue(stage);

			ServiceConfiguration config_o = conf_stage.ToObject<ServiceConfiguration>();
			config_o.stage = stage;

			return config_o;
		}

		public void UpdateConfig(JObject newConfig)
		{

			// TODO update Instance
			// TODO write to file maybe

			foreach (IReloadable singleListener in this.changeListeners)
			{
				singleListener.reload(ServiceConfiguration.Instance);
			}

		}

		public void ListenForChange(IReloadable newListener)
		{
			if (this.changeListeners == null)
			{
				this.changeListeners = new List<IReloadable>();
			}

			this.changeListeners.Add(newListener);
		}

	}
}