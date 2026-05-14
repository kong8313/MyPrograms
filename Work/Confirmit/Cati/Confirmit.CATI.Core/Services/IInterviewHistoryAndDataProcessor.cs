using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Tasks;

namespace Confirmit.CATI.Core.Services
{
    public interface IInterviewHistoryAndDataProcessor
    {
        void SaveHistoryAndControlData(bool isSavedFromWrapup, InterviewHistoryData historyData, InterviewControlData controlData, BvInterviewTimings timings, BvSurveyEntity survey, int? LinkedIntervewiewSessionId, TaskContext previousContext, bool executeSchedulingScript, int? sessionId);
    }
}