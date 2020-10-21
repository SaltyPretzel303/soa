using System;
using System.Collections.Generic;
using CollectorService.Configuration;
using CollectorService.Data;
using CommunicationModel;
using CommunicationModel.RestModels.CollectorCRUDRequest;
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

		/* response format:
				[
				{sampleName: user_22, values[ {},{},{} ... }, 
				{sampleName: user_22, values[ {},{},{} ... }, 
				{sampleName: user_22, values[ {},{},{} ... },
				    .
				    .
				    .
				]
		 */
		[HttpGet]
		[Route("all")]
		public IActionResult getAllData()
		{

			if (this.database != null)
			{
				return Ok(this.database.getAllSamples());
			}

			return StatusCode(500);
		}

		[HttpGet]
		[Route("recordsRange")]
		public IActionResult getRecordsRange([FromQuery] string sensorName,
										[FromQuery] long fromTimestamp,
										[FromQuery] long toTimestamp)
		{

			if (fromTimestamp >= toTimestamp)
			{
				string message = $"Invalid timestamps:\nFromTimestamp: {fromTimestamp}\nToTimestamp: {toTimestamp}";
				Console.WriteLine("Bad request, invalid timestamps ... \n" + message);
				return BadRequest(message);
			}

			SensorDataRecords records = this.database.getRecordRange(sensorName, fromTimestamp, toTimestamp);

			if (records != null)
			{
				return Ok(records);
			}

			return StatusCode(500);
		}

		[HttpGet]
		[Route("recordsList")]
		public IActionResult getRecordsList([FromBody] GetListOfRecordsArg reqArg)
		{
			SensorDataRecords results = this.database.getRecordsList(reqArg.sensorName, reqArg.timestamps);

			return new OkObjectResult(results);
		}

		[HttpPost]
		[Route("addRecords")]
		public IActionResult addRecords([FromBody] AddRecordsArg reqArg)
		{
			JArray dataArray = new JArray();
			foreach (string txtData in reqArg.newRecords)
			{
				JObject tempData = JObject.Parse(txtData);
				dataArray.Add(tempData);
			}

			this.database.pushToSensor(reqArg.sensorName, dataArray);

			return new OkResult();
		}

		[HttpPost]
		[Route("updateRecord")]
		public IActionResult updateRecord([FromBody] UpdateRecordArg reqArg)
		{
			Console.WriteLine($"Request to update record:\nSensor name: {reqArg.sensorName}\nRecord timestamp: {reqArg.timestamp}\nField: {reqArg.field}\nNew value: {reqArg.value}");

			bool updateRes = this.database.updateRecord(reqArg.sensorName,
									reqArg.timestamp,
									reqArg.field,
									reqArg.value);
			if (updateRes == true)
			{
				return new OkResult();
			}
			else
			{
				return new BadRequestResult();
			}
		}

		[HttpDelete]
		[Route("deleteRecord")]
		public IActionResult deleteRecord([FromBody] DeleteRecordArg reqArg)
		{

			bool delResult = this.database.deleteRecord(reqArg.sensorName, reqArg.recordTimestamp);

			if (delResult == true)
			{
				return new OkResult();
			}
			else
			{
				return new BadRequestResult();
			}
		}

		[HttpDelete]
		[Route("deleteSensorData")]
		public IActionResult deleteSensorData([FromQuery] string sensorName)
		{

			bool delResult = this.database.deleteSensorData(sensorName);

			if (delResult == true)
			{
				return new OkResult();
			}
			else
			{
				return new BadRequestResult();
			}
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