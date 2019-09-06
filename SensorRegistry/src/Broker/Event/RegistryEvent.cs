using System;
using System.Linq;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using SensorRegistry.Broker.Event.Reports;

namespace SensorRegistry.Broker.Event
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

			JObject retObj = new JObject();

			retObj["eventSourceType"] = RegistryEvent.eventSourceType;
			retObj["source"] = this.source;
			retObj["time"] = this.time;

			retObj["reportType"] = this.report.getReportType();
			retObj["report"] = this.report.toJson();

			return retObj;
		}


	}
}