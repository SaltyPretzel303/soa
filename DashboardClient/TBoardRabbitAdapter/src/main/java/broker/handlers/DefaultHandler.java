package broker.handlers;

import java.io.IOException;

import com.rabbitmq.client.AMQP.BasicProperties;

import com.rabbitmq.client.Envelope;

import broker.EventHandler;
import broker.messages.incoming.ServiceEvent;

public class DefaultHandler extends EventHandler {

	public DefaultHandler(String deviceType, String topic, String filter) {
		super(deviceType, topic, filter);
	}

	@Override
	public void handleDelivery(
			String consumerTag,
			Envelope envelope,
			BasicProperties properties,
			byte[] body) throws IOException {

		String strContent = new String(body);
		var objContent = json.JsonUtil.deserialize(
				strContent,
				ServiceEvent.class);

		var deviceInfo = this.deviceCache.getDevice(objContent.sourceId);
		super.sendTelemetryData(strContent, deviceInfo.token);
	}

}
