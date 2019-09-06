using Newtonsoft.Json.Linq;

namespace SensorRegistry.Broker.Event.Reports
{
	public class RegistrationReport : Report
	{

		public string sensorName;
		public string sensorAddr;
		public int port;

		public RegistrationReport(string sensorName, string sensorAddr, int port)
		{
			this.sensorName = sensorName;
			this.sensorAddr = sensorAddr;
			this.port = port;
		}

		public string getReportType()
		{
			return "registration_report";
		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}
	}
}