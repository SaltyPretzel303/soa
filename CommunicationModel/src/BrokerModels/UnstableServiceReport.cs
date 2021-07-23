using System;
using System.Collections.Generic;

namespace CommunicationModel.BrokerModels
{
	public class UnstableServiceReport : ServiceEvent
	{
		public ServiceObserverResultType reportType;
		public int downCount { get; set; }
		public List<ServiceLifetimeEvent> downEvents { get; set; }

		public UnstableServiceReport(
			string serviceId,
			ServiceObserverResultType reportType,
			int downCount,
			List<ServiceLifetimeEvent> downEvents)

			: base(serviceId, ServiceType.ServiceObserver, "")
		{
			this.reportType = reportType;
			this.downCount = downCount;
			this.downEvents = downEvents;
		}
	}
}