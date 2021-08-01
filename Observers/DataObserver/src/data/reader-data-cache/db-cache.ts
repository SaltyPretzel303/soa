import { ReaderData } from '../../broker/reader-data';
import { IReaderDataCache } from './reader-data-cache';

import { IReaderData, DataModel } from './reader-data-model'


export default class DbCache implements IReaderDataCache {

	// at this point I guess mongo connection should be established
	// thats the job of mongoose-service ... ? 

	constructor() {
		// yep it is constructed ... 
	}

	async enqueueData(newData: ReaderData): Promise<ReaderData> {
		return await DataModel.create<ReaderData>(newData);
	}

	async dequeueData(): Promise<ReaderData | null> {
		let dbData: IReaderData | null = await DataModel
			.findOneAndRemove({})
			.projection({ _id: 0, __v: 0 })
			.sort({ time: 'ascending' })
			.exec();

		let str_data = JSON.stringify(dbData);
		return JSON.parse(str_data) as ReaderData;
	}

	async getCachedData(): Promise<ReaderData[]> {
		let dbData: IReaderData[] = await DataModel
			.find({}, { __v: 0 })
			.sort({ time: 'ascending' })
			.exec();

		// convert data returned from db to actual ReaderData
		// instead IReaderData full of setters/getters and not properties
		// rule-engine cant use IReaderData "class"
		let retData: ReaderData[] = dbData.map((value) => {
			let str_value = JSON.stringify(value);
			let ret_obj = JSON.parse(str_value);

			// if this field was excluded using projections
			// there would be no way to remove that document from db by id
			// without some additional querying based on other properties
			delete ret_obj._id;

			return ret_obj as ReaderData;
		});

		let remove_count = 0;
		for (let record of dbData) {
			await DataModel.findByIdAndRemove(record._id);
			remove_count++;
		}
		// console.log(`Removed {remove_count} records from mongo-cache ... `);

		return retData;
	}

	getDataCount(): Promise<number> {
		return DataModel.countDocuments().exec();
	}
}
