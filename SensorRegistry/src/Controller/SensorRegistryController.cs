using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorRegistry.Broker;
using SensorRegistry.Registry;
using SensorRegistry.Configuration;
using CommunicationModel.RestModels;
using Newtonsoft.Json.Linq;

namespace SensorRegistry.Controller
{
	// dev port 5002
	// prod port 5000
	[ApiController]
	[Route("sensor/registry")]
	public class SensorRegistryController
	{

		// used to get ip and port of request source
		private IHttpContextAccessor httpContext;

		private ISensorRegistry sensorRegistry;
		private IMessageBroker broker;

		public SensorRegistryController(IHttpContextAccessor httpContext,
									ISensorRegistry sensorRegistry,
									IMessageBroker broker)
		{
			this.httpContext = httpContext;
			this.sensorRegistry = sensorRegistry;

			this.broker = broker;
		}

		// TODO should be post
		// problem, extract 2 arguments (1 is ok ... ) 
		[HttpGet]
		[Route("registerSensor")]
		public IActionResult registerSensor([FromQuery] string sensorName,
										[FromQuery] int portNum,
										[FromQuery] int lastReadIndex)
		{

			// sensor ip is extracted from request
			string sensorIp = this.httpContext.
								HttpContext.
								Connection.
								RemoteIpAddress.
								MapToIPv4().
								ToString();

			Console.WriteLine($"Request to register sensor: {sensorName}, with: {sensorIp}:{portNum}");

			RegistryResponse response = this.sensorRegistry.addSensorRecord(sensorName,
																		sensorIp,
																		portNum,
																		lastReadIndex);
			if (response.status == RegistryStatus.ok)
			{
				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());
		}

		// TODO should be post
		// problem, same as with registerSenosor
		[HttpGet]
		[Route("updateSensor")]
		public IActionResult updateSensor([FromQuery] string sensorName,
										[FromQuery] string sensorAddr,
										[FromQuery] int portNum,
										[FromQuery] int lastReadIndex)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			RegistryResponse response = this.sensorRegistry.updateSensorRecord(sensorName,
																			sensorAddr,
																			portNum,
																			lastReadIndex);
			if (response.status == RegistryStatus.ok)
			{
				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}

		[HttpGet]
		[Route("unregisterSensor")]
		public IActionResult unregisterSensor([FromQuery] string sensorName)
		{

			string sensorIp = this.httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

			Console.WriteLine($"Request to unregister sensor: {sensorName}, with {sensorIp}");
			RegistryResponse response = this.sensorRegistry.removeSensorRecord(sensorName);
			if (response.status == RegistryStatus.ok)
			{
				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}

		[HttpGet]
		[Route("getAddress")]
		public IActionResult getAddress([FromQuery] string sensorName)
		{

			RegistryResponse response = this.sensorRegistry.getSensorRecord(sensorName);
			if (response.status == RegistryStatus.ok)
			{

				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}

		[HttpGet]
		[Route("getSensors")]
		public IActionResult getSensors()
		{

			RegistryResponse response = this.sensorRegistry.getAllSensors();
			if (response.status == RegistryStatus.ok)
			{
				return new OkObjectResult(response.listData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}

		[HttpPost]
		[Route("updateConfig")]
		public IActionResult updateConfig([FromBody] UpdateConfigArg configArg)
		{

			JObject jsonConfig = JObject.Parse(configArg.TxtConfig);
			ServiceConfiguration.Instance.UpdateConfig(jsonConfig);

			return new OkResult();
		}

	}

}