using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CollectorService.Data;
using CommunicationModel;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class AddRecordsToSensorRequest : IRequest<bool>
	{
		public String SensorName { get; private set; }
		public List<SensorValues> Values { get; private set; }

		public AddRecordsToSensorRequest(string sensorName, List<SensorValues> values)
		{
			SensorName = sensorName;
			Values = values;
		}

	}

	public class AddRecordsToSensorRequestHandler
		: IRequestHandler<AddRecordsToSensorRequest, bool>
	{

		private IDatabaseService database;

		public AddRecordsToSensorRequestHandler(IDatabaseService database)
		{
			this.database = database;
		}

		public async Task<bool> Handle(AddRecordsToSensorRequest request,
			CancellationToken token)
		{
			return await database.AddToSensor(request.SensorName, request.Values);
		}
	}
}