using Confirmit.CATI.Core.Services.Survey.Data;

namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface ISurveyDataRowsDatabaseUpdater
    {
        bool Update(int surveyId, int interviewId, SurveyDataRowCache[] rows);
        bool Process(int surveyId, int interviewId, SurveyDataRowCache[] rows);
    }
}
