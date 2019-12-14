using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Reporter.Reports.Collector
{
	public interface Report
	{

		string getReportType();

		JObject toJson();

	}
}