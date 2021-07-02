import express from 'express'

import connectToDb from './data/mongoose-service'
import setupRoutes from './routes/data-events-routes'
import startBrokerReceiver from './broker/broker-receiver'
import startRuleEngine from './rule-engine/rule-engine'

const server = express();

// don't know why is this here, someone on stackOvf suggested
server.use(express.urlencoded({ extended: true }));
server.use(express.json()); // middleware for parsing json requests

server.get('/', function (req, res) {
	console.log("Accessing home route ... ");
	res.send("Hello there ... ");
});

setupRoutes(server);

(async () => {
	connectToDb();
	await startBrokerReceiver();
	await startRuleEngine();
})()
	.then(() => {
		console.log('All services initialized ... ');

		server.listen(8080, function () {
			console.log('Server is up and running (port 8080) ...');
		});
	})
