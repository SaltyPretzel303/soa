using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommunicationModel.RestModels;
using MediatR;
using Newtonsoft.Json;
using SensorRegistry.Configuration;

namespace SensorRegistry.MediatorRequests
{
	public class CheckSensorInfoRequest : IRequest<SensorReaderInfo>
	{
		public string SensorName { get; set; }
		public string SensorIp { get; set; }
		public int SensorPort { get; set; }

		public CheckSensorInfoRequest(
			string sensorName,
			string sensorIp,
			int sensorPort)
		{
			SensorName = sensorName;
			SensorIp = sensorIp;
			SensorPort = sensorPort;
		}

	}

	public class CheckSensorInfoRequestHandler
		: IRequestHandler<CheckSensorInfoRequest, SensorReaderInfo>
	{

		private ServiceConfiguration config;

		public CheckSensorInfoRequestHandler()
		{
			this.config = ServiceConfiguration.Instance;
		}

		public async Task<SensorReaderInfo> Handle(
			CheckSensorInfoRequest request,
			CancellationToken token)
		{

			string uri = $"http://{request.SensorIp}:{request.SensorPort}"
				+ $"/{config.sensorInfoPath}?"
				+ $"{config.sensorInfoNameArg}={request.SensorName}";

			Console.WriteLine("Gonna check that sensor ... " + uri);

			HttpClient httpClient = new HttpClient();
			HttpResponseMessage response = null;
			try
			{
				response = await httpClient.GetAsync(uri);
			}
			catch (HttpRequestException)
			// The request failed due to an underlying issue such as network connectivity, 
			// DNS failure, server certificate validation or timeout.
			{
				Console.WriteLine($"Checked sensor "
					+ $"({request.SensorName}@{request.SensorIp}:{request.SensorPort}) "
					+ $"is actually dead ... ");

				return null;
			}
			catch (Exception)
			{
				Console.WriteLine("Exception while getting sensor info ... ");
				return null;
			}

			if (response == null || !response.IsSuccessStatusCode)
			{
				Console.WriteLine("We got empty or bad response as an sensorInfo ... ");
				return null;
			}

			Console.WriteLine("Checked sensor "
				+ $"({request.SensorName}@{request.SensorIp}:{request.SensorPort}) "
				+ "is actually live ... ");

			string txtResponse = await response
				.Content
				.ReadAsStringAsync();

			var objResponse =
				JsonConvert.DeserializeObject<SensorReaderInfo>(txtResponse);

			return objResponse;

		}

	}

}