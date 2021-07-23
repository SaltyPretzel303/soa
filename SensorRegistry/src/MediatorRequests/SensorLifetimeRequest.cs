using System;
using CommunicationModel.BrokerModels;
using MediatR;
using SensorRegistry.Logger;
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
		private ILogger logger;

		public SensorLifetimeRequestHandler(
			ISensorRegistry localRegistry,
			ILogger logger)
		{
			this.LocalRegistry = localRegistry;
			this.logger = logger;
		}

		protected override void Handle(SensorLifetimeRequest request)
		{
			RegistryResponse regResponse = LocalRegistry
				.getSensorRecord(request.NewEvent.SensorName);

			if (request.NewEvent.lifeStage == LifetimeStages.Startup)
			{

				if (regResponse.status == RegistryStatus.noSuchRecord)
				{
					LocalRegistry.addSensorRecord(request.NewEvent.SensorName,
						request.NewEvent.IpAddress,
						request.NewEvent.ListeningPort,
						request.NewEvent.LastReadIndex);
				}
				else
				{
					logger.LogMessage("Startup event for ALREADY registered sensor ... ");
				}

			}
			else
			{
				// shutdown event received

				if (regResponse.status == RegistryStatus.ok)
				{
					LocalRegistry.removeSensorRecord(request.NewEvent.SensorName);
				}
				else
				{
					logger.LogMessage("Shutdown event for NOT registered sensor ... ");
				}

			}

		}
	}

}