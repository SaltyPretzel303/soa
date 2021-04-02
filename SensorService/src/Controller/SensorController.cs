using System;
using Microsoft.AspNetCore.Mvc;
using SensorService.Configuration;
using SensorService.Logger;
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

			logger.logMessage($"Data range request - sensor: {sensorName} index: {index} ");

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			CacheRecord cachedRecord = this.dataCache.GetSensorRecordsFrom(sensorName, index);
			if (cachedRecord != null &&
				cachedRecord.Records.Count > 0)
			{

				SensorDataRecords reqResult = new SensorDataRecords(
							sensorName,
							cachedRecord.Records.Count,
							cachedRecord.Records);

				return new OkObjectResult(reqResult);
			}

			return new BadRequestResult();
		}

		// TODO add config update method maybe 

	}

}
