"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getCacheCount = exports.getCachedData = exports.queueData = void 0;
var cache = [];
function queueData(data) {
    cache.push(data);
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
