const config = require('./config/config-manager.js').getConfig();

const express = require('express')
const server = express()
const database = require('./data/mongoose-service')

const eventsCrudRouter = require('./routes/data-events-routes')

database.createConnection();

// don't know why is this here, someone on stackOvf suggested
server.use(express.urlencoded({ extended: true }))
server.use(express.json()) // middleware for parsing json requests

server.get('/', function (req, res) {
	console.log("Accessing home route ... ");
	res.send("Hello there ... ");
})

eventsCrudRouter.setupRoutes(server);

var serverApp = server.listen(8080, function () {
	console.log('Server is up and running (port 8080) ...');
})