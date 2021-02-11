const dataModel = require('../data/data-events-model')

module.exports.getAllData = (req, res, next) => {

	dataModel.getAll()
		.then((data) => {

			// const objData = data.toObject();

			console.log(`Result length: ${data.length}`);

			res
				.type('application/json')
				.status(200)
				.send(data);

		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});

};

module.exports.getDataById = (req, res, next) => {

	// this may not be the best way to extract query parameter
	const reqId = req.query['id'];

	dataModel.getById(reqId)
		.then((data) => {

			res
				.type('application/json')
				.status(200)
				.send(data);

		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});

};

module.exports.insertData = (req, res, next) => {
	const newData = req.body;

	dataModel.insertOne(newData)
		.then(() => {
			res
				.status(200)
				.send("New data added to the database ... ");
		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});
};

module.exports.updateData = (req, res, next) => {

	const { id, fieldToUpdate, newValue } = req.body;

	dataModel.updateOne(id, fieldToUpdate, newValue)
		.then(() => {
			res
				.status(200)
				.send("Document successfully updated ... ");
		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});
};

module.exports.deleteData = (req, res, next) => {

	dataModel.removeById(req.query['id'])
		.then(() => {

			res
				.status(200)
				.send(`Document with id: ${req.query["id"]} is removed from the database ... `);

		}).catch((error) => {
			console.log(`We god an error from database\nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});

};

