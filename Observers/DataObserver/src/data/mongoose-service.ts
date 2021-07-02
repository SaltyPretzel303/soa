import { ServiceConfig } from '../config/service-configuration'
import mongoose from 'mongoose'

const config = ServiceConfig.GetInstance();

const dbAddress = config.dbAddress;
const dbPort = config.dbPort;
const dbName = config.dbName;

const dbUrl = "mongodb://" + dbAddress + ":" + dbPort + "/" + dbName;

const options = {
	useNewUrlParser: true,
	useUnifiedTopology: true,
	useFindAndModify: false
}

let retryCounter = 0;
let retryTimeout: number = config.dbRetryTimeout;

let timer: NodeJS.Timeout;

export default function createConnection(): void {
	console.log(`Trying to connect to mongodb on: ${dbUrl}`);

	// default timeout (waiting time) for connection is 36s
	mongoose.connect(dbUrl, options)
		.then(() => {
			console.log(`Successfully connected to mongodb on ${dbUrl}`);
		})
		.catch((error) => {
			retryCounter++;

			console.log(`Failed to connect to the mongodb on ${dbUrl}`);
			console.log(`Reason: ${error}`);
			console.log(`Retrying connection in ${retryTimeout}ms (${retryCounter + 1} attempt)`);
			// console.log(`Retry counter on: ${retryCounter}`);

			timer = setTimeout(createConnection, retryTimeout);
		});
}

mongoose.connection.readyState

process.on('exit', (code) => {
	if (timer != null) {
		clearTimeout(timer);
	}
	mongoose.disconnect();
});