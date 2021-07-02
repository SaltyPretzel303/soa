import { ReaderData } from '../../broker/reader-data'

export interface IReaderDataCache {
	enqueueData(newData: ReaderData): Promise<ReaderData>;
	dequeueData(): Promise<ReaderData | null>;
	getCachedData(): Promise<ReaderData[]>;
	getDataCount(): Promise<number>;
}

// import InMemoryCache from './in-memory-cache'
// const Cache: IReaderDataCache = new InMemoryCache(500);

import DbCache from './db-cache'
const Cache: DbCache = new DbCache();

export { Cache };
