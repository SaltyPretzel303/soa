namespace CollectorService.Configuration
{
	public class ConfigFields
	{
		public int port { get; set; }

		public string dbAddress { get; set; }
		public string dbPort { get; set; }
		public string dbName { get; set; }
		public string sensorsCollection { get; set; }
		public string configurationBackupCollection { get; set; }

		public string headerUrl { get; set; }
		public string dataRangeUrl { get; set; }

		public int readInterval { get; set; }

		public string sensorRegistryAddr { get; set; }
		public string sensorRegistryPort { get; set; }
		public string sensorAddrResolutionPath { get; set; }
		public string sensorListReqPath { get; set; }

		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }
		public int retryConnectionDelay { get; set; }

		public string collectorTopic { get; set; }
		public string accessEventFilter { get; set; }
		public string pullEventFilter { get; set; }

		public string serviceLogTopic { get; set; }
		public string serviceLifetimeTopic { get; set; }

		public string serviceTypeFilter { get; set; }
		public string configurationTopic { get; set; }
		public string targetConfiguration { get; set; }

		public string allFilter { get; set; }

		public string sensorRegistryTopic { get; set; }
		public string newSensorFilter { get; set; }
		public string sensorRemovedFilter { get; set; }

	}
}