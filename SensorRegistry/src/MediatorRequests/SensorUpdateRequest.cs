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

		private ISensorRegistry localRegistry;

		public SensorUpdateRequestHandler(ISensorRegistry localRegistry)
		{
			this.localRegistry = localRegistry;
		}

		protected override void Handle(SensorUpdateRequest request)
		{
			RegistryResponse regResponse = this.localRegistry.getSensorRecord(request.NewEvent.SensorName);

			if (regResponse.status == RegistryStatus.ok)
			{
				regResponse.singleData.AvailableRecords = request.NewEvent.LastReadIndex;
				this.localRegistry.updateSensorRecord(regResponse.singleData.Name,
													regResponse.singleData.Address,
													regResponse.singleData.Port,
													regResponse.singleData.AvailableRecords);
			}
		}

	}
}