var events = require("events");
var util = require("util");
const sql = require('mssql')

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter);

async function sqlRequest(surveySID, config) {
    try {
        const pool = await sql.connect(config);
        const result = await sql.query`SELECT [SID]
        ,[Name]
        ,[Description]
        ,[AndOrOperator]
        ,[SurveySID]
        ,[Hidden]
        FROM [BvFilters]`;
        sql.close();
        return result;
    } catch (err) {
        console.log(err)
    }
}

Command.prototype.command = function (res, output) {
    const { user, password, server, database } = this.api.globals.config;

    sqlRequest(res, { user, password, server, database }).then((result) => {
        output(result);
        this.emit("complete");
    });

    return this;
};

module.exports = Command;

