var events = require("events");
var util = require("util");

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter);

const sql = require('mssql')

async function p(surveySID, config) {
    try {
        let pool = await sql.connect(config)
        let result = await pool.request()
            .query(`DECLARE @SID INT = ${surveySID}
                DECLARE @Query NVARCHAR(MAX) = 'SELECT BvInterview.[ID], BvInterview.[TransientState], BvState.[Name] as StateName
                FROM BvInterview
                                 LEFT JOIN BvState ON BvState.StateID = BvInterview.TransientState
                                               AND BvState.StateGroupID = 27
                WHERE BvInterview.SurveySID = ${surveySID}'
                EXEC BvSpReportSSS @SID, @Query`)
        sql.close();
        return result;

    } catch (err) {

    }
};

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

