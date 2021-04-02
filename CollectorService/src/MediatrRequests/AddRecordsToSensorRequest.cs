using System;
using System.Collections.Generic;
using CollectorService.Data;
using CommunicationModel;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class AddRecordsToSensorRequest : IRequest
	{
		public String SensorName { get; private set; }
		public List<SensorValues> Values { get; private set; }

		public AddRecordsToSensorRequest(string sensorName, List<SensorValues> values)
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
			this.database.addToSensor(request.SensorName, request.Values);
		}
	}
}