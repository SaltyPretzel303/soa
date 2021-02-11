const mongoose = require('mongoose');

const dataEventSchema = new mongoose.Schema({
	time: String,
	priority: Number,
	description: String
});

const dataEventModel = mongoose.model("DataEvent", dataEventSchema);

module.exports.getAll = async () => {
	const allQuery = {};
	return dataEventModel
		.find(allQuery)
		.select(["-_id", "-__v"]); // remove these two fields from the final result
	// return promise 
};

module.exports.getById = async (id) => {
	return dataEventModel
		.findById(id)
		.select(["-_id", "-__v"]);
}

module.exports.insertOne = async (dataEvent) => {
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
