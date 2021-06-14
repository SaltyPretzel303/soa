"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var service_configuration_1 = require("../config/service-configuration");
var mongoose_1 = __importDefault(require("mongoose"));
var config = service_configuration_1.ServiceConfig.GetInstance();
var dbAddress = config.dbAddress;
var dbPort = config.dbPort;
var dbName = config.dbName;
var dbUrl = "mongodb://" + dbAddress + ":" + dbPort + "/" + dbName;
var options = {
    useNewUrlParser: true,
    useUnifiedTopology: true,
};
var retryCounter = 0;
var retryTimeout = config.dbRetryTimeout;
var timer;
function createConnection() {
    console.log("Trying to connect to mongodb on: " + dbUrl);
    // default timeout (waiting time) for connection is 36s
    mongoose_1.default.connect(dbUrl, options)
        .then(function () {
        console.log("Successfully connected to mongodb on " + dbUrl);
    })
        .catch(function (error) {
        retryCounter++;
        console.log("Failed to connect to the mongodb on " + dbUrl);
        console.log("Reason: " + error);
        console.log("Retrying connection in " + retryTimeout + "ms (" + (retryCounter + 1) + " attempt)");
        // console.log(`Retry counter on: ${retryCounter}`);
        timer = setTimeout(createConnection, retryTimeout);
    });
}
exports.default = createConnection;
process.on('exit', function (code) {
    if (timer != null) {
        clearTimeout(timer);
    }
    mongoose_1.default.disconnect();
});
