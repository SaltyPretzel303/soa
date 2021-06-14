"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var callback_api_1 = __importDefault(require("amqplib/callback_api"));
var service_configuration_1 = require("../config/service-configuration");
var config = service_configuration_1.ServiceConfig.Instance();
var url = "amqp://" + config.brokerAddress + ":" + config.brokerPort;
var connection;
function startBrokerReceiver() {
    callback_api_1.default.connect(url, function (err, conn) {
        if (err) {
            console.log("Failed to connect to broker on: " + url);
            console.log("Error: " + err);
            return;
        }
        console.log("Broker connection established on: " + url + " ... ");
        connection = conn;
        conn.createChannel(function (err, channel) {
            if (err) {
                console.log("Failed to create broker channel, error: " + err);
                return;
            }
            initReceiver(channel);
        });
    });
}
exports.default = startBrokerReceiver;
function initReceiver(chan) {
    // TODO add receivers for every route 
}
process.on('exit', function (code) {
    if (connection != null) {
        connection.close();
    }
});
