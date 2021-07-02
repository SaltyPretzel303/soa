import * as fs from 'fs'

import * as configDbModel from '../data/config-model'
import type IConfigListener from './i-config-listener'

class ServiceConfig {

	private static CONFIG_FILE_PATH = './service_config.json'
	private static PRODUCTION_VALUE = 'Production';
	private static DEVELOPMENT_VALUE = 'Development';

	stage: String;
	Development: ConfigFields;
	Production: ConfigFields;

	private static listeners: [IConfigListener];

	private static instance: ServiceConfig;
	public static GetInstance(): ConfigFields {

		if (ServiceConfig.instance == null) {
			ServiceConfig.readConfig();
		}

		let stage = ServiceConfig.instance.stage;
		if (stage == ServiceConfig.PRODUCTION_VALUE) {
			return ServiceConfig.instance.Production;
		} else if (stage == ServiceConfig.DEVELOPMENT_VALUE) {
			return ServiceConfig.instance.Development;
		} else {
			console.log("Stage field in config has inappropriate value ... ");
			return ServiceConfig.instance.Development;
		}
	}

	constructor(stage: String, dev: ConfigFields, prod: ConfigFields) {
		this.stage = stage;
		this.Development = dev;
		this.Production = prod;
	}

	private static readConfig(): void {
		let rawConfig = fs.readFileSync(ServiceConfig.CONFIG_FILE_PATH, 'utf-8');
		ServiceConfig.instance = JSON.parse(rawConfig);
	}

	public static subscribeForChange(newListener: IConfigListener): void {
		this.listeners.push(newListener);
	}

	public static async reloadConfig(newConfig: string): Promise<void> {

		let configObj = JSON.parse(newConfig);
		if (configObj == null) {
			console.log("New configuration can't be parsed ... ");
			return;
		}

		configDbModel.insertOne(ServiceConfig.instance);

		ServiceConfig.instance = configObj;

		for (let listener of this.listeners) {
			await listener.reload(ServiceConfig.instance);
		}
	}

}

// TODO whenever updating this class don't forget to update
// configFieldsSchema mongooseSchema in config-model.ts
// there is not really better solution :(

interface ConfigFields {
	listeningPort: number;
	dbAddress: string;
	dbPort: number;
	dbName: string;
	dbRetryTimeout: number;
	brokerAddress: string;
	brokerPort: number;
	ruleEngineReadPeriod: number;
	sensorDataTopic: string;
	sensorDataFilter: string;
	rulesDirPath: string;
	observingResultsTopic: string;
	observingResultsFilter: string;
}

export { ConfigFields };
export { ServiceConfig };