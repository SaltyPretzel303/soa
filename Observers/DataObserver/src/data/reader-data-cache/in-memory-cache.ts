import { ReaderData } from "../../broker/reader-data"
import { IReaderDataCache } from "./reader-data-cache"

export default class InMemoryCache implements IReaderDataCache {

	private cache_size;

	private cache: ReaderData[];

	private occupied = 0;
	private last = 0;
	private first = 0;

	constructor(size: number) {
		this.cache_size = size;
		this.cache = new Array<ReaderData>(size);
	}

	private isFull(): boolean {
		return this.occupied == this.cache_size;
	}

	private isEmpty(): boolean {
		return (this.occupied == 0);
	}

	private moveLast() {
		if (this.last < (this.cache_size - 1)) {
			this.last++;
		} else {
			this.last = 0;
		}
	}

	private moveFirst() {
		if (this.first < (this.cache_size - 1)) {
			this.first++;
		} else {
			this.first = 0;
		}
	}

	enqueueData(newData: ReaderData): Promise<ReaderData> {
		return new Promise((resolve) => {
			if (!this.isFull()) {
				this.cache[this.last] = newData;
				this.moveLast();
				this.occupied++;
			} else {
				console.log("Reader data cache is full ... ");
			}

			resolve(newData);
		});
	}

	dequeueData(): Promise<ReaderData | null> {

		return new Promise((resolve) => {
			let ret_data = null;

			if (this.occupied > 0) {
				ret_data = this.cache[this.first];
				this.moveFirst();
				this.occupied--;
			}

			resolve(ret_data);
		});
	}

	getCachedData(): Promise<ReaderData[]> {
		return new Promise(async (resolve) => {

			let ret_array = [] as ReaderData[];

			let new_data = null;
			while ((new_data = await this.dequeueData()) != null) {
				ret_array.push(new_data);
			}

			resolve(ret_array);
		});
	}

	getDataCount(): Promise<number> {
		return new Promise((resolve) => {
			resolve(this.occupied);
		});
	}
}