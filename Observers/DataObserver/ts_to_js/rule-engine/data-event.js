"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var DataEvent = /** @class */ (function () {
    function DataEvent(time, ruleName, eventName, eventMessage, data) {
        this.time = time;
        this.ruleName = ruleName;
        this.eventName = eventName;
        this.eventMessage = eventMessage;
        this.processedData = data;
    }
    DataEvent.prototype.shortPrint = function () {
        var replacer = function (key, value) {
            if (key == "processedData") {
                return "big_reader_data";
            }
            else {
                return value;
            }
        };
        return JSON.stringify(this, replacer);
    };
    return DataEvent;
}());
exports.default = DataEvent;
//# sourceMappingURL=data-event.js.map