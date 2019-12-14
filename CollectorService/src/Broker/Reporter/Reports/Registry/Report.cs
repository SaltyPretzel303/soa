using CollectorService.Data.Registry;
using Newtonsoft.Json.Linq;

namespace CollectorService.src.Broker.Reporter.Reports.Registry
{

	public enum RegistryReportType
	{
		SensorRegistration,
		SensorUnregitration
	}

	public class Report
	{

		public RegistryReportType type;

		public SensorRecord record;

		public Report(RegistryReportType type, SensorRecord record)
		{
			this.type = type;
			this.record = record;
		}

		public JObject toJson()
		{
			return JObject.FromObject(this);
		}

	}
}