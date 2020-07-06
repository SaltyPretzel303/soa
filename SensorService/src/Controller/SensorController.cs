using System.Threading.Tasks.Dataflow;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SensorService.Data;
using Newtonsoft.Json;
using SensorService.Configuration;
using SensorService.Logger;

namespace DataCollector.Controller
{
	// port: 5001
	[Route("sensor/data")]
	[ApiController]
	public class SensorController : ControllerBase
	{

		public IReader reader;
		public ILogger logger;

		public SensorController(IReader reader, ILogger logger)
		{

			this.reader = reader;
			this.logger = logger;

		}

		// ping request
		[HttpGet]
		public String homeGet()
		{
			return "NOT DEFAULT hello world ... (it works ... ) ";
		}

		[HttpGet("{index}")]
		[Route("range")]
		public JObject getRowsFrom([FromQuery] int index)
		{

			ServiceConfiguration conf = ServiceConfiguration.read();

			logger.logMessage("Data range request for index: " + index);
			// Console.WriteLine("Data range request for index: " + index);

			// try to get data for given index
			List<List<String>> ret_data = this.reader.getDataFrom(index);

			if (ret_data != null && ret_data.Count > 0)
			{

				List<String> columns = new List<String>(this.reader.getHeader());

				JObject json_result = new JObject();

				// initialize response header
				json_result[conf.responseTypeField] = conf.validResponse;
				json_result["samples_count"] = ret_data.Count; // TODO remove, (to_sample - from_sample) gives this value
				json_result["rows_count"] = ret_data[0].Count; // count of rows for first sensor (should be the same for every other)
				json_result["from_sample"] = conf.samplesRange.From;
				json_result["to_sample"] = conf.samplesRange.To;

				json_result["sensor_name_prefix"] = conf.sensorNamePrefix;

				int sample_counter = reader.getSamplesRange().From;

				foreach (List<string> single_sample in ret_data)
				{
					/*
					records list{
						value,value,value,value;
						value,value,value,value;
						value,value,value,value;
					}
					 */

					// user is just a array of rows (json_rows)
					JArray json_sample = new JArray();

					foreach (String row in single_sample)
					{
						/*
						row{
							value,value,value,value;
						}
						 */

						JObject json_row = new JObject();

						String[] values = row.Split(",");
						for (int j = 0; j < values.Length; j++)
						{
							json_row[columns[j]] = values[j];
						}

						json_sample.Add(json_row);

					}
					json_result[conf.sensorNamePrefix + sample_counter] = json_sample;
					sample_counter++;
				}

				return json_result;
			}

			return JsonConvert.DeserializeObject<JObject>("{'sensor_response': 'Sensor cant return value for: index=" + index + "' }");
		}

		// unused
		// leave it for possible later optimisation
		[HttpGet]
		[Route("header")]
		public string getHeader()
		{

			JArray array = new JArray();
			foreach (String column in this.reader.getHeader())
			{
				array.Add(column);
			}

			return array.ToString();
		}

	}
}
