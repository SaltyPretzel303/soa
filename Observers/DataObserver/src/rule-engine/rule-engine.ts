import fs from 'fs'

import { ConfigFields, ServiceConfig } from '../config/service-configuration'
import { Cache } from '../data/reader-data-cache/reader-data-cache'
import { ReaderData } from '../broker/reader-data'
import {
	Engine,
	RuleProperties,
	EngineResult,
	Almanac,
	RuleResult,
	EventHandler,
	Event
} from 'json-rules-engine'
import DataReaderFact from './data-reader-fact'
import DataEvent from './data-event'
import { sendDataEvent } from '../broker/broker-sender';
import * as eventModel from '../data/data-event-model'

const config: ConfigFields = ServiceConfig.GetInstance();

let timer: NodeJS.Timeout;
let engine: Engine;

async function readRules(): Promise<RuleProperties[]> {
	let read_rules: RuleProperties[] = [];

	try {

		let rule_files: string[] = await fs.promises.readdir(config.rulesDirPath);
		console.log(`Found ${rule_files.length} rule files ... `);

		for (let f_rule of rule_files) {

			let rule_path = `${config.rulesDirPath}${f_rule}`;
			let s_rule: string = await (await fs.promises.readFile(rule_path)).toString();
			let obj_rule: RuleProperties = JSON.parse(s_rule);

			if (obj_rule == undefined && obj_rule == null) {
				console.log(`Failed to parse rule ${rule_path} ... `);
				continue;
			}

			read_rules.push(obj_rule);
		}

		return read_rules;

	} catch (err) {
		console.log(`Failed to read rules from path: ${config.rulesDirPath}`);
		console.log(`Error: ${err}`);

		return [] as RuleProperties[];
	}

}

export default async function startRuleEngine(): Promise<number> {
	console.log(`Started rule engine with ${config.ruleEngineReadPeriod} interval ... `);

	let rules: RuleProperties[] = await readRules();
	engine = new Engine(rules);

	timer = setTimeout(timerEventHandler, config.ruleEngineReadPeriod);

	return rules.length;
}

async function timerEventHandler() {

	if (await Cache.getDataCount() == 0) {
		console.log("No data to process ... ");
		timer = setTimeout(timerEventHandler, config.ruleEngineReadPeriod);
		return;
	}

	let dataArray: ReaderData[] = await Cache.getCachedData();

	console.log(`Read: ${dataArray.length} records from cache ... `);

	for (let data of dataArray) {

		try {
			// this conversion is not needed apparently 
			// but yeah ... lets keep it
			let data_as_fact = new DataReaderFact("reader_data", data);
			let engine_result = await engine.run({ "reader_data": data_as_fact });

			for (let result of engine_result.results) {

				let fact: ReaderData = await engine_result.almanac.factValue('reader_data');
				let message = 'none';
				if (result.event != null &&
					result.event.params != null &&
					result.event.params.message != undefined) {
					message = result.event.params.message;
				}
				let rule_name = "";
				if (result.event != null) {
					rule_name = result.event.type;
				}

				let new_data_event: DataEvent = new DataEvent(
					new Date(),
					result.name,
					rule_name,
					message,
					fact
				);

				console.log(`data-event: ${new_data_event.shortPrint()}`)

				await sendDataEvent(new_data_event);
				await eventModel.insertOne(new_data_event);
			}

		} catch (err) {
			console.log(`Error during engine run ${JSON.stringify(err)} `);
		}
	}

	timer = setTimeout(timerEventHandler, config.ruleEngineReadPeriod);
}

process.on('exit', (code) => {
	clearTimeout(timer);
	if (engine != null && engine != undefined) {
		engine.stop();
	}
});
