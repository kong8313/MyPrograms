namespace Confirmit.CATI.Core.Services.Interfaces.Survey.Data
{
    public interface IInterviewFormDataSourceService : IInterviewFormDataService
    {
        void Initialize(int surveyId, int interviewId);
        void Commit();
    }
}