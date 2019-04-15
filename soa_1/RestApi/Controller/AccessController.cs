using System.Threading;
using System;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controller
{


	[Route("data/[controller]")]
	[ApiController]
	public class AccessController
	{

		// private some reference to database

		//  CRUD
		// <><><>
		// create
		// read
		// update
		// delete

		[HttpGet]
		[Route("specific")]
		public String getById([FromQuery(Name = "id")] int id)
		{

			Console.WriteLine("this is much better ...");

			return "Everything is ok ... with id : " + id;
		}

		[HttpPost]
		public IActionResult postData([FromBody] int data)
		{
			return null;
		}

		[HttpPut]
		public IActionResult putData([FromBody] int data)
		{
			return null;
		}

		[HttpDelete("{id}")]
		public IActionResult deleteData(int id)
		{
			return null;
		}

	}
}