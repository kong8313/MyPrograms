using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;

namespace Confirmit.CATI.Core.Reports
{
    public interface ICallHistoryDataProvider
    {
        object[] PrepareForExport(CallHistoryDataEntity x);

        bool IncludeReplicatedVariables { get; set; }

        List<CallHistoryDataEntity> GetCallHistoryData(string surveyIds, DateTime? startTime, DateTime? endTime, string[] variables, bool includeBreakTimes, bool includeLoginLogoutInfo);

        List<CallHistoryDataEntity> GetCallHistoryData(string surveySIDs, DateTime? startTime, DateTime? endTime,
            string[] replicatedVariables);

        IEnumerable<CallHistoryDataEntity> GetPersonSessionHistoryData(int? callCenterId, DateTime? startTime,
            DateTime? finishTime);

        IEnumerable<CallHistoryDataEntity> GetInterviewerBreaksData(string surveySIDs, DateTime? startTime,
            DateTime? endTime);

        string GetHeader(string replicatedVariables);
    }
}