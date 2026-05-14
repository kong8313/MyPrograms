using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    class SuspendedCallsQuery : SuspendedInterviewIDsQuery
    {
        private readonly string _replicationTable;
        private string _additionalSelectParameters = string.Empty;

        public SuspendedCallsQuery(int surveySID, string replicationTable)
            : base(surveySID)
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
'' AS LastInterviewerName,
BvState.Name AS StateName,
BvInterview.LastCallTime,
ISNULL(CFinterview.[CallAttemptCount], 0) as AttemptNumber,
BvTimezone.[Name] AS TimezoneName,
BvTimezone.[ID] AS TimezoneID, 
BvInterview.[ReviewStatus] as ReviewStatus,
BvDialType.[Name] as DialTypeName,
BvInterview.[DialTypeId] as DialTypeId,
NULL AS Priority, 
NULL AS Time, 
NULL AS ExpireTime, 
NULL AS ApptTime,
0 AS CallID,
BvInterview.[DialingMode],
NULL AS ExpTime, 
'' AS ShiftType, 
0 AS Shift_ID,
'' AS Resource
FROM #ids
LEFT JOIN BvInterview on #ids.id = BvInterview.id and BvInterview.SurveySID = {m_SurveySid}
INNER JOIN BvState ON BvState.StateID = BvInterview.TransientState AND BvState.StateGroupID = {SurveyRepository.GetById(m_SurveySid).StateGroupID}
LEFT JOIN BvTimezone ON BvTimezone.ID = BvInterview.TimezoneID
LEFT JOIN BvDialType ON BvDialType.[ID] = BvInterview.DialTypeId
LEFT JOIN {_replicationTable} CFinterview ON CFinterview.respid = BvInterview.[ID] ORDER BY #ids.i";
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
