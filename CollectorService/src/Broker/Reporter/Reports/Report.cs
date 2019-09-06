using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Reporter.Reports
{
	public interface Report
	{

		string getReportType();

		JObject toJson();

	}
}