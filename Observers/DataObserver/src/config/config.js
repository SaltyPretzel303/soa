const fs = require('fs');

// TODO somehow make some of the fields private
// some of the fields:
// - instance -> only accessed trough getConfig method
// - configListeners
// - constants
// ... 
class ConfigManager {

	#instance;
	static configListeners = new Array();

	static CONFIG_FILE_PATH = '../../service_config.json'
	static CONFIG_STAGE_FIELD = 'stage';

	// mapped fields

	rawConfig;
	stage;

	dbAddress;
	dbPort;
	dbName;

	static getInstance() {
		if (Config.instance == null) {
			Config.instance = Config.readConfig();
		}

		return Config.instance;
	}

	static readConfig() {
		if (!fs.existsSync(Config.CONFIG_FILE_PATH)) {
			return null;
		}

		let rawData = fs.readFileSync(Config.CONFIG_FILE_PATH, 'utf-8');
		console.log("FoundConfig: " + rawData);
		let jsonData = JSON.parse(rawData);

		// development or production
		let targetStage = jsonData[Config.CONFIG_STAGE_FIELD];

		let config = jsonData[targetStage];
		config.rawConfig = jsonData;
		config.stage = targetStage;

		return config;

	}

	reloadConfig(newConfig) {
		if (Config.configListeners.length > 0) {
			for (let listener of Config.configListeners) {
				listener.reload(newConfig);
			}
		}

		// TODO backup current configuration to db
		// this will require configuration mongoose schema

		let targetStage = newConfig[CONFIG_STAGE_FIELD];
		activeConfig = newConfig[targetStage];

		// async write new config to file
		fs.writeFile(CONFIG_FILE_PATH, JSON.stringify(newConfig), (error) => {
			if (error) {
				console.log("Error occurred while writing new configuration ... ");
				console.log(`Error: ${error}`);
			}
		});

	}

}
