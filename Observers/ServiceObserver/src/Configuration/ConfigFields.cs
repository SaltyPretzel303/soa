namespace ServiceObserver.Configuration
{
	public class ConfigFields
	{

		public string serviceId { get; set; }

		// REST api listening port 
		public int port { get; set; }

		// database
		public string dbAddress { get; set; }
		public string dbName { get; set; }
		public string observerReportsCollection { get; set; }
		public string configBackupCollection { get; set; }

		// rule engine db records
		public string unstableRecordCollection { get; set; }

		// message broker
		public string brokerAddress { get; set; }
		public int brokerPort { get; set; }
		public int connectionRetryDelay { get; set; }

		// topics and filters
		public string configUpdateTopic { get; set; }
		public string serviceLifetimeTopic { get; set; }
		public string lifetimeEventFilter { get; set; } // used by ruleEngine
		public string serviceLogTopic { get; set; }
		public string serviceTypeFilter { get; set; }

		// rule engine results
		public string observingResultsTopic { get; set; }
		public string observingResultsFilter { get; set; }
		public string unstableServiceFilter { get; set; }

		// rule engine
		public int ruleEngineTriggerInterval { get; set; }
		public int unstableRecordsLimit { get; set; }

	}
}