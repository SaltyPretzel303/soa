using System.IO;
using System.Net;
using System.Threading;
using System;
using Microsoft.AspNetCore.Mvc;

namespace RestApi.Controller
{

	// port: 5000
	[Route("data/[controller]")]
	[ApiController]
	public class AccessController
	{

		private static String data_url = "http://localhost:5002/data/crud";

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

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AccessController.data_url);

			// implement this later
			// request.AutomaticDecompression = DecompressionMethods.GZip; 


			String requested_data = String.Empty;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				requested_data = reader.ReadToEnd();
			}

			Console.WriteLine("Here comes the data ........................ ");
			Console.WriteLine(requested_data);

			return "GET ALL DATA REQUEST ...  ";
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