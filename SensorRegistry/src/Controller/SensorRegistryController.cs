using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorRegistry.Broker;
using SensorRegistry.Broker.Event;
using SensorRegistry.Broker.Event.Reports;
using SensorRegistry.Registry;

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

		[HttpGet]
		[Route("registerSensor")]
		public IActionResult registerSensor([FromQuery] string sensorName, [FromQuery] int portNum)
		{

			string sensorIp = this.httpContext.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

			Console.WriteLine($"Request to register sensor: {sensorName}, with: {sensorIp}:{portNum}");

			RegistryResponse response = this.sensorRegistry.addSensorRecord(sensorName, sensorIp, portNum);

			if (response.status == RegistryStatus.ok)
			{

				Report report = new RegistrationReport(sensorName, sensorIp, portNum);

				this.broker.publishEvent(new RegistryEvent(report));
				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());
		}

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

			RegistryResponse response = this.sensorRegistry.removeSensorRecord(sensorName);
			if (response.status == RegistryStatus.ok)
			{

				Report report = new UnregistrationReport(sensorName, response.singleData.address, response.singleData.port);

				this.broker.publishEvent(new RegistryEvent(report));

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

				return new OkObjectResult(response.listData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}



	}
}