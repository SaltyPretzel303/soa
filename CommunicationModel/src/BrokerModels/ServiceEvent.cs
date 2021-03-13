using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace CommunicationModel.BrokerModels
{

	public class ServiceEvent
	{
		// mac address 
		public string sourceId { get; set; }
		public ServiceType sourceType { get; set; }

		public DateTime time { get; set; }

		public string customMessage;

		public ServiceEvent(ServiceType type, string customMessage)
		{
			this.sourceId = NetworkInterface.
							GetAllNetworkInterfaces().
							Where((nic) =>
							{
								return nic.OperationalStatus == OperationalStatus.Up &&
								nic.NetworkInterfaceType != NetworkInterfaceType.Loopback;
							}).
							Select((nic) => { return nic.GetPhysicalAddress().ToString(); }).
							FirstOrDefault();
			this.time = DateTime.Now;
			this.customMessage = customMessage;
		}

	}
}