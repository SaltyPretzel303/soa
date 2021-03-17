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
		[Route("all")]
		public IActionResult getAllEvents()
		{

			return StatusCode(500);
		}

		[HttpGet]
		[Route("getConfig")]
		public IActionResult getConfigs()
		{
			ConfigBackupRecord record = db.getConfigs();

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

		[HttpGet]
		[Route("latest")]
		public IActionResult getLatest()
		{

			return StatusCode(500);
		}


	}
}