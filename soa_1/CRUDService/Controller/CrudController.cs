using System;
using CRUDService.Data;
using Microsoft.AspNetCore.Mvc;

namespace CRUDService.Controller
{
	[Route("data/[controller]")]
	[ApiController]
	public class CrudController : ControllerBase
	{

		public DatabaseService database { get; private set; }

		public CrudController(DatabaseService database)
		{
			this.database = database;
		}

		[HttpGet]
		public String getAllData()
		{

			// database get all

			return "";
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