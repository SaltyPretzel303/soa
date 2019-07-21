using System;
using CRUDService.Data;
using Microsoft.AspNetCore.Mvc;

namespace CRUDService.Controller
{
	// port: 50002
	[Route("data/[controller]")]
	[ApiController]
	public class CrudController : ControllerBase
	{

		public IDatabaseService database { get; private set; }

		public CrudController(IDatabaseService database)
		{
			this.database = database;
		}

		[HttpGet]
		public String getAllData()
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