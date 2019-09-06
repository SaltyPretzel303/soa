using Newtonsoft.Json.Linq;

namespace ServiceObserver.Report
{
	public class ServiceReportEvent
	{

		public string eventSourceType;

		public string source;
		public string time;

		public string reportType;
		public JObject report;

	}
}