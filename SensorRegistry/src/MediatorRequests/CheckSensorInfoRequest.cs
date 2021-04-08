using System;
using System.Net.Http;
using CommunicationModel.RestModels;
using MediatR;
using Newtonsoft.Json;
using SensorRegistry.Configuration;
using SensorRegistry.Registry;

namespace SensorRegistry.MediatorRequests
{
	public class CheckSensorInfoRequest : IRequest
	{
		public string SensorName { get; set; }
		public string SensorIp { get; set; }
		public int SensorPort { get; set; }

		public CheckSensorInfoRequest(string sensorName, string sensorIp, int sensorPort)
		{
			SensorName = sensorName;
			SensorIp = sensorIp;
			SensorPort = sensorPort;
		}

	}

	public class CheckSensorInfoRequestHandler : RequestHandler<CheckSensorInfoRequest>
	{

		private ISensorRegistry localRegistry;
		private ServiceConfiguration config;

		public CheckSensorInfoRequestHandler(ISensorRegistry localRegistry)
		{
			this.localRegistry = localRegistry;
			this.config = ServiceConfiguration.Instance;
		}

		protected override void Handle(CheckSensorInfoRequest request)
		{

			string uri = $"http://{request.SensorIp}:{request.SensorPort}"
				+ $"/{config.sensorInfoPath}?"
				+ $"{config.sensorInfoNameArg}={request.SensorName}";

			Console.WriteLine("Gonna check that sensor ... " + uri);

			HttpClient httpClient = new HttpClient();
			HttpResponseMessage response = null;
			try
			{
				response = httpClient.GetAsync(uri).Result;
			}
			catch (HttpRequestException)
			// The request failed due to an underlying issue such as network connectivity, 
			// DNS failure, server certificate validation or timeout.
			{
				Console.WriteLine($"Checked sensor "
					+ $"({request.SensorName}@{request.SensorIp}:{request.SensorPort}) "
					+ $"is actually dead ... ");

				return;
			}
			catch (Exception)
			{
				Console.WriteLine("Exception while getting sensor info ... ");
				return;
			}

			if (response == null || !response.IsSuccessStatusCode)
			{
				Console.WriteLine("We got empty or bad response as an sensorInfo ... ");
				return;
			}

			Console.WriteLine("Checked sensor "
				+ $"({request.SensorName}@{request.SensorIp}:{request.SensorPort}) "
				+ "is actually live ... ");

			string txtResponse = response
				.Content
				.ReadAsStringAsync()
				.Result;

			SensorReaderInfo objResponse =
				JsonConvert.DeserializeObject<SensorReaderInfo>(txtResponse);

			localRegistry.addSensorRecord(
				objResponse.SensorName,
				objResponse.IpAddress,
				objResponse.ListeningPort,
				objResponse.LastReadIndex);

		}

	}

}