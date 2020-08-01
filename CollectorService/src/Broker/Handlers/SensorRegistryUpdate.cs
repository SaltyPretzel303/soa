using System;
using CommunicationModel;
using CommunicationModel.BrokerModels;

namespace CollectorService.Broker.Events
{

	public interface ISensorRegistryUpdate
	{
		void HandleRegistryUpdate(SensorRegistryEvent registryEvent);
	}

	public class SensorRegistryUpdateHandler : ISensorRegistryUpdate
	{

		private IRegistryCache localRegistry;

		public SensorRegistryUpdateHandler(IRegistryCache localRegistry)
		{
			this.localRegistry = localRegistry;
		}

		public void HandleRegistryUpdate(SensorRegistryEvent registryEvent)
		{
			if (registryEvent.eventType == SensorRegEventType.NewSensor)
			{
				Console.WriteLine("Registry update: add");
				this.localRegistry.AddNewRecord(registryEvent.sensorRecord);
			}
			else if (registryEvent.eventType == SensorRegEventType.SensorUpdated)
			{
				Console.WriteLine("Registry update: update");
				this.localRegistry.UpdateRecord(registryEvent.sensorRecord);
			}
			else if (registryEvent.eventType == SensorRegEventType.SensorRemoved)
			{
				Console.WriteLine("Registry update: remove");
				this.localRegistry.RemoveRecord(registryEvent.sensorRecord);
			}
		}
	}
}