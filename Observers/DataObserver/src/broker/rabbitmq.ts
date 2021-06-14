import amqp from 'amqplib'
import { ServiceConfig, ConfigFields } from '../config/service-configuration'

const config: ConfigFields = ServiceConfig.GetInstance();
const url = `amqp://${config.brokerAddress}:${config.brokerPort}`

let connection: amqp.Connection;

async function connectToBroker(): Promise<amqp.Connection | null> {
	try {
		let connection = await amqp.connect(url);

		console.log(`Broker connection established on: ${url} ... `);
		return connection;
	} catch (error) {
		console.log(`Failed to connect to broker on: ${url}`);
		console.log(`Error: ${error}`);

		return null;
	}

}

export async function publish(): Promise<boolean> {
	if (connection == undefined || connection == null) {
		let conn_result = await connectToBroker();
		if (conn_result != null) {
			connection = conn_result;
		} else {
			console.log("Can't publish message, broker connection is not established ... ");
		}
		return false;
	}
	try {
		let channel = await connection.createChannel();

		let content: string = "some message ... ";

		return channel.sendToQueue("queue_name", Buffer.from(content));
	} catch (err) {
		console.log(`Failed to create broker channel, reason: ${err}`);
		return false;
	}

}

process.on('exit', (code) => {
	if (connection != null) {
		connection.close();
	}
})