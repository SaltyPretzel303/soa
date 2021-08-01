import { ReaderData } from '../broker/reader-data'

export default class DataEvent {

	time: Date;

	ruleName: string;
	eventName: string;
	eventMessage: string;

	processedData: ReaderData;

	constructor(
		time: Date,
		ruleName: string,
		eventName: string,
		eventMessage: string,
		data: ReaderData) {

		this.time = time;
		this.ruleName = ruleName;
		this.eventName = eventName;
		this.eventMessage = eventMessage;
		this.processedData = data;
	}

	shortPrint(): string {
		let replacer = (key: string, value: any) => {
			if (key == "processedData") {
				return "big_reader_data";
			} else {
				return value;
			}
		};
		return JSON.stringify(this, replacer);
	}
}