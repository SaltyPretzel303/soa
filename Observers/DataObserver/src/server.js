// TODO require configuration

const config = require('./config/config.js').getConfig();

const server = require('express')()
const bodyParser = require('body-parser')
const database = require('./data/mongoose-service')

const eventsCrudRouter = require('./routes/data-events-routes')

database.createConnection();

server.use(bodyParser.json()) // middleware for parsing json body 

server.get('/', function (req, res) {
	console.log("Accessing home route ... ");
	res.send("Hello there ... ");
})

eventsCrudRouter.setupRoutes(server);

var serverApp = server.listen(8080, function () {
	console.log('Server is up and running (port 8080) ...');
})