using Newtonsoft.Json.Linq;

namespace SensorRegistry.Broker.Event.Reports
{
	public interface Report
	{

		string getReportType();

		JObject toJson();

	}
}