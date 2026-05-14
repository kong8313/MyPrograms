using System.Diagnostics;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public class SurveyLaunchedWorker
    {
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;

        public SurveyLaunchedWorker(ISurveyMetadataCacheService surveyMetadataCacheService)
        {
            _surveyMetadataCacheService = surveyMetadataCacheService;
        }

        public void Execute(SurveyLaunchedNotification notification)
        {
            _surveyMetadataCacheService.ResetSurveyCache(notification.SurveyId);
            Trace.TraceInformation($"Survey metadata cache reset for survey {notification.SurveyId}");
        }
    }
}