const controller = require('./data-events-controller')

module.exports.setupRoutes = (app) => {

	// get all saved dataEvents from database 
	app.get('/data/getAll', controller.getAllData);

	// get single dataEvent matched by id 
	app.get("/data/getSingle", controller.getDataById);

	// a whole body of request obj should be data that are gonna be saved
	app.post("/data/addDataEvent", controller.insertData);

	/*
		requires data in format:
		{
			id: "someRandomCharacters", // id of document to update
			fieldToUpdate: "someFieldName" // which field you want to update
			newValue: "someNewValue"
			// new value which is going to be assigned to the field 'fieldToUpdate'
		}
	*/
	app.post("/data/updateDataEvent", controller.updateData);

	// delete data matched by id
	app.delete("/data/deleteDataEvent", controller.deleteData);

}