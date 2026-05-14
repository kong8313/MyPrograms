using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    class AllInterviewsQuery : InterviewIDsQuery
    {
        private readonly string _replicationTable;
        private string _additionalSelectParameters = string.Empty;

        public AllInterviewsQuery(int surveySid, string replicationTable)
            : base(surveySid)
        {
            _replicationTable = replicationTable;
        }

        public override string ToString()
        {
            return $@"create table #ids(id int primary key, i int identity(1, 1))
insert into #ids {base.ToString()} SELECT {_additionalSelectParameters}
ISNULL(BvInterview.ID, CFInterview.respid) AS InterviewID,
BvInterview.TelephoneNumber,
BvInterview.RespondentName,
ISNULL(BvPerson.[Name], '' ) AS LastInterviewerName,
BvState.Name AS StateName, 
BvInterview.LastCallTime, 
BvInterview.[DialingMode],
BvAppointment.Time AS ApptTime, 
ISNULL(CFinterview.[CallAttemptCount], 0) as AttemptNumber,
BvAppointment.ExpTime, 
BvTimezone.[Name] AS TimezoneName, 
BvTimezone.[ID] AS TimezoneID, 
BvInterview.[ReviewStatus] as ReviewStatus,
BvDialType.[Name] as DialTypeName,
BvInterview.[DialTypeId] as DialTypeId,
CASE BvCall.[TimeInShift] WHEN '1899-12-30 00:00:00.000' THEN NULL ELSE BvCall.[TimeInShift] END AS Time,
BvCall.[ID] AS CallID,

0 AS Shift_ID,
'' AS ShiftType,
0 AS CallState,
'' AS Resource,
NULL AS ExpireTime
FROM #ids
LEFT JOIN BvInterview on #ids.Id = BvInterview.ID AND BvInterview.SurveySID = {m_SurveySid}
LEFT join BvSvySchedule AS BvCall on BvInterview.[ID] = BvCall.InterviewID AND BvCall.SurveySID = {m_SurveySid}
LEFT JOIN BvState ON BvState.StateID = BvInterview.TransientState AND BvState.StateGroupID = 
                   { SurveyRepository.GetById(m_SurveySid).StateGroupID}
LEFT JOIN BvAppointment ON BvInterview.ID = BvAppointment.[InterviewSID] and BvAppointment.SurveySid = BvInterview.SurveySID and BvAppointment.State = 1
LEFT JOIN BvTimezone ON BvTimezone.ID = BvInterview.TimezoneID  
LEFT JOIN BvPerson ON BvPerson.[SID] = BvInterview.LastCallPersonSID  
LEFT JOIN BvDialType ON BvDialType.[ID] = BvInterview.DialTypeId
LEFT JOIN {_replicationTable} CFinterview ON CFinterview.respid = #ids.[ID] ORDER BY #ids.i";
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
