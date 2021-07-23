using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace CommunicationModel.BrokerModels
{

	public class ServiceEvent
	{
		// read from configuration
		public string sourceId { get; set; }

		public ServiceType sourceType { get; set; }

		public DateTime time { get; set; }

		public string customMessage { get; set; }

		// public ServiceEvent()
		// {

		// }

		public ServiceEvent(
			String serviceId,
			ServiceType type,
			string customMessage)
		{
			// should be removed 
			// left just as an example how to get mac address of networkDevice
			// if (sourceId == null)
			// {
			// 	this.sourceId = NetworkInterface.
			// 					GetAllNetworkInterfaces().
			// 					Where((nic) =>
			// 					{
			// 						return nic.OperationalStatus == OperationalStatus.Up &&
			// 						nic.NetworkInterfaceType != NetworkInterfaceType.Loopback;
			// 					}).
			// 					Select((nic) => { return nic.GetPhysicalAddress().ToString(); }).
			// 					FirstOrDefault();
			// }
			// else
			// {
			// 	this.sourceId = sourceId;
			// }

			this.sourceId = serviceId;
			this.time = DateTime.Now;
			this.sourceType = type;
			this.customMessage = customMessage;
		}

	}
}