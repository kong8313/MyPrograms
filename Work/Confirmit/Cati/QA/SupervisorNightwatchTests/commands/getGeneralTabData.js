var events = require("events");
var util = require("util");

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter);

const sql = require('mssql')

async function p(surveyID, config) {
    console.log("Getting data for", surveyID);
    try {
        const pool = await sql.connect(config);
        const result = await sql.query`select 
        [BvSurvey].Name as [SurveyName], 
        [SID],
        [Description], 
        [StateGroupID], 
        [BvStateGroup].Name as [StateGroupName], 
        [Target],
        [SurveySID],
        [BvSchedule].[ScheduleID] as [SchedulindID],
        [BvSchedule].Name as [ScheduleName],
        [ClusteredQuotaName],
        [IsRandomCallDeliveryEnabled],
        [ForceOpnRev],
        [IsTelephoneBlacklistSupported],
        [InterviewScreenRecording],
        [RecWholeInt],
        count(*) as Size
        from [BvSurvey] 
        JOIN [BvInterview] on [BvSurvey].SID = [BvInterview].SurveySID 
        JOIN [BvStateGroup] on [BvSurvey].StateGroupID = [BvStateGroup].ID 
        JOIN [BvSchedule] on [BvSurvey].ScheduleID = [BvSchedule].ScheduleID
        where [BvSurvey].Name = ${surveyID} 
        group by [BvSurvey].Name, 
        [SID],
        [Description], 
        [StateGroupID], 
        [BvStateGroup].Name,
        [Target],
        [SurveySID],
        [BvSchedule].[ScheduleID],
        [BvSchedule].Name,
        [ClusteredQuotaName],
        [IsRandomCallDeliveryEnabled],
        [ForceOpnRev],
        [IsTelephoneBlacklistSupported],
        [InterviewScreenRecording],
        [RecWholeInt]`;
        console.log(result)
        sql.close();
        return result;
    } catch (err) {
        console.log(err);
        return false;
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

