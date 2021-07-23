package broker;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeoutException;

import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;

import config.ConfigFields;
import config.ServiceConfiguration;

public class BrokerReceiver implements Runnable {

	private class BrokerConsumer {

		public EventHandler handler;
		public Channel channel;
		public String queue;

		public BrokerConsumer(EventHandler handler) {
			this.handler = handler;
		}

	}

	private ConfigFields config;

	private Connection connection;
	private List<BrokerConsumer> consumers;

	public BrokerReceiver(List<EventHandler> handlers) {
		this.config = ServiceConfiguration.getInstance();
		this.consumers = new ArrayList<BrokerConsumer>();

		for (var handler : handlers) {
			this.consumers.add(new BrokerConsumer(handler));
		}
	}

	@Override
	public void run() {

		var factory = new ConnectionFactory();
		factory.setHost(config.brokerAddress);
		factory.setPort(config.brokerPort);

		try {
			this.connection = factory.newConnection();
			System.out.println("Broker connection established on: ... ");

			for (var consumer : this.consumers) {
				var channel = this.connection.openChannel().get();

				String topic = consumer.handler.getTopicName();
				String filter = consumer.handler.getFilter();

				System.out.println("Creating consumer - topic: " + topic
						+ ", filter: " + filter);

				channel.exchangeDeclare(topic, "topic", true, true, null);
				String queue = channel.queueDeclare().getQueue();

				channel.queueBind(queue, topic, filter);
				channel.basicConsume(queue, consumer.handler);

				consumer.channel = channel;
				consumer.queue = queue;
			}

		} catch (IOException | TimeoutException e) {
			System.out.println("Exception while connecting to broker .. ");
			System.out.println(e.getMessage());
		}

	}

	public void StopReceiver() {

		try {
			if (this.consumers != null) {
				for (var consumer : this.consumers) {
					if (consumer.channel != null && consumer.channel.isOpen()) {

						consumer.channel.queueDelete(consumer.queue);
						consumer.channel.close();

					}
				}
			}

			if (this.connection != null && this.connection.isOpen()) {
				this.connection.close();
			}

		} catch (Exception e) {
			System.out.println(
					"Failed to close channel/connection in broker receiver ... ");
			System.out.println(e.getMessage());

		}
	}

}
