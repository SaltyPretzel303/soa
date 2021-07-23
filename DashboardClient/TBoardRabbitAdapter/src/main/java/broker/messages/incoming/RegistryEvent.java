package broker.messages.incoming;

public class RegistryEvent extends ServiceEvent {

	public RegistryEventType eventType;
	public SensorRegistryRecord sensorRecord;

	public RegistryEvent(
			String sourceId,
			ServiceType type,
			RegistryEventType eventType,
			SensorRegistryRecord sensorRecord,
			String customMessage) {

		super(sourceId, type, customMessage);

		this.eventType = eventType;
		this.sensorRecord = sensorRecord;
	}

}
