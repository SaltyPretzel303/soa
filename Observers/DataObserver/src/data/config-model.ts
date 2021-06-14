import mongoose from 'mongoose';
import type { ServiceConfig } from '../config/service-configuration'

const configFieldsSchema = new mongoose.Schema({
	listeningPort: Number,
	dbAddress: String,
	dbPort: Number,
	dbName: String
});

const configSchema = new mongoose.Schema({
	updateTime: String,

	stage: String,
	Development: configFieldsSchema,
	Production: configFieldsSchema
});

// const configRecordSchema = new mongoose.Schema({
// 	updateTime: String,
// 	config: configSchema
// });

export interface IServiceConfig extends ServiceConfig, mongoose.Document {
	_id: string;
	updateTime: string;
}

const configModel = mongoose.model<IServiceConfig>("Config", configSchema);

export async function getAll(): Promise<IServiceConfig[]> {
	return configModel
		.find({})
		.exec();
};

export async function getById(id: string): Promise<IServiceConfig | null> {
	return configModel
		.findById(id)
		.exec();
}

export async function insertOne(oldConfig: ServiceConfig): Promise<IServiceConfig> {
	let modelData: IServiceConfig = await configModel.create<ServiceConfig>(oldConfig);
	modelData.updateTime = Date.now().toString();

	return modelData.save();
}

export async function removeById(id: string): Promise<IServiceConfig | null> {
	return configModel.findByIdAndDelete(id).exec();
}
