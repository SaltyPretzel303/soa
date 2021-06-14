import type { Express } from 'express';
import * as controller from './data-events-controller'

export default function setupRoutes(app: Express) {

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
			newValue: "value that will replace existing document (if it exists)"
		}
	*/
	app.post("/data/updateDataEvent", controller.updateData);

	// delete data matched by id
	app.delete("/data/deleteDataEvent", controller.deleteData);

}