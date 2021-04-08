using System;
using CommunicationModel.BrokerModels;
using MediatR;
using SensorRegistry.Registry;

namespace SensorRegistry.MediatorRequests
{
	public class SensorUpdateRequest : IRequest
	{
		public SensorReaderEvent NewEvent { get; set; }

		public SensorUpdateRequest(SensorReaderEvent newEvent)
		{
			NewEvent = newEvent;
		}
	}

	public class SensorUpdateRequestHandler : RequestHandler<SensorUpdateRequest>
	{

		private ISensorRegistry LocalRegistry;
		private IMediator mediator;

		public SensorUpdateRequestHandler(ISensorRegistry localRegistry,
			IMediator mediator)
		{
			this.LocalRegistry = localRegistry;
			this.mediator = mediator;
		}

		protected override void Handle(SensorUpdateRequest request)
		{
			RegistryResponse regResponse = LocalRegistry
					.getSensorRecord(request.NewEvent.SensorName);

			if (regResponse.status == RegistryStatus.ok)
			{
				LocalRegistry.updateSensorRecord(
					request.NewEvent.SensorName,
					request.NewEvent.IpAddress,
					request.NewEvent.ListeningPort,
					request.NewEvent.LastReadIndex);
			}
			else if (regResponse.status == RegistryStatus.noSuchRecord)
			{

				// this request will check if that sensor is still alive
				// and add it to the registry if it is
				mediator.Send(new CheckSensorInfoRequest(
					request.NewEvent.SensorName,
					request.NewEvent.IpAddress,
					request.NewEvent.ListeningPort));

				// LocalRegistry.addSensorRecord(
				// 	request.NewEvent.SensorName,
				// 	request.NewEvent.IpAddress,
				// 	request.NewEvent.ListeningPort,
				// 	request.NewEvent.LastReadIndex);
			}
		}

	}
}