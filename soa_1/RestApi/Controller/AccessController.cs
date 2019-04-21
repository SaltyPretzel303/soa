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
		// read
		// 		all 
		// 		specific
		// create
		// update
		// delete
		// 		user
		//		row

		[HttpGet]
		[Route("all")]
		public String getAllData()
		{
			return "";
		}

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

		[HttpDelete("{id}")]
		public IActionResult deleteData(int id)
		{
			return null;
		}

	}
}