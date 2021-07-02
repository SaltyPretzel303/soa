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
Object.defineProperty(exports, "__esModule", { value: true });
exports.getCacheCount = exports.getCachedData = exports.queueData = void 0;
var dbCache = __importStar(require("../data/reader-data-model"));
var cache = [];
function queueData(data) {
    cache.push(data);
    dbCache
        .SaveData(data)
        .then(function () {
        console.log('Data cached in db ... ');
    });
}
exports.queueData = queueData;
function getCachedData() {
    var tempArray = cache;
    cache = [];
    return tempArray;
}
exports.getCachedData = getCachedData;
function getCacheCount() {
    return cache.length;
}
exports.getCacheCount = getCacheCount;
