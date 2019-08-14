using System;
using System.Linq;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Events
{

	// http requests report
	// sensor unavailable

	public class CollectorEvent
	{

		public const string eventType = "collector_event";

		// attention
		// currently implemented as mac-address 
		// initialized from default constructor
		public string source { get; private set; }

		public DateTime time { get; private set; }

		public JObject jsonContent { get; set; }

		public CollectorEvent()
		{
			// read mac-address
			this.source = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

			this.time = DateTime.Now;

			this.jsonContent = new JObject();

		}

		public CollectorEvent(JObject jsonContent) : this()
		{
			this.jsonContent = jsonContent;
		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}


	}
}