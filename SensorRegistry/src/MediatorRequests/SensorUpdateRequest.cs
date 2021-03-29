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

		public SensorUpdateRequestHandler(ISensorRegistry localRegistry)
		{
			this.LocalRegistry = localRegistry;
		}

		protected override void Handle(SensorUpdateRequest request)
		{
			RegistryResponse regResponse = this.LocalRegistry
					.getSensorRecord(request.NewEvent.SensorName);

			if (regResponse.status == RegistryStatus.ok)
			{
				regResponse.singleData.AvailableRecords = request.NewEvent.LastReadIndex;
				this.LocalRegistry.updateSensorRecord(regResponse.singleData.Name,
										regResponse.singleData.Address,
										regResponse.singleData.Port,
										regResponse.singleData.AvailableRecords);
			}
			else if (regResponse.status == RegistryStatus.noSuchRecord)
			{
				this.LocalRegistry.addSensorRecord(request.NewEvent.SensorName,
										request.NewEvent.IpAddress,
										request.NewEvent.ListeningPort,
										request.NewEvent.LastReadIndex);
			}
		}

	}
}