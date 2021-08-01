import ServiceType from './service-type'

export default class ServiceEvent {
	sourceId: String
	sourceType: ServiceType;
	time: Date;
	customMessage: String

	constructor(
		sourceId: String,
		sourceType: ServiceType,
		time: Date,
		customMessage: String) {

		this.sourceId = sourceId;
		this.sourceType = sourceType;
		this.time = time;
		this.customMessage = customMessage;
	}

}
