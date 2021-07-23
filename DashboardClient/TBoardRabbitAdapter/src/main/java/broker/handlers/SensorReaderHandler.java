package broker.handlers;

import java.io.IOException;

import com.rabbitmq.client.AMQP.BasicProperties;

import com.rabbitmq.client.Envelope;

import broker.EventHandler;
import broker.messages.incoming.SensorReaderEvent;
import broker.messages.outgoing.SensorTelemetry;

public class SensorReaderHandler extends EventHandler {

	public SensorReaderHandler(String type, String topic, String filter) {
		super(type, topic, filter);
	}

	public void handleDelivery(
			String consumerTag,
			Envelope envelope,
			BasicProperties properties, byte[] body)
			throws IOException {

		var strContent = new String(body);
		var objContent = json.JsonUtil.deserialize(
				strContent,
				SensorReaderEvent.class);

		// in sensor context sourceId == sensorName
		var sensorInfo = deviceCache.getDevice(objContent.sourceId);
		var telemetryData = new SensorTelemetry(
				objContent.sourceId,
				objContent.time,
				objContent.LastReadIndex,
				objContent.NewData);

		super.sendTelemetryData(telemetryData, sensorInfo.token);
	}

}
