import amqp from 'amqplib'
import { ServiceConfig, ConfigFields } from '../config/service-configuration'
import { ReaderData } from './reader-data'
// import { queueData } from '../rule-engine/data-cache'
import { Cache } from "../data/reader-data-cache/reader-data-cache"

const config: ConfigFields = ServiceConfig.GetInstance();
const url = `amqp://${config.brokerAddress}:${config.brokerPort}`

let connection: amqp.Connection;

export default async function startBrokerReceiver(): Promise<boolean> {
	try {
		connection = await amqp.connect(url);
		console.log(`Broker connection established on: ${url} ... `);

		let channel = await connection.createChannel();
		await initReceiver(channel);

		return true;
	} catch (error) {
		console.log(`Failed to start broker receiver: ${url}`);
		console.log(`Error: ${error}`);

		return false;
	}
}

async function initReceiver(channel: amqp.Channel): Promise<boolean> {

	let filter: string = config.sensorDataFilter;
	let topic: string = config.sensorDataTopic;

	await channel.assertExchange(
		topic,
		'topic',
		{ durable: true, autoDelete: true });

	let queue_assert_res: amqp.Replies.AssertQueue = await channel.assertQueue(
		'',
		{ durable: false, autoDelete: true }
	);

	await channel.bindQueue(queue_assert_res.queue, topic, filter);
	// TODO note that this maybe [doesn't have to be/should not be] awaited 
	await channel.consume(
		queue_assert_res.queue,
		(msg) => {
			let sData = msg?.content.toString();
			if (sData != null && sData != undefined) {

				let receivedData: ReaderData = JSON.parse(sData);
				Cache.enqueueData(receivedData);
				// queueData(receivedData); // prev. implementation 

				// console.log(`${JSON.stringify(receivedData)}`);
			}
		});

	return true;
}

process.on('exit', (code) => {
	if (connection != null) {
		connection.close();
	}
})