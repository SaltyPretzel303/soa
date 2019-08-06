using System;
using System.Collections.Generic;
using CollectorService.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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


		[Route("sensorRecords")]
		[HttpGet]
		public IActionResult getSingleRecord([FromQuery]string sensorName, [FromQuery]int fromTimestamp = 0, [FromQuery] int toTimestamp = -1)
		{

			Console.WriteLine($"Received args: \n SensorName: {sensorName}\n From timestamp: {fromTimestamp} \n To timestamp: {toTimestamp}\n");

			List<JObject> ret_array = this.database.getRecordsFromSensor(sensorName, fromTimestamp, toTimestamp);

			if (ret_array != null)
			{
				return Ok(ret_array);

			}

			return StatusCode(500);
		}

	}
}