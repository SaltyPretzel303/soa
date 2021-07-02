"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.deleteData = exports.updateData = exports.insertData = exports.getDataById = exports.getAllData = void 0;
var data_event_1 = __importDefault(require("../rule-engine/data-event"));
var dataModel = __importStar(require("../data/data-event-model"));
function getAllData(req, res, next) {
    dataModel.getAll()
        .then(function (data) {
        console.log("Get all result length: " + data.length);
        var clientReadyData = [];
        for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
            var record = data_1[_i];
            var singleData = new data_event_1.default(record.time, record.ruleName, record.eventName, record.eventMessage, record.processedData);
            clientReadyData.push(singleData);
        }
        res
            .type('application/json')
            .status(200)
            .send(clientReadyData);
    }).catch(function (error) {
        console.log("We got error from database ... \nError: " + error);
        res
            .status(500)
            .send("Database error ... ");
    });
}
exports.getAllData = getAllData;
;
function getDataById(req, res, next) {
    var reqId = req.query.dataId;
    dataModel.getById(reqId)
        .then(function (data) {
        if (data != null) {
            var clientReadyData = new data_event_1.default(data.time, data.ruleName, data.eventName, data.eventMessage, data.processedData);
            res
                .type('application/json')
                .status(200)
                .send(clientReadyData);
        }
        else {
            res
                .status(204)
                .send("No such record ... ");
        }
    }).catch(function (error) {
        console.log("We got error from database ... \nError: " + error);
        res
            .status(500)
            .send("Database error ... ");
    });
}
exports.getDataById = getDataById;
;
function insertData(req, res, next) {
    var newData = req.body;
    dataModel.insertOne(newData)
        .then(function (record) {
        var clientReadyResponse = new data_event_1.default(record.time, record.ruleName, record.eventName, record.eventMessage, record.processedData);
        res
            .status(200)
            .send(clientReadyResponse);
    }).catch(function (error) {
        console.log("We got error from database ... \nError: " + error);
        res
            .status(500)
            .send("Database error ... ");
    });
}
exports.insertData = insertData;
;
function updateData(req, res, next) {
    var id = req.body.id;
    var newValue = req.body.newValue;
    console.log("requesting data update of id: " + id);
    console.log(JSON.stringify(newValue));
    var message = "This method is still not implemented ... ";
    res
        .status(204)
        .send(message);
    return;
    dataModel.updateOne(id, newValue)
        .then(function (record) {
        if (record != null) {
            var clientReadyData = new data_event_1.default(record.time, record.ruleName, record.eventName, record.eventMessage, record.processedData);
            res
                .status(200)
                .send(clientReadyData);
        }
        else {
            res
                .status(204)
                .send("No such record, id: " + id);
        }
    }).catch(function (error) {
        console.log("We got error from database ... \nError: " + error);
        res
            .status(500)
            .send("Database error ... ");
    });
}
exports.updateData = updateData;
;
function deleteData(req, res, next) {
    var id = req.query.id;
    console.log("requesting data removal id: " + id);
    dataModel.removeById(id)
        .then(function (record) {
        if (record != null) {
            var clientReadyData = new data_event_1.default(record.time, record.ruleName, record.eventName, record.eventMessage, record.processedData);
            res
                .status(200)
                .send(clientReadyData);
        }
        else {
            res
                .status(204)
                .send("Failed to remove doc with id: " + id + " ... ");
        }
    }).catch(function (error) {
        console.log("We god an error from database\nError: " + error);
        res
            .status(500)
            .send("Database error ... ");
    });
}
exports.deleteData = deleteData;
;
//# sourceMappingURL=data-events-controller.js.map