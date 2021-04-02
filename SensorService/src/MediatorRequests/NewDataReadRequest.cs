using System;
using System.Collections.Generic;
using CommunicationModel;
using CommunicationModel.BrokerModels;
using MediatR;
using SensorService.Broker;
using SensorService.Configuration;

namespace SensorService.MediatorRequests
{
	public class StoreSensorDataRequest : IRequest
	{
		public string SensorName { get; set; }

		public List<string> Header { get; set; }
		public int ReadIndex { get; set; }
		public SensorValues NewData { get; set; }

		public StoreSensorDataRequest(string sensorName,
					List<string> header,
					int readIndex,
					SensorValues newData)
		{
			SensorName = sensorName;
			Header = header;
			ReadIndex = readIndex;
			NewData = newData;
		}
	}

	public class StoreSensorDataRequestHandler : RequestHandler<StoreSensorDataRequest>
	{

		private IDataCacheManager cache;
		private IMessageBroker broker;

		private ServiceConfiguration config;

		public StoreSensorDataRequestHandler(IDataCacheManager cache, IMessageBroker broker)
		{
			this.cache = cache;
			this.broker = broker;

			this.config = ServiceConfiguration.Instance;
		}

		protected override void Handle(StoreSensorDataRequest request)
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