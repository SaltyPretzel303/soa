const mognoose = require('mongoose')

const url = "mongodb://localhost:27017";
const dbName = "data_observer";

const dbUrl = url + "/" + dbName;

const options = {
	useNewUrlParser: true,
	useUnifiedTopology: true
}

let retryCounter = 0;
let retryTimeout = 1500;

// TODO maybe accept configuration trough the method arguments ... 
const createConnection = () => {
	console.log(`Trying to connect to mongodb on: ${dbUrl}`);
	mognoose.connect(dbUrl, options).then(() => {

		console.log(`Succesfully connected to mongodb on ${dbUrl}`);

	}).catch((error) => {
		retryCounter++;

		console.log(`Failed to connect to the mongodb on ${dbUrl}`);
		console.log(`Going to retry to connect for ${retryTimeout}ms`);
		console.log(`Retry counter on: ${retryCounter}`);

		setTimeout(createConnection, retryTimeout);
	});
}

module.exports.createConnection = createConnection;

process.on('exit',(code)=>{
	mognoose.disconnect
});