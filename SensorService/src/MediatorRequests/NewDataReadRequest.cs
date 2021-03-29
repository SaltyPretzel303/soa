using System;
using System.Collections.Generic;
using CommunicationModel.BrokerModels;
using MediatR;
using SensorService.Broker;
using SensorService.Configuration;

namespace SensorService.MediatorRequests
{
	public class NewDataReadRequest : IRequest
	{
		public string SensorName { get; set; }

		public List<string> Header { get; set; }
		public int ReadIndex { get; set; }
		public string NewData { get; set; }

		public NewDataReadRequest(string sensorName,
					List<string> header,
					int readIndex,
					string newData)
		{
			SensorName = sensorName;
			Header = header;
			ReadIndex = readIndex;
			NewData = newData;
		}
	}

	public class NewDataReadRequestHandler : RequestHandler<NewDataReadRequest>
	{

		private IDataCacheManager cache;
		private IMessageBroker broker;

		private ServiceConfiguration config;

		public NewDataReadRequestHandler(IDataCacheManager cache, IMessageBroker broker)
		{
			this.cache = cache;
			this.broker = broker;

			this.config = ServiceConfiguration.Instance;
		}

		protected override void Handle(NewDataReadRequest request)
		{

			this.cache.AddData(request.SensorName,
							request.Header,
							request.NewData);

			this.broker.PublishSensorEvent(new SensorReaderEvent(request.SensorName,
													request.Header,
													request.ReadIndex,
													request.NewData,
													config.hostIP,
													config.listeningPort),
								config.sensorReadEventFilter);
		}
	}

}