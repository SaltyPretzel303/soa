import mongoose from 'mongoose';
import DataEvent from '../rule-engine/data-event'

const dataEventSchema = new mongoose.Schema({
	time: String,
	priority: Number,
	description: String,
});

export interface IDataEvent extends DataEvent, mongoose.Document {
	_id: string;
}

const dataModel = mongoose.model<IDataEvent>(
	'DataEvent',
	dataEventSchema,
	'DataEvents');

export async function getAll(): Promise<IDataEvent[]> {
	return dataModel
		.find({})
		// .select(["-_id", "-__v"])
		.exec();
};

export async function getById(id: string): Promise<IDataEvent | null> {
	return dataModel
		.findById(id)
		// .select(["-_id", "-__v"])
		.exec();
}

export async function insertOne(newData: DataEvent): Promise<IDataEvent> {
	const dbData: IDataEvent = await dataModel.create<DataEvent>(newData);
	// TODO handle errors somehow ... 
	return dbData.save();
}

// this just doesn't work ...
// TODO should be implemented somehow ... 
export async function updateOne(id: string, newValue: DataEvent)
	: Promise<IDataEvent | null> {

	const newData = await dataModel.create<DataEvent>(newValue);
	const data = new dataModel();

	console.log(`${JSON.stringify(newData)}`);
	// TODO also handle errors here
	// const query = { _id: "id" };
	return dataModel
		.findOneAndReplace({ _id: id }, newData)
		// .findByIdAndUpdate(id, newData)
		// .replaceOne(query, newData)
		.exec();
}

export async function removeById(id: string): Promise<IDataEvent | null> {
	return dataModel.findByIdAndDelete(id).exec();
}
