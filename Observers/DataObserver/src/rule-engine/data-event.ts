export default class DataEvent {

	time: number;
	priority: number;
	description: string;

	constructor(time: number, priority: number, description: string) {
		this.time = time;
		this.priority = priority;
		this.description = description;
	}

}