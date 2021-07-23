package broker.messages.outgoing;

import broker.messages.incoming.SensorRegistryRecord;

public class RegistryTelemetry {

	public String sourceId;
	public String eventType;
	public SensorRegistryRecord sensorRecord;

	public RegistryTelemetry(
			String sourceId,
			String eventType,
			SensorRegistryRecord sensorRecord) {

		this.sourceId = sourceId;
		this.eventType = eventType;
		this.sensorRecord = sensorRecord;
	}

}
