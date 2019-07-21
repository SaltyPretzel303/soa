using System.Net;
using System;
using DataCollector.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DataCollector.Controller
{
	// port: 5001
	[Route("data/[controller]")]
	[ApiController]
	public class ReaderController : ControllerBase
	{

		public Reader reader;

		public ReaderController(Reader reader)
		{
			this.reader = reader;

			if (this.reader != null)
			{
				Console.WriteLine("Reader is >>NOT<< null in controller constuctor ... ");
			}
			else
			{
				Console.WriteLine("Reader >>IS<< null in controller constructor ... ");
			}

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

			// try to get data for given index
			List<List<String>> ret_data = this.reader.getDataFrom(index);
			List<String> columns = new List<String>(this.reader.getHeader());

			if (ret_data != null && ret_data.Count > 0)
			{

				JObject json_result = new JObject();

				// initialize response header
				// add number of users
				// add number of rows
				json_result["users_count"] = ret_data.Count;
				json_result["rows_count"] = ret_data[0].Count; // count of rows for first user (should be the same for every other)

				for (int i = 0; i < ret_data.Count; i++)
				{
					List<String> user = ret_data[i];
					/*
					user list{
						value,valu	e,value,value;
						value,value,value,value;
						value,value,value,value;
					}
					 */

					// user is just a array of rows (json_rows)
					JArray json_user = new JArray();

					foreach (String row in user)
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

						json_user.Add(json_row);

					}
					json_result["user_" + i] = json_user;
				}

				return json_result.ToString();
			}

			return "Invalid index ... ";
		}

		// unused
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
