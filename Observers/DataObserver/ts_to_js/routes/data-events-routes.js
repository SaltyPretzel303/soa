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
var controller = __importStar(require("./data-events-controller"));
function setupRoutes(app) {
    // get all saved dataEvents from database 
    app.get('/data/getAll', controller.getAllData);
    // get single dataEvent matched by id 
    app.get("/data/getSingle", controller.getDataById);
    // a whole body of request obj should be data that are gonna be saved
    app.post("/data/addDataEvent", controller.insertData);
    /*
        requires data in format:
        {
            id: "someRandomCharacters", // id of document to update
            newValue: "value that will replace existing document (if it exists)"
        }
    */
    app.post("/data/updateDataEvent", controller.updateData);
    // delete data matched by id
    app.delete("/data/deleteDataEvent", controller.deleteData);
}
exports.default = setupRoutes;
//# sourceMappingURL=data-events-routes.js.map