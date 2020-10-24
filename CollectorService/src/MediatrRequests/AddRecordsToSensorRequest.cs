using System;
using CollectorService.Data;
using MediatR;
using Newtonsoft.Json.Linq;

namespace CollectorService.MediatrRequests
{
	public class AddRecordsToSensorRequest : IRequest
	{
		public String SensorName { get; private set; }
		public JArray Values { get; private set; }

		public AddRecordsToSensorRequest(string sensorName, JArray values)
		{
			SensorName = sensorName;
			Values = values;
		}

	}

	public class AddRecordsToSensorRequestHandler : RequestHandler<AddRecordsToSensorRequest>
	{

		private IDatabaseService database;

		public AddRecordsToSensorRequestHandler(IDatabaseService database)
		{
			this.database = database;
		}

		protected override void Handle(AddRecordsToSensorRequest request)
		{
			this.database.pushToSensor(request.SensorName, request.Values);
		}
	}
}