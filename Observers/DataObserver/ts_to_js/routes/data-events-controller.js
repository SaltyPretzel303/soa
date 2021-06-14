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
            var singleData = new data_event_1.default(record.time, record.priority, record.description);
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
            var clientReadyData = new data_event_1.default(data.time, data.priority, data.description);
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
        .then(function (addedData) {
        var clientReadyResponse = new data_event_1.default(addedData.time, addedData.priority, addedData.description);
        // TODO maybe instead returning object same as the one that is sent
        // create a new class which is gonna encapsulate that obj. and some message
        // it is unnecessary but pretty ... 
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
        .then(function (updatedData) {
        if (updatedData != null) {
            var clientReadyData = new data_event_1.default(updatedData.time, updatedData.priority, updatedData.description);
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
        .then(function (removedData) {
        if (removedData != null) {
            var clientReadyData = new data_event_1.default(removedData.time, removedData.priority, removedData.description);
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
