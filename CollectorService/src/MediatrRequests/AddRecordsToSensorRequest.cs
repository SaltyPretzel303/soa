using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CollectorService.Data;
using CommunicationModel;
using MediatR;
using Newtonsoft.Json.Linq;

namespace CollectorService.MediatrRequests
{
	public class AddRecordsToSensorRequest : IRequest<bool>
	{
		public String SensorName { get; private set; }
		public string CsvHeader { get; private set; }
		public List<string> CsvValues { get; private set; }

		public AddRecordsToSensorRequest(
			string sensorName,
			string csvHeader,
			List<string> csvValues)
		{
			this.SensorName = sensorName;
			this.CsvHeader = csvHeader;
			this.CsvValues = csvValues;
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
			List<SensorValues> objValues = request.CsvValues.Select((csvLine) =>
			{
				return ParseCsv(request.CsvHeader, csvLine);
			})
			.ToList();

			return await database.AddToSensor(request.SensorName, objValues);
		}

		private SensorValues ParseCsv(string header, string row)
		{
			string[] values = row.Split(",");

			JObject jObj = new JObject();
			int index = 0;

			foreach (string field in header.Split(","))
			{
				jObj[field] = values[index];
				index++;
			}

			return jObj.ToObject<SensorValues>();
		}


	}

}