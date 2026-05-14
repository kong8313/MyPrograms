using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ISurveyDatabaseService
    {
        int IncrementCallAttemptCount(int surveyId, int interviewId);
        int GetCallAttemptCount(int surveyId, int interviewId);
        void UpdateIts(int surveyId, int interviewId, int its);
        void UpdateTimeZoneId(int surveyId, int interviewId, int timeZoneId);
        List<string> ProcessRespondentFieldsBatch(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields);
        List<string> ProcessCallHistoryLoopFieldsBatch(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields);
        string ProcessResponseField(int surveyId, int interviewId, BvHistoryCustomFieldsEntity field);
        List<string> GetCustomFieldValues(int surveySID, int interviewID);
    }
}