namespace Confirmit.CATI.Supervisor.Core.Persons
{
    public interface IChangeAutomaticSurveyService
    {
        bool ChangeSeamless(int personId, int surveyId);
    }
}
