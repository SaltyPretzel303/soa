using Newtonsoft.Json.Linq;

namespace CollectorService.Broker.Reporter.Reports
{
	public class SensorPullReport : Report
	{

		public const string reportType = "sensor_pull_report";

		public string sensorAddress;

		public string whatHappened;

		public SensorPullReport(string sensorAddress, string whatHappened)
		{
			this.sensorAddress = sensorAddress;
			this.whatHappened = whatHappened;
		}

		public string getReportType()
		{
			return "sensor_pull_report";
		}

		public JObject toJson()
		{

			return JObject.FromObject(this);

		}
	}
}