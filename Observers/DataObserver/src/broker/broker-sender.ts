import amqp from 'amqplib'
import { ServiceConfig } from '../config/service-configuration'
import DataEvent from '../rule-engine/data-event'
import DataEventMessage from './data-event-message'
import ServiceType from './service-type'

const config = ServiceConfig.GetInstance();
const url = `amqp://${config.brokerAddress}:${config.brokerPort}`;

let connection: amqp.Connection | null = null;
// let is_connected: boolean = false;

async function establishConnection(): Promise<boolean> {
	if (connection != null) {
		// connection is already established
		return true;
	}

	try {
		connection = await amqp.connect(url);
		console.log(`Broker sender connected to: ${url}`);

		return true;
	} catch (err) {
		console.log(`Broker sender failed to establish connection: ${err}`);
		connection = null;

		return false;
	}
}

async function getChannel(): Promise<amqp.Channel | null> {
	if (connection == null) {
		let result = await establishConnection();
		if (result != true) {
			return null;
		}
	}

	if (connection != null) {
		return connection.createChannel()
	} else {
		return null;
	}
}

export async function sendDataEvent(newEvent: DataEvent): Promise<boolean> {
	let topic = config.observingResultsTopic;
	let filter = config.observingResultsFilter;

	let topic_options: amqp.Options.AssertExchange = {
		durable: true,
		autoDelete: true
	};
	let publish_options: amqp.Options.Publish = {
		mandatory: false
	}

	let service_event = new DataEventMessage(
		config.serviceId,
		ServiceType.SensorObserver,
		newEvent.time,
		newEvent.eventMessage,
		newEvent.ruleName,
		newEvent.eventName,
		newEvent.processedData);

	let str_message = JSON.stringify(service_event);

	let channel;
	try {

		// if (connection == null) {
		// 	await establishConnection();
		// }

		channel = await getChannel();
		if (channel == null) {
			return false;
		}

		await channel.assertExchange(topic, "topic", topic_options);

		// why is publish method not async ... ?
		channel.publish(topic, filter, Buffer.from(str_message), publish_options);

		return true;
	} catch (err) {
		console.log(`Failed to publish event: ${err}`);
		return false;
	} finally {
		if (channel != null && channel != undefined) {
			channel.close();
		}
	}
}

process.on('exit', (code) => {
	if (connection != null) {
		connection.close();
	}
});