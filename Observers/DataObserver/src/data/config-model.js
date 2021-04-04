const mongoose = require('mongoose');

const configSchema = new mongoose.Schema({
	updateTime: String,
	config: {
		stage: String,
		Development: {
			listeningPort: Number,
			dbAddress: String,
			dbPort: Number,
			dbName: String
		},
		Production: {
			listeningPort: Number,
			dbAddress: String,
			dbPort: Number,
			dbName: String
		}
	},
});

const configDataModel = mongoose.model("Config", configSchema);

module.exports.getAll = async () => {
	const allQuery = {};
	let filterQ = mongoose.
	return configDataModel
		.find(allQuery)
		.updateOne()
		.select(["-_id", "-__v"]); // remove these two fields from the final result
	// return promise 
};

module.exports.getById = async (id) => {
	return configDataModel
		.findById(id)
		.select(["-_id", "-__v"]);
}

module.exports.insertOne = async (newConfig) => {
	const newDataModel = new dataEventModel(dataEvent);
	return newDataModel.save(); // will return promise
}

module.exports.updateOne = async (id, fieldToUpdate, newValue) => {

	let oldData = await dataEventModel.findById(id);

	oldData[fieldToUpdate] = newValue;

	return oldData.save();

}

module.exports.removeById = (id) => {

	const idQuery = { "_id": id };

	return dataEventModel.deleteOne(idQuery);

}
