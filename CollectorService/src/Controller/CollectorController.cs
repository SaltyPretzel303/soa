using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectorService.Configuration;
using CollectorService.Data;
using CommunicationModel;
using CommunicationModel.RestModels;
using CommunicationModel.RestModels.CollectorCRUDRequest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
		public async Task<IActionResult> getAllData()
		{

			if (database == null)
			{
				return StatusCode(500);
			}

			List<SensorModel> dbResponse = await database.GetAllValues();

			if (dbResponse == null)
			{
				return StatusCode(500);
			}


			var retData = new List<CollectorDataRecords>();
			foreach (SensorModel model in dbResponse)
			{
				var newRecord = new CollectorDataRecords(
						model.sensorName,
						model.values.Count,
						model.values.Select((value) =>
						{
							var str_value = JsonConvert.SerializeObject(value);
							return (JObject.Parse(str_value)).ToObject<SensorValues>();
						}).ToList()
						);

				retData.Add(newRecord);
			}

			return Ok(retData);
		}

		[HttpGet]
		[Route("recordsRange")]
		public async Task<IActionResult> getRecordsRange(
			[FromQuery] string sensorName,
			[FromQuery] long fromTimestamp,
			[FromQuery] long toTimestamp)
		{

			if (fromTimestamp >= toTimestamp)
			{

				string message = "Requested invalid timestamps: "
					+ $"FromTimestamp: {fromTimestamp} "
					+ $"ToTimestamp: {toTimestamp}";

				Console.WriteLine(message);

				return BadRequest(message);
			}
			
			SensorModel dbRecord = await database.getRecordRange(
				sensorName,
				fromTimestamp,
				toTimestamp);

			if (dbRecord != null)
			{
				var retData = new CollectorDataRecords(
					dbRecord.sensorName,
					dbRecord.values.Count,
					dbRecord.values.Select((value) =>
						{
							var str_value = JsonConvert.SerializeObject(value);
							return (JObject.Parse(str_value)).ToObject<SensorValues>();
						}).ToList()
					);

				return Ok(retData);
			}

			return StatusCode(500);
		}

		[HttpGet]
		[Route("recordsList")]
		public async Task<IActionResult> getRecordsList(
			[FromBody] GetListOfRecordsArg reqArg)
		{

			SensorModel dbResult = await database.getRecordsList(
				reqArg.sensorName,
				reqArg.timestamps);

			if (dbResult != null)
			{
				var retData = new CollectorDataRecords(
					dbResult.sensorName,
					dbResult.values.Count,
					dbResult.values.Select((value) =>
						{
							var str_value = JsonConvert.SerializeObject(value);
							return (JObject.Parse(str_value)).ToObject<SensorValues>();
						}).ToList()
					);

				return new OkObjectResult(retData);
			}

			return StatusCode(500);
		}

		[HttpPost]
		[Route("addRecords")]
		public async Task<IActionResult> addRecords([FromBody] AddRecordsArg reqArg)
		{
			await this.database.AddToSensor(reqArg.sensorName, reqArg.newRecords);

			return new OkResult();
		}

		[HttpPost]
		[Route("updateRecord")]
		public async Task<IActionResult> updateRecord([FromBody] UpdateRecordArg reqArg)
		{
			Console.WriteLine($"Request to update record:"
				+ $"\nSensor name: {reqArg.sensorName}"
				+ $"\nRecord timestamp: {reqArg.timestamp}"
				+ $"\nField: {reqArg.field}"
				+ $"\nNew value: {reqArg.value}");

			bool updateRes = await database.updateRecord(
				reqArg.sensorName,
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
		public async Task<IActionResult> deleteRecord([FromBody] DeleteRecordArg reqArg)
		{

			bool delResult = await database.deleteRecord(
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
		public async Task<IActionResult> deleteSensorData([FromQuery] string sensorName)
		{

			bool delResult = await database.deleteSensorData(sensorName);

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
		public async Task<IActionResult> updateConfiguration([FromBody] UpdateConfigArg configArg)
		{
			await ServiceConfiguration.reload(
				JsonConvert.DeserializeObject<ServiceConfiguration>(configArg.TxtConfig),
				this.database);

			return StatusCode(200);
		}

	}

}