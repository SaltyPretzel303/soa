using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace CommunicationModel.BrokerModels
{

	public class ServiceEvent
	{
		public string source { get; set; }
		public DateTime time { get; set; }

		public string customMessage;

		public ServiceEvent(string customMessage)
		{
			this.source = NetworkInterface.
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