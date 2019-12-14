using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Reporter.Reports.Collector
{
	public class LifeCycleReport : Report
	{

		public const string reportType = "life_cycle_report";

		public string reportContent;

		public LifeCycleReport(string content)
		{

			this.reportContent = content;

		}

		public string getReportType()
		{
			return "life_cycle_report";
		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}

	}
}