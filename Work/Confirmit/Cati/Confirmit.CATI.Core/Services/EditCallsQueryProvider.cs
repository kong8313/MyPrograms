using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class EditCallsQueryProvider : IEditCallsQueryProvider
    {
        /*
         General view of generated code in case when need to change all parameters
        Comments after '--' won't be generated

;WITH tempTable as 
(SELECT transArr.[ItemID] as IID, 
(SELECT CASE WHEN @ShiftTypeID = -1                                                              -- This select will be in such case if we need to change shiftType, if we don't need to change it – it will be always (SELECT 1) as temp1
       THEN -ISNULL( inter.[TimezoneID], 0 ) 
       ELSE CASE WHEN @ShiftTypeID > 0 
             THEN (SELECT shiftZones.[ID] FROM BvShiftZones shiftZones WHERE shiftZones.[ShiftTypeID] = @ShiftTypeID AND shiftZones.[TimeZoneID] = ISNULL( inter.[TimezoneID], 0 ) ) 
             ELSE @ShiftTypeID 
             END
       END ) as ShiftTypeId,
(SELECT state.[FcdAction] FROM [BvState] state, [BvInterview] inter WHERE inter.[SurveySID] = @SurveySID AND inter.[ID] = transArr.[ItemID] AND inter.[TransientState] = state.[StateId] AND state.[StateGroupId] = @StateGroupId) as FcdAction,
(SELECT CASE WHEN 1<>1 THEN 1 ELSE 0 END) as IsClosed,                                             -- These 2 selects will be in such case if we need to change callState, if we don't need to change it – it will be (SELECT 1) as temp2
(SELECT ISNULL(dbo.GetTZBias(@TimeInShift, CASE WHEN ISNULL(inter.TimezoneID, 0) = 0 THEN @DefaultTZID ELSE TimeZoneID END), 0) ) as TimeToCallBias,           -- This select will be in such case if we need to change timeToCall, if we don't need to change it – it will be (SELECT 1) as temp3
(SELECT ISNULL(dbo.GetTZBias(@ExpireTime, CASE WHEN ISNULL(inter.TimezoneID, 0) = 0 THEN @DefaultTZID ELSE TimeZoneID END), 0) ) as TimeToExpireBias           -- This select will be in such case if we need to change timeToExpire, if we don't need to change it – it will be (SELECT 1) as temp4
FROM [BvTransferArrays] transArr
LEFT JOIN BvReplicatedData_34 as repl ON repl.[respid] = transArr.[ItemID]
LEFT JOIN [BvInterview] inter ON inter.[ID] = transArr.[ItemID] AND inter.[SurveySID] = @SurveySID
WHERE transArr.[BatchID] = @BatchID   )           

UPDATE BvSvySchedule SET 
    TimeInShift = CASE WHEN @TimeInShift = '1899-12-30' THEN @TimeInShift ELSE DATEADD(minute, tempTable.[TimeToCallBias], @TimeInShift) END,            –- This row exists if we need to change timeToCall                                     
    ExpireTime = CASE WHEN @ExpireTime = '9999-01-01' THEN @ExpireTime ELSE DATEADD(minute, tempTable.[TimeToExpireBias], @ExpireTime) END,              –- This row exists if we need to change timeToExpire                                     
    CallState = CASE WHEN tempTable.[FcdAction] = 1 OR tempTable.IsClosed = 0 OR @CallState = 3 THEN @CallState ELSE @FcdBehaviorAlgorithm END,    –- This row exists if we need to change callState
    Priority = @Priority, OldPriority = 0,                                                        –- This row exists if we need to change priority                                     
    ShiftTypeID = tempTable.[ShiftTypeID],                                                        –- This row exists if we need to change shiftType                                           

    Type = [Type]                                                                                 -- This row will be always in such case even if we don't need to change other fields. It has been done to fill #InterviewIds table and providing a row into CallHistory                                     
    OUTPUT inserted.[InterviewID]
    INTO #InterviewIds
       FROM tempTable 
       WHERE BvSvySchedule.[SurveySID] = @SurveySid AND BvSvySchedule.[InterviewID] = tempTable.[IID]  AND BvSvySchedule.[CallState] > 0

UPDATE [BvInterview] SET                                                                                    -- This update query exists if we need to change dialingMode or extendedStatus
    [DialingMode] = @DialingMode,                                                                           –- This row exists if we need to change dialingMode
    [TransientState] = @ExtendedStatus                                                                      –- This row exists if we need to change extendedStatus
       FROM #InterviewIds interIds 
       WHERE BvInterview.[SurveySID] = @SurveySID AND BvInterview.[ID] = interIds.[Id]

SELECT @@ROWCOUNT as ProcessedCalls

         */

        private readonly ITimezoneService _timezoneService;

        public EditCallsQueryProvider(ITimezoneService timezoneService)
        {
            _timezoneService = timezoneService;
        }

        private string GetSelectForShiftTypeId(int? shiftType)
        {
            if (shiftType.HasValue)
            {
                return @"(SELECT CASE WHEN @ShiftTypeID = -1 
	THEN -ISNULL( inter.[TimezoneID], 0 ) 
	ELSE CASE WHEN @ShiftTypeID > 0 
		THEN (SELECT shiftZones.[ID] FROM BvShiftZones shiftZones WHERE shiftZones.[ShiftTypeID] = @ShiftTypeID AND shiftZones.[TimeZoneID] = ISNULL( inter.[TimezoneID], 0 ) ) 
		ELSE @ShiftTypeID 
		END
	END ) as ShiftTypeId";
            }

            return "(SELECT 1) as temp1";
        }

        private string GetSelectsForFcdActionAndIsClosed(int? callState, int? extendedStatus)
        {
            if (callState.HasValue)
            {
                string getFcdActionQuery = extendedStatus.HasValue
                    ? "(SELECT state.[FcdAction] FROM [BvState] state WHERE state.[StateId] = @ExtendedStatus AND state.[StateGroupId] = @StateGroupId) as FcdAction,"
                    : "(SELECT state.[FcdAction] FROM [BvState] state, [BvInterview] inter WHERE inter.[SurveySID] = @SurveySID AND inter.[ID] = transArr.[ItemID] AND inter.[TransientState] = state.[StateId] AND state.[StateGroupId] = @StateGroupId) as FcdAction,";
                
                return getFcdActionQuery + "\r\n(SELECT CASE WHEN qcell.CellID IS NOT NULL THEN 1 ELSE 0 END) as IsClosed";
            }

            return "(SELECT 1) as temp2";
        }

        private string GetSelectForTimeToCallBias(DateTime? timeToCall)
        {
            if (timeToCall.HasValue)
            {
                return "(SELECT ISNULL(dbo.GetTZBias(@TimeInShift, CASE WHEN ISNULL(inter.[TimezoneID], 0) = 0 THEN @DefaultTZID ELSE inter.[TimeZoneID] END), 0) ) as TimeToCallBias";
            }

            return "(SELECT 1) as temp3";
        }

        private string GetSelectForTimeToExpireBias(DateTime? timeToExpire)
        {
            if (timeToExpire.HasValue)
            {
                return "(SELECT ISNULL(dbo.GetTZBias(@ExpireTime, CASE WHEN ISNULL(inter.[TimezoneID], 0) = 0 THEN @DefaultTZID ELSE inter.[TimeZoneID] END), 0) ) as TimeToExpireBias";
            }

            return "(SELECT 1) as temp4";
        }

        private string GetBvInterviewJoinIfNeeded(DateTime? timeToCall, DateTime? timeToExpire, int? shiftType)
        {
            if (timeToCall.HasValue || timeToExpire.HasValue || shiftType.HasValue)
            {
                return "LEFT JOIN [BvInterview] inter ON inter.[ID] = transArr.[ItemID] AND inter.[SurveySID] = @SurveySID";
            }

            return string.Empty;
        }
        
        private string GetUpdateCallSets(DateTime? timeToCall, DateTime? timeToExpire, int? callState, int? callPriority, int? shiftType)
        {
            var updateCallSets = new StringBuilder();
            if (timeToCall.HasValue)
            {
                updateCallSets.AppendLine("    TimeInShift = CASE WHEN @TimeInShift = '1899-12-30' THEN @TimeInShift ELSE DATEADD(minute, tempTable.[TimeToCallBias], @TimeInShift) END,");
            }

            if (timeToExpire.HasValue)
            {
                updateCallSets.AppendLine("    ExpireTime = CASE WHEN @ExpireTime = '9999-01-01' THEN @ExpireTime ELSE DATEADD(minute, tempTable.[TimeToExpireBias], @ExpireTime) END,");
            }

            if (callState.HasValue)
            {
                updateCallSets.AppendLine("    CallState = CASE WHEN tempTable.[FcdAction] = 1 OR tempTable.IsClosed = 0 OR @CallState = 3 THEN @CallState ELSE @FcdBehaviorAlgorithm END,");
            }

            if (callPriority.HasValue)
            {
                updateCallSets.AppendLine("    Priority = @Priority, OldPriority = 0,");
            }

            if (shiftType.HasValue)
            {
                updateCallSets.AppendLine("    ShiftTypeID = tempTable.[ShiftTypeID],");
            }

            return updateCallSets.ToString();
        }

        private string GetUpdateBvInterviewQuery(byte? dialingMode, int? extendedStatus)
        {
            var updateInterviewSets = new List<string>();
            if (dialingMode.HasValue)
            {
                updateInterviewSets.Add("    [DialingMode] = @DialingMode");
            }

            if (extendedStatus.HasValue)
            {
                updateInterviewSets.Add("    [TransientState] = @ExtendedStatus");
            }

            if (updateInterviewSets.Count > 0)
            {
                return $@"UPDATE [BvInterview] SET 
{string.Join(",\r\n", updateInterviewSets)}
	FROM #InterviewIds interIds 
	WHERE BvInterview.[SurveySID] = @SurveySID AND BvInterview.[ID] = interIds.[Id]";
            }

            return string.Empty;
        }

        public string GetQuery(
            int surveySid,
            int batchId,
            DateTime? timeToCall,
            DateTime? timeToExpire,
            int? callState,
            int? callPriority,
            int? shiftType,
            int? extendedStatus,
            byte? dialingMode,
            int fcdBehaviorAlgorithm,
            string whereCondition,
            int stateGroupId)
        {
            return $@"
;WITH tempTable as 
(SELECT transArr.[ItemID] as IID, 
{GetSelectForShiftTypeId(shiftType)},
{GetSelectsForFcdActionAndIsClosed(callState, extendedStatus)},
{GetSelectForTimeToCallBias(timeToCall)},
{GetSelectForTimeToExpireBias(timeToExpire)}
FROM [BvTransferArrays] transArr
LEFT JOIN BvReplicatedData_{surveySid} as repl ON repl.[respid] = transArr.[ItemID]
{GetBvInterviewJoinIfNeeded(timeToCall, timeToExpire, shiftType)}
LEFT JOIN BvInterviewQuotaCell AS icell
ON icell.SurveyId = {surveySid} AND icell.InterviewId = repl.respid
LEFT JOIN BvSurveyQuotaCell AS qcell
ON icell.SurveyID = qcell.SurveyID AND icell.QuotaID = qcell.QuotaID AND icell.CellID = qcell.CellID AND qcell.IsOpen = 0
WHERE transArr.[BatchID] = @BatchID)

UPDATE BvSvySchedule SET 
{GetUpdateCallSets(timeToCall, timeToExpire, callState, callPriority, shiftType)} 
    Type = [Type]
    OUTPUT inserted.[InterviewID]
    INTO #InterviewIds
	FROM tempTable 
	WHERE BvSvySchedule.[SurveySID] = @SurveySid AND BvSvySchedule.[InterviewID] = tempTable.[IID]  AND BvSvySchedule.[CallState] > 0

{GetUpdateBvInterviewQuery(dialingMode, extendedStatus)}

SELECT @@ROWCOUNT as ProcessedCalls";
        }

        public IEnumerable<SqlParameter> GetSqlParameters(
            int surveySid,
            int batchId,
            DateTime? timeToCall,
            DateTime? timeToExpire,
            int? callState,
            int? callPriority,
            int? shiftType,
            int? extendedStatus,
            byte? dialingMode,
            int fcdBehaviorAlgorithm,
            int stateGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("SurveySID", surveySid),
                new SqlParameter("BatchID", batchId)
            };

            if (timeToCall.HasValue || timeToExpire.HasValue)
            {
                parameters.Add(new SqlParameter("DefaultTZID", _timezoneService.GetDefaultCallCenterTimezoneId()));

                if (timeToCall.HasValue)
                {
                    if (timeToCall == DateTime.MinValue)
                    {
                        timeToCall = CallQueueService.DefaultTimeInShift;
                    }

                    var timeToCallWithoutMs = timeToCall.Value.AddTicks(-(timeToCall.Value.Ticks % TimeSpan.TicksPerSecond));
                    parameters.Add(new SqlParameter("TimeInShift", timeToCallWithoutMs));
                }

                if (timeToExpire.HasValue)
                {
                    var timeToExpireWithoutMs = timeToExpire.Value.AddTicks(-(timeToExpire.Value.Ticks % TimeSpan.TicksPerSecond));
                    parameters.Add(new SqlParameter("ExpireTime", timeToExpireWithoutMs));
                }
            }

            if (callState.HasValue)
            {
                parameters.Add(new SqlParameter("CallState", callState.Value));
                parameters.Add(new SqlParameter("FcdBehaviorAlgorithm", fcdBehaviorAlgorithm));
                parameters.Add(new SqlParameter("StateGroupId", stateGroupId));
            }

            if (callPriority.HasValue)
            {
                parameters.Add(new SqlParameter("Priority", callPriority.Value));
            }

            if (shiftType.HasValue)
            {
                parameters.Add(new SqlParameter("ShiftTypeID", shiftType.Value));
            }

            if (dialingMode.HasValue)
            {
                parameters.Add(new SqlParameter("DialingMode", dialingMode.Value));
            }

            if (extendedStatus.HasValue)
            {
                parameters.Add(new SqlParameter("ExtendedStatus", extendedStatus.Value));
            }

            return parameters;
        }
    }
}