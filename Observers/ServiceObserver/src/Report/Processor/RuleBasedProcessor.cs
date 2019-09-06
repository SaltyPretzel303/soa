using System;
using System.Collections.Generic;
using ServiceObserver.Broker;
using ServiceObserver.Configuration;

namespace ServiceObserver.Report.Processor
{


	public class RuleBasedProcessor : ReportProcessor
	{

		private delegate ServiceEvent DServiceQosRule(ServiceReportEvent report);

		private Dictionary<string, DServiceQosRule> rules;

		public RuleBasedProcessor()
		{

			this.populateRules();

		}

		private void populateRules()
		{

			this.rules = new Dictionary<string, DServiceQosRule>();

			this.rules.Add("life_cycle_report", this.lifecycleRule);
			this.rules.Add("sensor_pull_report", this.sensorDataPullRule);
			this.rules.Add("access_report", this.apiAccessibilityRule);

		}


		public void processReport(ServiceReportEvent report)
		{

			DServiceQosRule rule = null;
			if (this.rules.TryGetValue(report.eventSourceType, out rule))
			{

				ServiceEvent foundEvent = rule(report);
				if (foundEvent != null)
				{
					// TODO create appropriate service event, pass it some arguments
					MessageBroker.Instance.publishServiceEvent(new ServiceEvent());

				}

			}
			else
			{

				Console.WriteLine("Unknown source event type received, eventSourceType: " + report.eventSourceType);

			}


		}

		private ServiceEvent lifecycleRule(ServiceReportEvent report)
		{
			return null;
		}

		private ServiceEvent sensorDataPullRule(ServiceReportEvent report)
		{
			return null;
		}

		private ServiceEvent apiAccessibilityRule(ServiceReportEvent report)
		{
			return null;
		}

	}
}