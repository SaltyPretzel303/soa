using System;
using System.Collections.Generic;
using CollectorService.Configuration;
using CollectorService.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CollectorService.Controller
{
	// dev: 5003 prod: 5000
	[Route("data/access")]
	[ApiController]
	public class CollectorController : ControllerBase
	{

		public IDatabaseService database { get; private set; }

		public CollectorController(IDatabaseService database)
		{
			this.database = database;
		}

		// CRUD

		// create

		// read

		// update

		// delete

		/* response format:
				[
				{sampleName: user_22, values[ {},{},{} ... }, {sampleName: user_22, values[ {},{},{} ... }, {sampleName: user_22, values[
				{},{},{} ... },
				    .
				    .
				    .
				]
		 */
		[Route("all")]
		[HttpGet]
		public IActionResult getAllData()
		{

			if (this.database != null)
			{
				return Ok(this.database.getAllSamples());
			}

			return StatusCode(500);
		}

		// custom home used for testing 
		public IActionResult getHome()
		{
			return Ok(database.customQuery());
		}

		[Route("sensorRecords")]
		[HttpGet]
		public IActionResult getSingleRecord([FromQuery]string sensorName, [FromQuery]long fromTimestamp, [FromQuery] long toTimestamp)
		{

			if (fromTimestamp >= toTimestamp)
			{

				string message = $"Invalid timestamps:\nFromTimestamp: {fromTimestamp}\nToTimestamp: {toTimestamp}";
				Console.WriteLine("Bad request, invalid timestamps ... \n" + message);
				return BadRequest(message);

			}

			List<JObject> ret_array = this.database.getRecordsFromSensor(sensorName, fromTimestamp, toTimestamp);

			if (ret_array != null)
			{

				return Ok(ret_array);

			}

			return StatusCode(500);
		}


		[HttpPost]
		public IActionResult updateConfiguration(string configUpdateRequest)
		{

			// check does this user has privileges to change config
			// backup current config in to the database 
			// write this configuration in to the config file on default path 

			// TODO follow above steps

			ServiceConfiguration.reload(JObject.Parse(configUpdateRequest), this.database);

			return StatusCode(200);

		}


	}
}