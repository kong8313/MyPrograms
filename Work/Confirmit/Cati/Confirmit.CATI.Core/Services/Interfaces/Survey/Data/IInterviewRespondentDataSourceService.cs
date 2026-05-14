namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IInterviewRespondentDataSourceService : IInterviewRespondentDataService
    {
        void Initialize(int surveyId, int interviewId);
        void Commit();
    }
}