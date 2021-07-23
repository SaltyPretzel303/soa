using System;
using System.Threading;
using System.Threading.Tasks;
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

	public class SensorUpdateRequestHandler
		: AsyncRequestHandler<SensorUpdateRequest>
	{

		private ISensorRegistry LocalRegistry;
		private IMediator mediator;

		public SensorUpdateRequestHandler(
			ISensorRegistry localRegistry,
			IMediator mediator)
		{
			this.LocalRegistry = localRegistry;
			this.mediator = mediator;
		}

		protected override async Task Handle(
			SensorUpdateRequest request,
			CancellationToken cancellationToken)
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
				// we received event from sensor that is still not registered

				// this request will check if that sensor is still alive
				// and return it's info if it is alive

				var result = await mediator.Send(new CheckSensorInfoRequest(
					request.NewEvent.SensorName,
					request.NewEvent.IpAddress,
					request.NewEvent.ListeningPort));

				if (result != null)
				{
					LocalRegistry.addSensorRecord(
						result.SensorName,
						result.IpAddress,
						result.ListeningPort,
						result.LastReadIndex);
				}

			}

			return;
		}

	}
}