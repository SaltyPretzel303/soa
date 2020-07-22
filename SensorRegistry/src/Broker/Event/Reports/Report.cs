using Newtonsoft.Json.Linq;
using CommunicationModel;

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

		public SensorRegistryRecord record;

		public RegistryReport(RegistryReportType type, SensorRegistryRecord record)
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