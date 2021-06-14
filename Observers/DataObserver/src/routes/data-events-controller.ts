import { Request, Response, NextFunction } from 'express';
import DataEvent from '../rule-engine/data-event'
import * as dataModel from '../data/data-event-model';

export function getAllData(req: Request, res: Response, next: NextFunction) {
	dataModel.getAll()
		.then((data: dataModel.IDataEvent[]) => {

			console.log(`Get all result length: ${data.length}`);

			let clientReadyData: DataEvent[] = [];
			for (let record of data) {

				let singleData: DataEvent = new DataEvent(
					record.time,
					record.priority,
					record.description);

				clientReadyData.push(singleData);
			}

			res
				.type('application/json')
				.status(200)
				.send(clientReadyData);

		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});
};

export function getDataById(req: Request, res: Response, next: NextFunction) {

	const reqId: string = req.query.dataId as string;

	dataModel.getById(reqId)
		.then((data: dataModel.IDataEvent | null) => {

			if (data != null) {

				const clientReadyData: DataEvent = new DataEvent(
					data.time,
					data.priority,
					data.description
				);

				res
					.type('application/json')
					.status(200)
					.send(clientReadyData);

			} else {
				res
					.status(204)
					.send("No such record ... ");
			}

		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});

};

export function insertData(req: Request, res: Response, next: NextFunction) {

	const newData: DataEvent = req.body as DataEvent;

	dataModel.insertOne(newData)
		.then((addedData: dataModel.IDataEvent) => {

			const clientReadyResponse: DataEvent = new DataEvent(
				addedData.time,
				addedData.priority,
				addedData.description
			);

			// TODO maybe instead returning object same as the one that is sent
			// create a new class which is gonna encapsulate that obj. and some message
			// it is unnecessary but pretty ... 
			res
				.status(200)
				.send(clientReadyResponse);

		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);

			res
				.status(500)
				.send("Database error ... ");
		});
};

export function updateData(req: Request, res: Response, next: NextFunction) {

	const id: string = req.body.id;
	const newValue: DataEvent = req.body.newValue as DataEvent;

	console.log(`requesting data update of id: ${id}`);
	console.log(JSON.stringify(newValue))

	const message: string = "This method is still not implemented ... ";

	res
		.status(204)
		.send(message);

	return;

	dataModel.updateOne(id, newValue)
		.then((updatedData: dataModel.IDataEvent | null) => {
			if (updatedData != null) {

				const clientReadyData: DataEvent = new DataEvent(
					updatedData.time,
					updatedData.priority,
					updatedData.description
				);

				res
					.status(200)
					.send(clientReadyData);
			} else {
				res
					.status(204)
					.send(`No such record, id: ${id}`);
			}

		}).catch((error) => {
			console.log(`We got error from database ... \nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});
};

export function deleteData(req: Request, res: Response, next: NextFunction) {

	let id: string = req.query.id as string;
	console.log(`requesting data removal id: ${id}`);

	dataModel.removeById(id)
		.then((removedData: dataModel.IDataEvent | null) => {

			if (removedData != null) {
				const clientReadyData: DataEvent = new DataEvent(
					removedData.time,
					removedData.priority,
					removedData.description
				);

				res
					.status(200)
					.send(clientReadyData);
			} else {
				res
					.status(204)
					.send(`Failed to remove doc with id: ${id} ... `);
			}

		}).catch((error) => {
			console.log(`We god an error from database\nError: ${error}`);
			res
				.status(500)
				.send("Database error ... ");
		});

};

