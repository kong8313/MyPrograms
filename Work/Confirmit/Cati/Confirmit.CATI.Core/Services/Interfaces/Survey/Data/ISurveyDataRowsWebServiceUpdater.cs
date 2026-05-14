using Confirmit.CATI.Core.Services.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface ISurveyDataRowsWebServiceUpdater
    {
        void Update(int surveyId, int interviewId, SurveyDataRowCache[] rows);
    }
}