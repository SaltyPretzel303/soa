package broker.handlers;

import java.io.IOException;

import com.rabbitmq.client.AMQP.BasicProperties;
import com.rabbitmq.client.Envelope;

import broker.EventHandler;
import broker.messages.incoming.CollectorAccessEvent;
import broker.messages.outgoing.CollectorTelemetry;

public class CollectorAccessHandler extends EventHandler {

	public CollectorAccessHandler(
			String deviceType,
			String topic,
			String filter) {
		super(deviceType, topic, filter);
	}

	@Override
	public void handleDelivery(String consumerTag, Envelope envelope,
			BasicProperties properties, byte[] body)
			throws IOException {

		var strContent = new String(body);
		var objContent = json.JsonUtil.deserialize(
				strContent,
				CollectorAccessEvent.class);

		var deviceInfo = deviceCache.getDevice(objContent.sourceId);
		var telemetryData = new CollectorTelemetry(objContent);

		super.sendTelemetryData(telemetryData, deviceInfo.token);
	}

}
