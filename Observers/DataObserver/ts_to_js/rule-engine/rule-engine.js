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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
var fs_1 = __importDefault(require("fs"));
var service_configuration_1 = require("../config/service-configuration");
var reader_data_cache_1 = require("../data/reader-data-cache/reader-data-cache");
var json_rules_engine_1 = require("json-rules-engine");
var data_reader_fact_1 = __importDefault(require("./data-reader-fact"));
var data_event_1 = __importDefault(require("./data-event"));
var broker_sender_1 = require("../broker/broker-sender");
var eventModel = __importStar(require("../data/data-event-model"));
var config = service_configuration_1.ServiceConfig.GetInstance();
var timer;
var engine;
function readRules() {
    return __awaiter(this, void 0, void 0, function () {
        var read_rules, rule_files, _i, rule_files_1, f_rule, rule_path, s_rule, obj_rule, err_1;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    read_rules = [];
                    _a.label = 1;
                case 1:
                    _a.trys.push([1, 8, , 9]);
                    return [4 /*yield*/, fs_1.default.promises.readdir(config.rulesDirPath)];
                case 2:
                    rule_files = _a.sent();
                    console.log("Found " + rule_files.length + " rule files ... ");
                    _i = 0, rule_files_1 = rule_files;
                    _a.label = 3;
                case 3:
                    if (!(_i < rule_files_1.length)) return [3 /*break*/, 7];
                    f_rule = rule_files_1[_i];
                    rule_path = "" + config.rulesDirPath + f_rule;
                    return [4 /*yield*/, fs_1.default.promises.readFile(rule_path)];
                case 4: return [4 /*yield*/, (_a.sent()).toString()];
                case 5:
                    s_rule = _a.sent();
                    obj_rule = JSON.parse(s_rule);
                    if (obj_rule == undefined && obj_rule == null) {
                        console.log("Failed to parse rule " + rule_path + " ... ");
                        return [3 /*break*/, 6];
                    }
                    read_rules.push(obj_rule);
                    _a.label = 6;
                case 6:
                    _i++;
                    return [3 /*break*/, 3];
                case 7: return [2 /*return*/, read_rules];
                case 8:
                    err_1 = _a.sent();
                    console.log("Failed to read rules from path: " + config.rulesDirPath);
                    console.log("Error: " + err_1);
                    return [2 /*return*/, []];
                case 9: return [2 /*return*/];
            }
        });
    });
}
function startRuleEngine() {
    return __awaiter(this, void 0, void 0, function () {
        var rules;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    console.log("Started rule engine with " + config.ruleEngineReadPeriod + " interval ... ");
                    return [4 /*yield*/, readRules()];
                case 1:
                    rules = _a.sent();
                    engine = new json_rules_engine_1.Engine(rules);
                    timer = setTimeout(timerEventHandler, config.ruleEngineReadPeriod);
                    return [2 /*return*/, rules.length];
            }
        });
    });
}
exports.default = startRuleEngine;
function timerEventHandler() {
    return __awaiter(this, void 0, void 0, function () {
        var dataArray, _i, dataArray_1, data, data_as_fact, engine_result, _a, _b, result, fact, message, rule_name, new_data_event, err_2;
        return __generator(this, function (_c) {
            switch (_c.label) {
                case 0: return [4 /*yield*/, reader_data_cache_1.Cache.getDataCount()];
                case 1:
                    if ((_c.sent()) == 0) {
                        // console.log("No data to process ... ");
                        timer = setTimeout(timerEventHandler, config.ruleEngineReadPeriod);
                        return [2 /*return*/];
                    }
                    return [4 /*yield*/, reader_data_cache_1.Cache.getCachedData()];
                case 2:
                    dataArray = _c.sent();
                    console.log("Read: " + dataArray.length + " records from cache ... ");
                    _i = 0, dataArray_1 = dataArray;
                    _c.label = 3;
                case 3:
                    if (!(_i < dataArray_1.length)) return [3 /*break*/, 14];
                    data = dataArray_1[_i];
                    _c.label = 4;
                case 4:
                    _c.trys.push([4, 12, , 13]);
                    data_as_fact = new data_reader_fact_1.default("reader_data", data);
                    return [4 /*yield*/, engine.run({ "reader_data": data_as_fact })];
                case 5:
                    engine_result = _c.sent();
                    _a = 0, _b = engine_result.results;
                    _c.label = 6;
                case 6:
                    if (!(_a < _b.length)) return [3 /*break*/, 11];
                    result = _b[_a];
                    return [4 /*yield*/, engine_result.almanac.factValue('reader_data')];
                case 7:
                    fact = _c.sent();
                    message = 'none';
                    if (result.event != null &&
                        result.event.params != null &&
                        result.event.params.message != undefined) {
                        message = result.event.params.message;
                    }
                    rule_name = "";
                    if (result.event != null) {
                        rule_name = result.event.type;
                    }
                    new_data_event = new data_event_1.default(new Date(), result.name, rule_name, message, fact);
                    console.log("data-event: " + new_data_event.shortPrint());
                    return [4 /*yield*/, broker_sender_1.sendDataEvent(new_data_event)];
                case 8:
                    _c.sent();
                    return [4 /*yield*/, eventModel.insertOne(new_data_event)];
                case 9:
                    _c.sent();
                    _c.label = 10;
                case 10:
                    _a++;
                    return [3 /*break*/, 6];
                case 11: return [3 /*break*/, 13];
                case 12:
                    err_2 = _c.sent();
                    console.log("Error during engine run " + JSON.stringify(err_2) + " ");
                    return [3 /*break*/, 13];
                case 13:
                    _i++;
                    return [3 /*break*/, 3];
                case 14:
                    timer = setTimeout(timerEventHandler, config.ruleEngineReadPeriod);
                    return [2 /*return*/];
            }
        });
    });
}
process.on('exit', function (code) {
    clearTimeout(timer);
    if (engine != null && engine != undefined) {
        engine.stop();
    }
});
//# sourceMappingURL=rule-engine.js.map