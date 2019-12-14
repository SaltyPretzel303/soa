using System.ComponentModel.Design;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorRegistry.Broker;
using SensorRegistry.Broker.Event;
using SensorRegistry.Broker.Event.Reports;
using SensorRegistry.Registry;
using SensorRegistry.Configuration;

namespace SensorRegistry.Controller
{
	// port 5005
	[ApiController]
	[Route("sensor/registry")]
	public class SensorRegistryController
	{

		private IHttpContextAccessor httpContext;
		private ISensorRegistry sensorRegistry;
		private MessageBroker broker;

		public SensorRegistryController(IHttpContextAccessor httpContext, ISensorRegistry sensorRegistry)
		{
			this.httpContext = httpContext;
			this.sensorRegistry = sensorRegistry;

			this.broker = MessageBroker.Instance;

		}

		// TODO should be post
		// problem, extract 2 arguments (1 is ok ... ) 
		[HttpGet]
		[Route("registerSensor")]
		public IActionResult registerSensor([FromQuery] string sensorName, [FromQuery] int portNum)
		{

			// sensor ip is extracted from request
			string sensorIp = this.httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

			Console.WriteLine($"Request to register sensor: {sensorName}, with: {sensorIp}:{portNum}");

			RegistryResponse response = this.sensorRegistry.addSensorRecord(sensorName, sensorIp, portNum);

			if (response.status == RegistryStatus.ok)
			{

				RegistryReport report = new RegistryReport(RegistryReportType.SensorRegistration, response.singleData);

				ServiceConfiguration conf = ServiceConfiguration.read();

				this.broker.publishEvent(new RegistryEvent(report), conf.newSensorFilter);

				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());
		}

		// TODO should be post
		// problem, same as with registerSenosor
		[HttpGet]
		[Route("updateSensor")]
		public IActionResult updateSensor([FromQuery]string sensorName, [FromQuery] string sensorAddr, [FromQuery] int portNum)
		{

			Console.WriteLine($"Request for updating sensor: {sensorName}, with: {sensorAddr}:{portNum}");

			RegistryResponse response = this.sensorRegistry.changeSensorRecord(sensorName, sensorAddr, portNum);
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

				RegistryReport report = new RegistryReport(RegistryReportType.SensorUnregitration, response.singleData);

				ServiceConfiguration conf = ServiceConfiguration.read();

				this.broker.publishEvent(new RegistryEvent(report), conf.sensorRemovedFilter);

				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}

		[HttpGet]
		[Route("getAddress")]
		public IActionResult getAddress([FromQuery]string sensorName)
		{

			RegistryResponse response = this.sensorRegistry.getSensorAddr(sensorName);
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

				// returns list of sensorRecords
				return new OkObjectResult(response.listData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}



	}
}