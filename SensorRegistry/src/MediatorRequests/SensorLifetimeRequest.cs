using System;
using CommunicationModel.BrokerModels;
using MediatR;
using SensorRegistry.Registry;

namespace SensorRegistry.MediatorRequests
{
	public class SensorLifetimeRequest : IRequest
	{
		public SensorLifetimeEvent NewEvent { get; set; }

		public SensorLifetimeRequest(SensorLifetimeEvent newEvent)
		{
			NewEvent = newEvent;
		}
	}

	public class SensorLifetimeRequestHandler : RequestHandler<SensorLifetimeRequest>
	{

		private ISensorRegistry LocalRegistry;

		public SensorLifetimeRequestHandler(ISensorRegistry localRegistry)
		{
			LocalRegistry = localRegistry;
		}

		protected override void Handle(SensorLifetimeRequest request)
		{

			RegistryResponse regResponse = this.LocalRegistry
					.getSensorRecord(request.NewEvent.SensorName);

			if (request.NewEvent.lifeStage == LifetimeStages.Startup)
			{

				if (regResponse.status == RegistryStatus.noSuchRecord)
				{
					this.LocalRegistry.addSensorRecord(request.NewEvent.SensorName,
										request.NewEvent.IpAddress,
										request.NewEvent.ListeningPort,
										request.NewEvent.LastReadIndex);
				}
				else
				{
					// TODO use logger
					Console.WriteLine("Startup event for ALREADY registered sensor ... ");
				}

			}
			else
			{
				// shutdown event received

				if (regResponse.status == RegistryStatus.ok)
				{
					this.LocalRegistry.removeSensorRecord(request.NewEvent.SensorName);
				}
				else
				{
					// TODO use logger
					Console.WriteLine("Shutdown event for NOT registered sensor ... ");
				}

			}

		}
	}

}