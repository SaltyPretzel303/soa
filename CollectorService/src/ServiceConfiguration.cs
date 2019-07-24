using System.IO;
using System.Net;
using System;
using Newtonsoft.Json.Linq;

namespace SensorService.src
{

	public class ServiceConfiguration
	{

		// singleton specific
		private static ServiceConfiguration instance;
		public static ServiceConfiguration Instance
		{
			get
			{

				if (ServiceConfiguration.instance == null)
				{

					Console.WriteLine("Creating new configuration ... ");

					ServiceConfiguration.instance = new ServiceConfiguration();

				}

				return instance;
			}
			set
			{
				ServiceConfiguration.instance = value;
			}
		}

		private static string configuration_path = "./service_config.json";
		public static string UNKNOWN_RESULT = "unknown_result";

		private JObject json_config;
		private string env_string_full;
		private string env_string;

		// constructor
		private ServiceConfiguration()
		{

			Console.WriteLine("Reading service configuration ... ");

			String config_file_content = File.ReadAllText(ServiceConfiguration.configuration_path);
			this.json_config = JObject.Parse(config_file_content);

			this.env_string_full = this.json_config.GetValue("env").ToString();
			if (this.env_string_full.ToLower() == "production")
			{
				this.env_string = "prod";
			}
			else if (this.env_string_full.ToLower() == "development")
			{
				this.env_string = "dev";
			}

			Console.WriteLine("Configuration file parsed ... ");
			Console.WriteLine("Configuration for: " + this.env_string);

		}

		// methods

		public string configRow(string config_name)
		{

			Console.WriteLine("Config read: " + config_name);

			JToken config_row = null;
			if (this.json_config.TryGetValue(config_name, out config_row))
			{

				// single_row : {
				// 		prod : value.
				// 		dev : value
				// }

				Console.WriteLine("Jtoken: " + config_row);


				return ((JObject)config_row)[this.env_string].ToString();


			}

			return ServiceConfiguration.UNKNOWN_RESULT;

		}

	}
}