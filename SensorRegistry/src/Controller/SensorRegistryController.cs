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

		private ISensorRegistry sensorRegistry;
		private IMessageBroker broker;

		public SensorRegistryController(
			ISensorRegistry sensorRegistry,
			IMessageBroker broker)
		{
			this.sensorRegistry = sensorRegistry;
			this.broker = broker;
		}

		[HttpPost]
		[Route("addSensor")]
		public IActionResult postSensor([FromBody] SensorDataArg reqArg)
		{

			Console.WriteLine($"Sensor add trough post request: {reqArg.SensorName}");

			RegistryResponse response = sensorRegistry.addSensorRecord(reqArg.SensorName,
														reqArg.IpAddress,
														reqArg.PortNum,
														reqArg.LastReadIndex);

			if (response.status == RegistryStatus.ok)
			{
				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}

		[HttpPost]
		[Route("updateSensor")]
		public IActionResult updateSensor([FromBody] SensorDataArg reqArg)
		{

			ServiceConfiguration conf = ServiceConfiguration.Instance;

			RegistryResponse response = sensorRegistry.updateSensorRecord(reqArg.SensorName,
															reqArg.IpAddress,
															reqArg.PortNum,
															reqArg.LastReadIndex);

			if (response.status == RegistryStatus.ok)
			{
				return new OkObjectResult(response.singleData);
			}

			return new BadRequestObjectResult("Registry response: " + response.status.ToString());

		}

		[HttpDelete]
		[Route("deleteSensor")]
		public IActionResult delete([FromQuery] string sensorName)
		{

			Console.WriteLine($"Sensor removed: {sensorName} ... ");
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
				return new OkObjectResult(response.singleData.Address);
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

		[HttpGet]
		[Route("getSensor")]
		public IActionResult getSensors([FromQuery] string sensorName)
		{
			RegistryResponse response = this.sensorRegistry.getSensorRecord(sensorName);

			if (response.status == RegistryStatus.ok)
			{
				return new OkObjectResult(response.singleData);
			}

			return new BadRequestResult();
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