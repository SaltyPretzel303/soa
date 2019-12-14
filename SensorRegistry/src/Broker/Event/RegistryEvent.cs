using System;
using System.Linq;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using SensorRegistry.Broker.Event.Reports;

namespace SensorRegistry.Broker.Event
{
	public class RegistryEvent
	{

		// attention
		// currently implemented as mac-address 
		// initialized from default constructor
		public string source { get; private set; }

		public DateTime time { get; private set; }

		public RegistryReport report { get; set; }

		public RegistryEvent()
		{
			// read mac-address
			this.source = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

			this.time = DateTime.Now;

		}

		public RegistryEvent(RegistryReport report) : this()
		{
			this.report = report;
		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}


	}
}