using System;
using System.Linq;
using System.Net.NetworkInformation;
using CollectorService.Broker.Reporter.Reports;
using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Events
{

	// http requests report
	// sensor unavailable

	public class CollectorEvent
	{

		public const string eventSourceType = "collector";

		// attention
		// currently implemented as mac-address 
		// initialized from default constructor
		public string source { get; private set; }

		public DateTime time { get; private set; }

		public Report report { get; set; }

		public CollectorEvent()
		{
			// read mac-address
			this.source = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

			this.time = DateTime.Now;

		}

		public CollectorEvent(Report report) : this()
		{
			this.report = report;
		}

		public JObject toJson()
		{

			JObject retObj = new JObject();

			retObj["eventSourceType"] = CollectorEvent.eventSourceType;
			retObj["source"] = this.source;
			retObj["time"] = this.time;

			retObj["reportType"] = this.report.getReportType();
			retObj["report"] = this.report.toJson();

			return retObj;
		}


	}
}