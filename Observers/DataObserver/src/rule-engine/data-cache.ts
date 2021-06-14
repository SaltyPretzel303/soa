import { ReaderData } from '../broker/reader-data'

let cache: ReaderData[] = [] as ReaderData[];

export function queueData(data: ReaderData): void {
	cache.push(data);
}

export function getCachedData(): ReaderData[] {
	let tempArray = cache;
	cache = [] as ReaderData[];
	return tempArray;
}

export function getCacheCount(): number {
	return cache.length;
}
