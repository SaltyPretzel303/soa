import fs from 'fs'

import { getCachedData, getCacheCount } from './data-cache'
import { ConfigFields, ServiceConfig } from '../config/service-configuration'
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

function timerEventHandler() {
	if (getCacheCount() > 0) {
		let dataArray: ReaderData[] = getCachedData();
		let factsArray: DataReaderFact[] = [];

		console.log(`Read: ${dataArray.length} records from cache ... `);

		for (let data of dataArray) {
			let data_as_fact = new DataReaderFact("reader_data", data);

			// engine.addFact('reader_data', data_as_fact);
			engine.addFact<ReaderData>(data_as_fact);

			factsArray.push(data_as_fact);
		}

		/*
				NOTE		TODO		REFACTOR
			
			instead pulling all data together, passing them to engine and calling run 
			pull them on by one, pass it to engine with the same id/name, 
			wait for the result and then repeat until cache is empty ... 
			hopefully engine will be able to process all the cached events before 
			cache gets overflowed/tooBig 
			at that point maybe create two engines and run them parallel with 
			the same rules ... 

		*/

		// engine.run(factsArray)
		engine.run()
			.then(function (result: EngineResult) {

				console.log(`Got ${result.events.length} successful events from engine ... `);
				for (let event of result.events) {
					console.log(`\t type: ${event.type}`)
				}

				console.log(`Got ${result.failureEvents.length} failed events from engine ... `);
				for (let event of result.failureEvents) {
					console.log(`\t type: ${event.type}`);
				}

			})
			.catch(function (reason) {
				console.log(`Engine processing failed: ${reason}`);
			})
			.finally(() => {
				timer = setTimeout(timerEventHandler, config.ruleEngineReadPeriod);
			});

	} else {
		console.log("Engine's data cache is empty ... ");
	}

}

process.on('exit', (code) => {
	clearTimeout(timer);
	if (engine != null && engine != undefined) {
		engine.stop();
	}
});
