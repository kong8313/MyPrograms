namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public interface ISurveyMetadataCacheService
    {
        void ResetSurveyCache(int surveyId);
        ISurveyMetadataCache Get(int surveyId);
    }
}