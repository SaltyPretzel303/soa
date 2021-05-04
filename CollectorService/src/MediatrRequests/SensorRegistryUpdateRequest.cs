using System;
using CommunicationModel.BrokerModels;
using MediatR;

namespace CollectorService.MediatrRequests
{
	public class SensorRegistryUpdateRequest : IRequest
	{
		public SensorRegistryEvent registryEvent { get; private set; }

		public SensorRegistryUpdateRequest(SensorRegistryEvent registryEvent)
		{
			this.registryEvent = registryEvent;
		}
	}

	public class SensorRegistryUpdateRequestHandler
		: RequestHandler<SensorRegistryUpdateRequest>
	{

		private IRegistryCache localRegistry;

		public SensorRegistryUpdateRequestHandler(IRegistryCache registryCache)
		{
			this.localRegistry = registryCache;
		}

		protected override void Handle(SensorRegistryUpdateRequest request)
		{
			SensorRegistryEvent registryEvent = request.registryEvent;

			if (registryEvent.eventType == SensorRegEventType.NewSensor)
			{
				Console.WriteLine("Registry update: add");
				localRegistry.AddNewRecord(registryEvent.sensorRecord);
			}
			else if (registryEvent.eventType == SensorRegEventType.SensorUpdated)
			{
				// there is gonna be a lot of this so lets just comment them ... 
				// Console.WriteLine("Registry update: update");
				localRegistry.UpdateRecord(registryEvent.sensorRecord);
			}
			else if (registryEvent.eventType == SensorRegEventType.SensorRemoved)
			{
				Console.WriteLine("Registry update: remove");
				localRegistry.RemoveRecord(registryEvent.sensorRecord);
			}
		}
	}

}