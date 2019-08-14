using Newtonsoft.Json.Linq;

namespace CollectorService.src.Broker.Reporter.Report
{
	public class LifeCycleReport
	{

		public const string reportType = "life_cycle_report";

		public string reportContent;

		public LifeCycleReport(string content)
		{

			this.reportContent = content;

		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}

	}
}