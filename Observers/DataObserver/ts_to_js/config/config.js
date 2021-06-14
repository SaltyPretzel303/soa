"use strict";
// TODO this file should be removed ... ? 
var fs = require('fs');
// TODO somehow make some of the fields private
// some of the fields:
// - instance -> only accessed trough getConfig method
// - configListeners
// - constants
// ... 
var ConfigManager = /** @class */ (function () {
    function ConfigManager() {
        _instance.set(this, void 0);
    }
    ConfigManager.getInstance = function () {
        if (Config.instance == null) {
            Config.instance = Config.readConfig();
        }
        return Config.instance;
    };
    ConfigManager.readConfig = function () {
        if (!fs.existsSync(Config.CONFIG_FILE_PATH)) {
            return null;
        }
        var rawData = fs.readFileSync(Config.CONFIG_FILE_PATH, 'utf-8');
        console.log("FoundConfig: " + rawData);
        var jsonData = JSON.parse(rawData);
        // development or production
        var targetStage = jsonData[Config.CONFIG_STAGE_FIELD];
        var config = jsonData[targetStage];
        config.rawConfig = jsonData;
        config.stage = targetStage;
        return config;
    };
    ConfigManager.prototype.reloadConfig = function (newConfig) {
        if (Config.configListeners.length > 0) {
            for (var _i = 0, _a = Config.configListeners; _i < _a.length; _i++) {
                var listener = _a[_i];
                listener.reload(newConfig);
            }
        }
        // TODO backup current configuration to db
        // this will require configuration mongoose schema
        var targetStage = newConfig[CONFIG_STAGE_FIELD];
        activeConfig = newConfig[targetStage];
        // async write new config to file
        fs.writeFile(CONFIG_FILE_PATH, JSON.stringify(newConfig), function (error) {
            if (error) {
                console.log("Error occurred while writing new configuration ... ");
                console.log("Error: " + error);
            }
        });
    };
    var _instance;
    _instance = new WeakMap();
    ConfigManager.configListeners = new Array();
    ConfigManager.CONFIG_FILE_PATH = '../../service_config.json';
    ConfigManager.CONFIG_STAGE_FIELD = 'stage';
    return ConfigManager;
}());
