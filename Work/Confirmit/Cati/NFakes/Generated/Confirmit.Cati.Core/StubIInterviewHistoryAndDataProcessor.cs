using System;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Tasks;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIInterviewHistoryAndDataProcessor : IInterviewHistoryAndDataProcessor 
    {
        private IInterviewHistoryAndDataProcessor _inner;

        public StubIInterviewHistoryAndDataProcessor()
        {
            _inner = null;
        }

        public IInterviewHistoryAndDataProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SaveHistoryAndControlDataBooleanInterviewHistoryDataInterviewControlDataBvInterviewTimingsBvSurveyEntityNullableOfInt32TaskContextBooleanNullableOfInt32Delegate(bool isSavedFromWrapup, InterviewHistoryData historyData, InterviewControlData controlData, BvInterviewTimings timings, BvSurveyEntity survey, int? LinkedIntervewiewSessionId, TaskContext previousContext, bool executeSchedulingScript, int? sessionId);
        public SaveHistoryAndControlDataBooleanInterviewHistoryDataInterviewControlDataBvInterviewTimingsBvSurveyEntityNullableOfInt32TaskContextBooleanNullableOfInt32Delegate SaveHistoryAndControlDataBooleanInterviewHistoryDataInterviewControlDataBvInterviewTimingsBvSurveyEntityNullableOfInt32TaskContextBooleanNullableOfInt32;

        void IInterviewHistoryAndDataProcessor.SaveHistoryAndControlData(bool isSavedFromWrapup, InterviewHistoryData historyData, InterviewControlData controlData, BvInterviewTimings timings, BvSurveyEntity survey, int? LinkedIntervewiewSessionId, TaskContext previousContext, bool executeSchedulingScript, int? sessionId)
        {

            if (SaveHistoryAndControlDataBooleanInterviewHistoryDataInterviewControlDataBvInterviewTimingsBvSurveyEntityNullableOfInt32TaskContextBooleanNullableOfInt32 != null)
            {
                SaveHistoryAndControlDataBooleanInterviewHistoryDataInterviewControlDataBvInterviewTimingsBvSurveyEntityNullableOfInt32TaskContextBooleanNullableOfInt32(isSavedFromWrapup, historyData, controlData, timings, survey, LinkedIntervewiewSessionId, previousContext, executeSchedulingScript, sessionId);
            } else if (_inner != null)
            {
                ((IInterviewHistoryAndDataProcessor)_inner).SaveHistoryAndControlData(isSavedFromWrapup, historyData, controlData, timings, survey, LinkedIntervewiewSessionId, previousContext, executeSchedulingScript, sessionId);
            }
        }

    }
}