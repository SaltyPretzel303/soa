package config;

import java.util.List;

public class ConfigFields {

	public String serviceId;

	public String brokerAddress;
	public int brokerPort;

	public String sensorReaderTopic;
	public String sensorReaderFilter;

	public String collectorEventTopic;
	public String collectorPullEventFilter;
	public String collectorAccessEventFilter;

	public String registryEventTopic;
	public String registryEventFilter;

	public String dbAddress;
	public int dbPort;

	public String tBoardAddress;
	public String tBoardPort;
	public String tBoardTenantUsername;
	public String tBoardTenantPassword;

	public String tBoardCustomerUsername;
	public String tBoardCustomerPassword;

	public String tBoardSensorDeviceType;
	public String tBoardCollectorDeviceType;
	public String tBoardRegistryDeviceType;

	public List<EventHandlerInfo> handlersInfo;

	public ConfigFields() {

	}

}
