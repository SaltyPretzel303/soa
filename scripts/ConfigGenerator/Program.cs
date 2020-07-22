using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ConfigGenerator
{
	class Program
	{
		/*
			args[1] = starting index
			args[2] = config. destination
		*/
		static void Main(string[] args)
		{

			int index = int.Parse(args[0]);
			Console.WriteLine("Generating (dotnet): " + index);
			string destination = args[1];
			Console.WriteLine("On path: " + destination);

			string s_template = File.ReadAllText("./template");
			int port_num = 5040;

			JObject root = JObject.Parse(s_template);

			JObject dev_part = (JObject)root["Development"];
			JObject prod_part = (JObject)root["Production"];

			// TODO change this part to list of filenames

			JObject samples_range = new JObject();
			samples_range["From"] = index;
			samples_range["To"] = index + 1;

			dev_part["samplesRange"] = samples_range;
			prod_part["samplesRange"] = samples_range;

			// end of changes

			dev_part["listeningPort"] = port_num + index;
			prod_part["listeningPort"] = port_num + index;


			dev_part["sensorName"] = "sensor_" + index;
			prod_part["sensorName"] = "sensor_" + index;

			root["Development"] = dev_part;
			root["Production"] = prod_part;


			string output_config = root.ToString();
			File.WriteAllText(destination, output_config);

		}
	
	}
}
