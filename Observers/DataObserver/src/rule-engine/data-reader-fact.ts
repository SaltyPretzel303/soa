import { Fact } from 'json-rules-engine'
import { ReaderData } from '../broker/reader-data'

export default class DataClassFact extends Fact<ReaderData> {
	constructor(fact_id: string, data: ReaderData) {
		super(fact_id, data);
	}
}