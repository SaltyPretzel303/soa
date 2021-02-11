const { json } = require('body-parser');
const fs = require('fs');
const { listenerCount } = require('process');

const CONFIG_FILE_PATH = '../service_config.json'
const CONFIG_STAGE_FIELD = 'stage';

let activeConfig;

let configListeners = new Array();

function readConfig() {

	if (!fs.existsSync(CONFIG_FILE_PATH)) {
		return null;
	}

	let rawData = fs.readFileSync(CONFIG_FILE_PATH, 'utf-8');
	let jsonData = JSON.parse(rawData);

	// development or production
	let targetStage = jsonData[CONFIG_STAGE_FIELD];

	return jsonData[targetStage];
}

function getConfig() {

	if (activeConfig == null) {
		activeConfig = readConfig();
	}

	return activeConfig;
}

function reloadConfig(newConfig) {

	if (configListeners.length > 0) {
		for (let listener of configListeners) {
			listener.reload(newConfig);
		}
	}

	// TODO backup current configuration to db
	// TODO this will requre configuration mongoose schema

	let targetStage = newConfig[CONFIG_STAGE_FIELD];
	activeConfig = newConfig[targetStage];

	// async write new config to file
	fs.writeFile(CONFIG_FILE_PATH, JSON.stringify(newConfig), (error) => {
		if (error) {
			console.log("Error occured while writing new configuration ... ");
			console.log(`Error: ${error}`);
		}
	});

}

function subscribeForReload(reloadListener) {
	configListeners.push(reloadListener);
}

module.exports.getConfig = getConfig;
module.exports.subscribeForReload = subscribeForReload;
module.exports.reloadConfig = reloadConfig;