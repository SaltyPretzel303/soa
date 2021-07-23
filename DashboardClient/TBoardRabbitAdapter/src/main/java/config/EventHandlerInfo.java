package config;

public class EventHandlerInfo {

	public String deviceType;
	public String deviceTopic;
	public String deviceFilter;

	public EventHandlerInfo(
			String deviceType,
			String deviceTopic,
			String deviceFilter) {

		this.deviceType = deviceType;
		this.deviceTopic = deviceTopic;
		this.deviceFilter = deviceFilter;
	}

}
