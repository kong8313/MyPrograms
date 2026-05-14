namespace Confirmit.CATI.Core.Misc
{
    public interface ISurveyConnectionStringProvider
    {
        SurveyConnectionInfo GetConnectionInfo(int surveyId, bool updateLastConnectionTime = true);
    }
}
