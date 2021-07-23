package broker.messages.incoming;

import java.util.Date;

public class ServiceEvent {

	public String sourceId;

	public ServiceType sourceType;

	public Date time;

	public String customMessage;

	public ServiceEvent(
			String sourceId,
			ServiceType type,
			String customMessage) {

		this.sourceId = sourceId;
		this.time = new Date();
		this.sourceType = type;
		this.customMessage = customMessage;
	}
}
