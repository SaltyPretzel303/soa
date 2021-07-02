"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var json_rules_engine_1 = require("json-rules-engine");
var DataClassFact = /** @class */ (function (_super) {
    __extends(DataClassFact, _super);
    function DataClassFact(fact_id, data) {
        return _super.call(this, fact_id, data) || this;
    }
    return DataClassFact;
}(json_rules_engine_1.Fact));
exports.default = DataClassFact;
//# sourceMappingURL=data-reader-fact.js.map