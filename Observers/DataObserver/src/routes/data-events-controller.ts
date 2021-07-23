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
					record.ruleName,
					record.eventName,
					record.eventMessage,
					record.processedData);

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
					data.ruleName,
					data.eventName,
					data.eventMessage,
					data.processedData);
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
		.then((record: dataModel.IDataEvent) => {

			const clientReadyResponse: DataEvent = new DataEvent(
				record.time,
				record.ruleName,
				record.eventName,
				record.eventMessage,
				record.processedData);

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

	// TODO implement
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
		.then((record: dataModel.IDataEvent | null) => {
			if (record != null) {

				const clientReadyData: DataEvent = new DataEvent(
					record.time,
					record.ruleName,
					record.eventName,
					record.eventMessage,
					record.processedData);

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
		.then((record: dataModel.IDataEvent | null) => {

			if (record != null) {
				const clientReadyData: DataEvent = new DataEvent(
					record.time,
					record.ruleName,
					record.eventName,
					record.eventMessage,
					record.processedData);

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

