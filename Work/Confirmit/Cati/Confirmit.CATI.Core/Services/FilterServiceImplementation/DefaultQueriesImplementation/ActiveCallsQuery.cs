using Confirmit.CATI.Common;
namespace Confirmit.CATI.Core.Services.FilterServiceImplementation.DefaultQueriesImplementation
{
    internal class ActiveCallsQuery : BaseQuery
    {
        public ActiveCallsQuery(int surveySID, string replicationTable)
            : base(
                @"BvCall.[SurveySID] AS SurveySID, 
BvSurvey.[Name] AS SurveyName, 
BvCall.[InterviewID], 
BvCall.[Priority], 
BvCall.[TimeInShift] AS Time, 
CASE BvCall.[ExpireTime] 
   WHEN '9999-01-01 00:00:00.000' THEN NULL 
   ELSE BvCall.[ExpireTime] 
END AS ExpireTime, 
ISNULL(BvInterview.[TelephoneNumber], '' ) AS TelephoneNumber, 
ISNULL(BvInterview.[RespondentName], '' ) AS RespondentName, 
ISNULL(BvInterview.[TimezoneID], 0 ) AS TimezoneID, 
ISNULL(BvInterview.[TransientState], 0 ) AS TransientState, 
ISNULL(BvState.[Name],'') AS StateName, 
BvInterview.[LastCallTime],
BvInterview.[DialingMode],
BvAppointment.[Time] AS ApptTime, 
BvAppointment.[ContactName], 
ISNULL(CFinterview.[CallAttemptCount], 0 ) AS AttemptNumber,
BvCall.[ID] AS CallID, 
BvCall.[ApptID], 
ISNULL( BvShiftZones.[ShiftTypeID], 
BvCall.[ShiftTypeID] ) AS Shift_ID, 
BvCall.[CallState], 
BvAppointment.[ExpTime], 
ISNULL(BvTimezone.[Name], '' ) AS TimezoneName, 
ISNULL(BvShiftType.[Name], '' ) AS ShiftType, 
ISNULL(BvViewPersonAndGroup.[Name], '') AS Resource",
@"FROM BvCachedCalls AS cachedCalls WITH (NOLOCK)  
INNER JOIN BvSvySchedule BvCall ON cachedCalls.SurveySID = BvCall.SurveySID 
                              AND  BvCall.InterviewID = cachedCalls.[InterviewID]
INNER JOIN BvSurvey WITH (NOLOCK) ON BvSurvey.SID = BvCall.SurveySID 
INNER JOIN BvInterview WITH (NOLOCK) ON BvInterview.SurveySID = BvCall.SurveySID 
                              AND  BvCall.InterviewID = BvInterview.[ID]  
LEFT JOIN BvState WITH (NOLOCK) ON BvState.StateID = BvInterview.TransientState 
                       AND  BvState.StateGroupID = BvSurvey.StateGroupID  
LEFT  JOIN BvAppointment WITH (NOLOCK) ON BvCall.ApptID = BvAppointment.[ID]  
LEFT  JOIN BvTimezone WITH (NOLOCK) ON BvTimezone.[ID] = BvInterview.TimezoneID
LEFT JOIN BvShiftZones WITH (NOLOCK) ON BvShiftZones.[ID] = BvCall.ShiftTypeID  
LEFT JOIN BvShiftType WITH (NOLOCK) ON  BvShiftType.ObjectID = BvShiftZones.ShiftTypeID  
LEFT JOIN BvViewPersonAndGroup WITH (NOLOCK) ON  BvViewPersonAndGroup.SID = BvCall.ExplicitSID
LEFT JOIN " + replicationTable + @" CFinterview ON CFinterview.respid = BvCall.InterviewID",
                @"WHERE BvCall.SurveySID = " + surveySID + @"  AND 
(BvInterview.SurveySID = " + surveySID + @"  OR BvInterview.SurveySID  IS NULL )  AND 
BvCall.CallState > 0 ", TableTypes.Call | TableTypes.Appointment | TableTypes.CFVariables | TableTypes.Interview |
            TableTypes.ShiftType | TableTypes.Resource, surveySID)
        { }
    }
}