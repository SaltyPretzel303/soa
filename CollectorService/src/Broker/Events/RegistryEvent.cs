using System;
using System.Linq;
using System.Net.NetworkInformation;
using CollectorService.src.Broker.Reporter.Reports.Registry;
using Newtonsoft.Json.Linq;

namespace CollectorService.src.Broker.Events
{
	public class RegistryEvent
	{
		public const string eventSourceType = "registry";

		// attention
		// currently implemented as mac-address 
		// initialized from default constructor
		public string source { get; private set; }

		public DateTime time { get; private set; }

		public Report report { get; set; }

		public RegistryEvent()
		{
			// read mac-address
			this.source = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

			this.time = DateTime.Now;

		}

		public RegistryEvent(Report report) : this()
		{
			this.report = report;
			Console.WriteLine("Report assigned: " + this.report.toJson().ToString());
		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}

	}
}