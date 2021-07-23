package broker.handlers;

import java.io.IOException;

import com.rabbitmq.client.AMQP.BasicProperties;
import com.rabbitmq.client.Envelope;

import broker.EventHandler;
import broker.messages.incoming.RegistryEvent;
import broker.messages.outgoing.RegistryTelemetry;

public class RegistryEventHandler extends EventHandler {

	public RegistryEventHandler(
			String deviceType,
			String topic,
			String filter) {
		super(deviceType, topic, filter);
	}

	@Override
	public void handleDelivery(String consumerTag, Envelope envelope,
			BasicProperties properties, byte[] body) throws IOException {

		// System.out.println("Registry event ... ");
		var strContent = new String(body);
		var objContent = json.JsonUtil.deserialize(
				strContent,
				RegistryEvent.class);

		var deviceInfo = deviceCache.getDevice(objContent.sourceId);
		var telemetryData = new RegistryTelemetry(
				objContent.sourceId,
				objContent.eventType.toString(),
				objContent.sensorRecord);

		super.sendTelemetryData(telemetryData, deviceInfo.token);
	}

}
