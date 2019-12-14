using Newtonsoft.Json.Linq;
using SensorRegistry.Registry;

namespace SensorRegistry.Broker.Event.Reports
{

	public enum RegistryReportType
	{
		SensorRegistration,
		SensorUnregitration
	}

	public class RegistryReport
	{

		public RegistryReportType type;

		public SensorRecord record;

		public RegistryReport(RegistryReportType type, SensorRecord record)
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