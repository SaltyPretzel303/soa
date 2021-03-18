using System;
using System.Collections.Generic;
using CollectorService.Data;
using CommunicationModel.RestModels;
using Microsoft.AspNetCore.Mvc;
using ServiceObserver.Data;

namespace ServiceObserver.Controller
{
	// dev: 5004 prod: 3000
	[Route("observer/service")]
	[ApiController]
	public class ServiceObserverController : ControllerBase
	{

		private IDatabaseService db;

		public ServiceObserverController(IDatabaseService db)
		{
			this.db = db;
		}

		[HttpGet]
		public string getSomeString()
		{
			return "Hello there, I am observing, don't worry ... ";
		}

		[HttpGet]
		[Route("getAll")]
		public IActionResult getAllUnstableEvents()
		{
			List<UnstableServiceDbRecord> dbRecords = db.GetAllUnstableRecords();

			if (dbRecords != null)
			{
				List<UnstableServiceRecord> resList = new List<UnstableServiceRecord>();

				foreach (UnstableServiceDbRecord dbRecord in dbRecords)
				{
					resList.Add(new UnstableServiceRecord(dbRecord.serviceId,
													dbRecord.downCount,
													dbRecord.downEvents,
													dbRecord.recordedTime));
				}

				return new OkObjectResult(resList);
			}
			else
			{
				return StatusCode(500);
			}
		}

		[HttpGet]
		[Route("getForService")]
		public IActionResult getUnstableEventsForService([FromQuery] string serviceId)
		{
			List<UnstableServiceDbRecord> dbRecords = db.GetUnstableRecordsForService(serviceId);

			if (dbRecords != null)
			{
				List<UnstableServiceRecord> resList = new List<UnstableServiceRecord>();

				foreach (UnstableServiceDbRecord dbRecord in dbRecords)
				{
					resList.Add(new UnstableServiceRecord(dbRecord.serviceId,
													dbRecord.downCount,
													dbRecord.downEvents,
													dbRecord.recordedTime));
				}

				return new OkObjectResult(resList);
			}
			else
			{
				return StatusCode(500);
			}
		}

		[HttpGet]
		[Route("getLatest")]
		public IActionResult getLatestUnstableEvent()
		{

			UnstableServiceDbRecord dbRecord = db.GetLatestRecord();

			if (dbRecord != null)
			{

				UnstableServiceRecord resRecord = new UnstableServiceRecord(
													dbRecord.serviceId,
													dbRecord.downCount,
													dbRecord.downEvents,
													dbRecord.recordedTime);
				return new OkObjectResult(resRecord);
			}
			else
			{
				return StatusCode(500);
			}

		}

		[HttpGet]
		[Route("getOldConfig")]
		public IActionResult getConfigs()
		{
			ConfigBackupRecord record = db.GetConfigs();

			if (record != null)
			{
				Console.WriteLine("Record is not null ... ");
				List<ConfigRecord> resultList = new List<ConfigRecord>();

				foreach (DatedConfigRecord singleConfRec in record.oldConfigs)
				{
					resultList.Add(new ConfigRecord(singleConfRec.AsJsonConfig().ToString(),
													singleConfRec.backupDate));
				}

				return new OkObjectResult(resultList);
			}
			else
			{
				Console.WriteLine("Record is null ... ");
				return StatusCode(500);
			}
		}

	}
}