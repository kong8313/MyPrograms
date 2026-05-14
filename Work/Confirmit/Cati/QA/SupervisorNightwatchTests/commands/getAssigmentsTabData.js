var events = require("events");
var util = require("util");

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter);

const sql = require('mssql')

async function p(surveySID, config) {
    try {
        const pool = await sql.connect(config);
        const result = await sql.query`exec BvSpAssignment_List ${surveySID}, 1`;
        sql.close();
        return result;
    } catch (err) {
        console.log(err)
    }
}

Command.prototype.command = function (res, f) {
    var me = this;
    const config = {
        user: this.api.globals.config.user,
        password: this.api.globals.config.password,
        server: this.api.globals.config.server,
        database: this.api.globals.config.database
    };
    p(res, config).then((result) => {
        f(result);
        me.emit("complete");
    });

    return this;
};

module.exports = Command;

