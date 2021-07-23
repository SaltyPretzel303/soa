using System;

namespace CommunicationModel.BrokerModels
{
	// NOTE this class should be removed
	// it is indented to be root class for all serviceObserver events
	// but all these fields are the same as in the serviceEvent
	public class ServiceObserverReport : ServiceEvent
	{
		public ServiceObserverResultType reportType;

		public ServiceObserverReport(
			string serviceId,
			DateTime recordedTime,
			ServiceObserverResultType reportType)
			: base(serviceId, ServiceType.ServiceObserver, "")
		{
			this.reportType = reportType;
		}
	}
}