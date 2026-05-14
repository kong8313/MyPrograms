using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    internal class ScheduledCallsQuery : ScheduledInterviewIDsQuery
    {
        private readonly string _replicationTable;
        private string _additionalSelectParameters = string.Empty;

        public ScheduledCallsQuery(int surveySID, string replicationTable)
            : base(surveySID)
        {
            _replicationTable = replicationTable;
        }

        public override string ToString()
        {
            return $@"create table #ids(id int primary key, i int identity(1, 1))
insert into #ids {base.ToString()} 
SELECT {_additionalSelectParameters}
BvCall.[InterviewID],
BvCall.[Priority], 
BvCall.[TimeInShift]  AS Time, 
CASE BvCall.[ExpireTime] WHEN '9999-01-01 00:00:00.000' THEN NULL ELSE BvCall.[ExpireTime] END AS ExpireTime, 
ISNULL(BvInterview.[TelephoneNumber], '' ) AS TelephoneNumber,
ISNULL(BvInterview.[RespondentName], '' ) AS RespondentName,
'' AS LastInterviewerName,
ISNULL(BvState.[Name],'')  AS StateName,
BvInterview.[LastCallTime],
BvInterview.[DialingMode],
BvAppointment.[Time] AS ApptTime,
BvAppointment.[ContactName],
ISNULL(CFinterview.[CallAttemptCount], 0 ) AS AttemptNumber,
BvCall.[ID] AS CallID,
BvAppointment.[ExpTime],
ISNULL(BvTimezone.[Name], '' ) AS TimezoneName,
ISNULL( BvShiftZones.[ShiftTypeID], BvCall.[ShiftTypeID] ) AS Shift_ID, 
BvTimezone.[ID] AS TimezoneID,
ISNULL(BvShiftType.[Name], '' ) AS ShiftType,
BvCall.[CallState],
ISNULL(BvViewPersonAndGroup.[Name], '') AS Resource,
BvInterview.[ReviewStatus] as ReviewStatus,
BvDialType.[Name] as DialTypeName,
BvInterview.[DialTypeId] as DialTypeId

FROM #ids
inner join BvSvySchedule AS BvCall on #ids.ID = BvCall.InterviewID AND BvCall.SurveySID = {m_SurveySid}
INNER JOIN BvInterview ON BvInterview.SurveySID = BvCall.SurveySID AND BvCall.InterviewID = BvInterview.[ID]   
LEFT JOIN BvState ON BvState.StateID = BvInterview.TransientState AND BvState.StateGroupID = {SurveyRepository.GetById(m_SurveySid).StateGroupID}
LEFT JOIN BvAppointment ON BvCall.ApptID = BvAppointment.[ID]  
LEFT JOIN BvTimezone ON BvTimezone.[ID] = BvInterview.TimezoneID  
LEFT JOIN BvShiftZones ON BvShiftZones.[ID] = BvCall.ShiftTypeID  
LEFT JOIN BvShiftType ON  BvShiftType.ObjectID = BvShiftZones.ShiftTypeID  
LEFT JOIN BvViewPersonAndGroup ON BvViewPersonAndGroup.SID = BvCall.ExplicitSID
LEFT JOIN BvDialType ON BvDialType.[ID] = BvInterview.DialTypeId
LEFT JOIN {_replicationTable} CFinterview ON CFinterview.respid = BvCall.InterviewID ORDER BY #ids.i";
        }

        public override void AddSelectParameter(ReplicatedColumn[] selectedColumns)
        {
            var selectClause = GenerateStringForAditionalSelectParameters(selectedColumns);

            _additionalSelectParameters = selectClause.ToString();

            if (selectClause.Length > 0)
                _additionalSelectParameters += ", ";
        }
    }
}
