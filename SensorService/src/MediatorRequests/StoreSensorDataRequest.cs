using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunicationModel;
using CommunicationModel.BrokerModels;
using MediatR;
using Newtonsoft.Json.Linq;
using SensorService.Broker;
using SensorService.Configuration;

namespace SensorService.MediatorRequests
{
	public class StoreSensorDataRequest : IRequest
	{
		public string SensorName { get; set; }

		public string CsvHeader { get; set; }
		public int ReadIndex { get; set; }
		public string CsvValues { get; set; }

		public StoreSensorDataRequest(
			string sensorName,
			string csvHeader,
			int readIndex,
			string csvValues)
		{
			SensorName = sensorName;
			CsvHeader = csvHeader;
			ReadIndex = readIndex;
			CsvValues = csvValues;
		}
	}

	public class StoreSensorDataRequestHandler
		: AsyncRequestHandler<StoreSensorDataRequest>
	{

		private IDataCacheManager cache;
		private IMessageBroker broker;

		private ServiceConfiguration config;

		public StoreSensorDataRequestHandler(
			IDataCacheManager cache,
			IMessageBroker broker)
		{
			this.cache = cache;
			this.broker = broker;

			this.config = ServiceConfiguration.Instance;
		}

		protected override async Task Handle(
			StoreSensorDataRequest request,
			CancellationToken cancellationToken)
		{
			this.cache.AddData(
				request.SensorName,
				request.CsvHeader,
				request.CsvValues);

			var headerList = request.CsvHeader.Split(",").ToList();
			var objValues = ParseCsv(headerList, request.CsvValues);

			await broker.PublishSensorEvent(
				new SensorReaderEvent(
					config.serviceId,
					request.SensorName,
					headerList,
					request.ReadIndex,
					objValues,
					config.hostIP,
					config.listeningPort),
				config.sensorReadEventFilter);
		}

		private SensorValues ParseCsv(List<string> header, string row)
		{
			string[] values = row.Split(",");

			JObject jObj = new JObject();
			int index = 0;

			foreach (string field in header)
			{
				jObj[field] = values[index];
				index++;
			}

			return jObj.ToObject<SensorValues>();
		}

	}

}