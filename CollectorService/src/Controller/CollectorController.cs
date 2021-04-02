using System;
using System.Collections.Generic;
using CollectorService.Configuration;
using CollectorService.Data;
using CommunicationModel;
using CommunicationModel.RestModels;
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

				List<SensorModel> dbResponse = this.database.getAllSamples();

				if (dbResponse == null)
				{
					return StatusCode(500);
				}

				List<SensorDataRecords> retData = new List<SensorDataRecords>();
				foreach (SensorModel model in dbResponse)
				{
					SensorDataRecords newRecord = new SensorDataRecords(
							model.sensorName,
							model.records.Count,
							model.records);

					retData.Add(newRecord);
				}

				return Ok(retData);
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

				string message = "Bad request, invalid timestamps: "
						+ $"FromTimestamp: {fromTimestamp} "
						+ $"ToTimestamp: {toTimestamp}";

				Console.WriteLine(message);

				return BadRequest(message);
			}

			SensorModel records = this.database.getRecordRange(
				sensorName,
				fromTimestamp,
				toTimestamp);

			if (records != null)
			{
				SensorDataRecords retData = new SensorDataRecords(
					records.sensorName,
					records.records.Count,
					records.records);

				return Ok(retData);
			}

			return StatusCode(500);
		}

		[HttpGet]
		[Route("recordsList")]
		public IActionResult getRecordsList([FromBody] GetListOfRecordsArg reqArg)
		{

			SensorModel dbResult = this.database.getRecordsList(
				reqArg.sensorName,
				reqArg.timestamps);

			if (dbResult != null)
			{
				SensorDataRecords retData = new SensorDataRecords(
					dbResult.sensorName,
					dbResult.records.Count,
					dbResult.records);

				return new OkObjectResult(retData);
			}

			return StatusCode(500);
		}

		[HttpPost]
		[Route("addRecords")]
		public IActionResult addRecords([FromBody] AddRecordsArg reqArg)
		{
			this.database.addToSensor(reqArg.sensorName, reqArg.newRecords);

			return new OkResult();
		}

		// TODO this is not refactored to use SensorModel
		[HttpPost]
		[Route("updateRecord")]
		public IActionResult updateRecord([FromBody] UpdateRecordArg reqArg)
		{
			Console.WriteLine($"Request to update record:"
				+ $"\nSensor name: {reqArg.sensorName}"
				+ $"\nRecord timestamp: {reqArg.timestamp}"
				+ $"\nField: {reqArg.field}"
				+ $"\nNew value: {reqArg.value}");

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

			bool delResult = this.database.deleteRecord(
				reqArg.sensorName,
				reqArg.timestamp);

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
		[Route("updateConfig")]
		public IActionResult updateConfiguration([FromBody] UpdateConfigArg configArg)
		{
			ServiceConfiguration.reload(JObject.Parse(configArg.TxtConfig), this.database);

			return StatusCode(200);
		}

	}

}