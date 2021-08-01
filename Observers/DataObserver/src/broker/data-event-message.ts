import ServiceEvent from './service-event'
import ServiceType from '../broker/service-type'
import { ReaderData } from '../broker/reader-data'

export default class DataEventMessage extends ServiceEvent {
	ruleName: string;
	eventName: string;
	data: ReaderData;

	constructor(
		sourceId: string,
		serviceType: ServiceType,
		time: Date,
		message: string,
		ruleName: string,
		eventName: string,
		data: ReaderData) {

		super(sourceId, serviceType, time, message)

		this.ruleName = ruleName;
		this.eventName = eventName;
		this.data = data;
	}


}