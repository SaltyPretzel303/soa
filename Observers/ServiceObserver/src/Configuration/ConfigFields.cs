namespace ServiceObserver.Configuration
{
	public class ConfigFields
	{
		// REST api listening port 
		public int port { get; set; }

		// database
		public string dbAddress { get; set; }
		public string dbName { get; set; }
		public string observerReportsCollection { get; set; }
		public string configBackupCollection { get; set; }

		// rule engine records
		public string unstableRecordCollection { get; set; }

		// message broker
		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }
		public int connectionRetryDelay { get; set; }

		// topics and filters
		public string observerReportTopic { get; set; }
		public string configUpdateTopic { get; set; }
		public string serviceLifetimeTopic { get; set; }
		public string lifetimeEventFilter { get; set; }
		public string serviceLogTopic { get; set; }
		public string serviceTypeFilter { get; set; }

		// rule engine results
		public string observingResultsTopic { get; set; }
		public string observingResultsFilter { get; set; }

		// rule engine
		public int ruleEngineTriggerInterval { get; set; }
		public int unstableRecordsLimit { get; set; }

	}
}