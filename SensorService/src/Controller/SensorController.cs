using System;
using Microsoft.AspNetCore.Mvc;
using SensorService.Configuration;
using SensorService.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommunicationModel;

namespace DataCollector.Controller
{
	// port: 5001
	[Route("sensor/data")]
	[ApiController]
	public class SensorController : ControllerBase
	{

		public ILogger logger;
		public IDataCacheManager dataCache;

		public SensorController(ILogger logger, IDataCacheManager dataCache)
		{
			this.logger = logger;
			this.dataCache = dataCache;
		}

		// ping request
		[HttpGet]
		public String homeGet()
		{
			return "NOT DEFAULT hello world ... (it works ... ) ";
		}

		[HttpGet("{index}")]
		[Route("range")]
		public IActionResult getRowsFrom([FromQuery] string sensorName, [FromQuery] int index)
		{

			logger.logMessage($"Data range request > sensor: {sensorName} index: {index} ");

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			CacheRecord sensorRecord = this.dataCache.GetSensorRecordsFrom(sensorName, index);
			if (sensorRecord != null &&
				sensorRecord.Records.Count > 0)
			{

				SensorDataRecords reqResult = new SensorDataRecords();
				reqResult.SensorName = sensorName;
				reqResult.RecordsCount = sensorRecord.Records.Count;

				foreach (string singleCsvRecord in sensorRecord.Records)
				{

					/*
					singleCsvRecord {
						value,value,value,value;
					}
					*/

					JObject singleJsonRecord = new JObject();

					string[] values = singleCsvRecord.Split(",");
					for (int i = 0; i < values.Length; i++)
					{
						singleJsonRecord[sensorRecord.Header[i]] = values[i];
					}

					reqResult.Records.Add(singleJsonRecord.ToString(Formatting.None));

				}

				return new OkObjectResult(reqResult);
			}

			return new BadRequestResult();
		}

	}

}
