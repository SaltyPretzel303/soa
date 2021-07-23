using System;
using Microsoft.AspNetCore.Mvc;
using SensorService.Configuration;
using SensorService.Logger;
using CommunicationModel;
using CommunicationModel.RestModels;

namespace DataCollector.Controller
{
	// port: 5001
	[Route("sensor/data")]
	[ApiController]
	public class SensorController : ControllerBase
	{

		private ILogger logger;
		private IDataCacheManager dataCache;

		private ServiceConfiguration config;

		public SensorController(ILogger logger, IDataCacheManager dataCache)
		{
			this.logger = logger;
			this.dataCache = dataCache;

			this.config = ServiceConfiguration.Instance;
		}

		// ping request
		[HttpGet]
		public String homeGet()
		{
			return "NOT DEFAULT hello world ... (it works ... ) ";
		}

		[HttpGet("{index}")]
		[Route("range")]
		public IActionResult getRowsFrom(
			[FromQuery] string sensorName,
			[FromQuery] int index)
		{

			logger.logMessage($"Data range request - sensor: {sensorName} index: {index} ");

			CacheRecord cachedRecord =
				this.dataCache.GetSensorRecordsFrom(sensorName, index);
			if (cachedRecord != null &&
				cachedRecord.CsvRecords.Count > 0)
			{

				var reqResult = new SensorDataRecords(
					sensorName,
					cachedRecord.CsvRecords.Count,
					cachedRecord.CsvHeader,
					cachedRecord.CsvRecords);

				return new OkObjectResult(reqResult);
			}

			return new BadRequestResult();
		}

		[HttpGet]
		[Route("info")]
		public IActionResult getInfo([FromQuery] string sensorName)
		{

			var info = new SensorReaderInfo(
				sensorName,
				config.hostIP,
				config.listeningPort,
				dataCache.GetLastReadIndex(sensorName));

			return new ObjectResult(info);
		}

		// TODO add config update method maybe 

	}

}
