package broker.handlers;

import java.io.IOException;

import com.rabbitmq.client.AMQP.BasicProperties;
import com.rabbitmq.client.Envelope;

import broker.EventHandler;
import broker.messages.incoming.CollectorPullEvent;
import broker.messages.outgoing.CollectorTelemetry;

public class CollectorPullHandler extends EventHandler {

	public CollectorPullHandler(
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
		System.out.println(strContent);
		var objContent = json.JsonUtil.deserialize(
				strContent,
				CollectorPullEvent.class);

		System.out.println(json.JsonUtil.prettySerialize(objContent));
		var deviceInfo = deviceCache.getDevice(objContent.sourceId);
		var telemetryData = new CollectorTelemetry(objContent);

		super.sendTelemetryData(telemetryData, deviceInfo.token);
	}

}
