using Newtonsoft.Json.Linq;

namespace SensorRegistry.Broker.Event.Reports
{
	public class UnregistrationReport : Report
	{

		private string sensorName;
		private string sensorAddr;
		private int port;

		public UnregistrationReport(string sensorName, string sensorAddr, int port)
		{
			this.sensorName = sensorName;
			this.sensorAddr = sensorAddr;
			this.port = port;
		}

		public string getReportType()
		{
			return "unregistration_report";
		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}

	}
}