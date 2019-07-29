using System.Runtime.Serialization.Json;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SensorService.Data;

namespace DataCollector.Controller
{
	// port: 5001
	[Route("data/[controller]")]
	[ApiController]
	public class SensorController : ControllerBase
	{

		public Reader reader;

		public SensorController(Reader reader)
		{

			this.reader = reader;

		}

		[HttpGet]
		public String homeGet()
		{
			// just for testing
			// leave it
			return "NOT DEFAULT hello world ... ";
		}

		[HttpGet("{index}")]
		[Route("range")]
		public String getRowsFrom([FromQuery]int index)
		{

			Console.WriteLine("Data range request for index: " + index);

			// try to get data for given index
			List<List<String>> ret_data = this.reader.getDataFrom(index);

			if (ret_data != null && ret_data.Count > 0)
			{

				List<String> columns = new List<String>(this.reader.getHeader());

				JObject json_result = new JObject();

				// initialize response header
				// add number of users
				// add number of rows
				json_result["samples_count"] = ret_data.Count;
				json_result["rows_count"] = ret_data[0].Count; // count of rows for first user (should be the same for every other)
				json_result["from_sample"] = this.reader.samplesRange.From;
				json_result["to_sample"] = this.reader.samplesRange.To;
				json_result["sample_prefix"] = this.reader.prefix;

				int sample_counter = reader.samplesRange.From;

				foreach (List<string> single_sample in ret_data)
				{
					/*
					user list{
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
					json_result[reader.prefix + sample_counter] = json_sample;
					sample_counter++;
				}

				return json_result.ToString();
			}

			return "Invalid index ... ";
		}

		// unused
		// leave it for later optimisation
		[HttpGet]
		[Route("header")]
		public String getHeader()
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
