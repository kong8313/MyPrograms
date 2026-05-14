var events = require("events");
var util = require("util");
const XmlReader = require('xml-reader');
const _ = require("lodash")
const sql = require('mssql')

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter);

async function sqlRequest(surveySID, config) {
    try {
        const pool = await sql.connect(config);
        const result = await sql.query`SELECT [BvSchedule].[ScheduleID] as [ScheduleID]
        ,[XmlInUse]
        FROM [BvSchedule]
        inner join [BvSurvey] on [BvSurvey].ScheduleID = [BvSchedule].ScheduleID
        where [BvSurvey].[SID] = ${surveySID}`;
        sql.close();
        return result;
    } catch (err) {
        console.log(err)
    }
}

Command.prototype.command = function (res, output) {
    const {user, password, server, database} = this.api.globals.config;

    sqlRequest(res, {user, password, server, database}).then((result) => {
        const reader = XmlReader.create({stream: true});
        const parseXML = XmlReader.parseSync(result.recordset[0].XmlInUse);
        const customParams = _.find(parseXML.children, {name: "CustomParameters"})
            .children;

        let resultXML = [];
        customParams.forEach(arr => {
            let sqlRequest = {};
            arr.children.forEach(elements => {
                sqlRequest[elements.name] = elements.children.length > 0 ? elements.children[0].value : "";
            })

            resultXML.push(sqlRequest)
        })
        output(resultXML);
        this.emit("complete");
    });

    return this;
};

module.exports = Command;

