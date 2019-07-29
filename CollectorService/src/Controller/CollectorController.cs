using System;
using System.Collections.Generic;
using CollectorService.Data;
using Microsoft.AspNetCore.Mvc;

namespace CollectorService.Controller
{
	// port: 50002
	[Route("data/access")]
	[ApiController]
	public class CollectorController : ControllerBase
	{

		public IDatabaseService database { get; private set; }

		public CollectorController(IDatabaseService database)
		{
			this.database = database;
		}

		[HttpGet]
		public string getAllDataAsync()
		{

			if (this.database != null)
			{
				return this.database.getAllRecords();
			}

			return "Database is null ... :( ... ";
		}

		[HttpGet("{user_id}", Name = "single")]
		public String getUser([FromQuery]String user_id)
		{

			// database get by id

			return "";
		}

		[HttpPost]
		public String addUser([FromBody]String user_data)
		{

			// database.store user_data

			return "";
		}


		[HttpDelete("{id}", Name = "delete")]
		public String removeUser([FromQuery] String user_id)
		{

			// database.delete with id

			return "";
		}

	}
}