using System.Net;
using System;
using DataCollector.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DataCollector.Controller
{
	[Route("data/[controller]")]
	[ApiController]
	public class AccessController : ControllerBase
	{

		public Reader reader;

		public AccessController(Reader reader)
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
			return "NOT hello world ... ";
		}

		[HttpGet("{index}")]
		[Route("range")]
		public String getDataFrom([FromQuery]int index)
		{

			// try to get data for given index
			List<List<String>> ret_data = this.reader.getDataFrom(index);
			String[] columns = this.reader.getColumns();

			if (ret_data != null)
			{

				JObject json_result = new JObject();
				JArray users = new JArray();

				foreach (List<String> user in ret_data)
				{
					/*
					user list{
						value,value,value,value;
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
						for (int i = 0; i < values.Length; i++)
						{
							json_row[columns[i]] = values[i];
						}

						json_user.Add(json_row);

					}
					users.Add(json_user);
				}

				json_result["users"] = users;

				return json_result.ToString();
			}

			return "Invalid index ... ";
		}

	}
}