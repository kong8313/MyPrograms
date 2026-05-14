namespace Confirmit.CATI.Core.Services.Survey
{
    public interface ISurveyStateService
    {
        void CloseSurvey(int sid);
        void ShutdownSurvey(int sid);
        void Open(int sid);
    }
}