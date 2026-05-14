using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services
{
    public class TimeBreaksHistoryService
    {
        public static void FinishInterviewerBreak(int personId)
        {
            BvSpFinishInterviewerBreakAdapter.ExecuteNonQuery(personId);
        }

        public static DateTime? GetStartBreakTime(int personId)
        {
            return BvSpGetInterviewerActiveBreakAdapter.ExecuteEntity(personId).StartTime;
        }

        /// <summary>
        /// Gets break history data for selected surveys
        /// </summary>
        /// <param name="surveySIDs">String with surveys IDs separated by ','</param>
        /// <param name="startTime">Start time for break history data</param>
        /// <param name="endTime">End time for break history data</param>
        public static List<BvSpGetInterviewerBreaksEntity> GetInterviewerBreaks(string surveySIDs, DateTime? startTime, DateTime? endTime)
        {
            int maxRows = ServiceLocator.Resolve<ISystemSettings>().Reports.CallHistoryReportInterviewerBreaksRowsLimit;

            return BvSpGetInterviewerBreaksAdapter.ExecuteEntityList(startTime, endTime, surveySIDs, maxRows);
        }
    }
}
