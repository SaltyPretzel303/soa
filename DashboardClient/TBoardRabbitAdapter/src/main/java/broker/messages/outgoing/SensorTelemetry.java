package broker.messages.outgoing;

import java.util.Date;

import broker.messages.incoming.SensorValues;

public class SensorTelemetry {

	public String sensorName;

	public Date time;

	public int lastReadIndex;

	public SensorValues sensorValues;

	public SensorTelemetry(
			String sensorName,
			Date time,
			int lastReadIndex,
			SensorValues sensorValues) {

		this.sensorName = sensorName;
		this.time = time;
		this.lastReadIndex = lastReadIndex;
		this.sensorValues = sensorValues;
	}

}
