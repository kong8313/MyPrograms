using System;
using System.Diagnostics;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Misc
{
    public class LanguageVariableProvider : ILanguageVariableProvider
    {
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly IInterviewDataServiceFactory _interviewDataServiceFactory;

        private const string LanguageVariableName = "language";

        public LanguageVariableProvider(
            ISurveyMetadataCacheService surveyMetadataCacheService,
            IInterviewDataServiceFactory interviewDataServiceFactory)
        {
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _interviewDataServiceFactory = interviewDataServiceFactory;
        }

        public int? GetLanguageForInterview(int surveySid, int interviewId)
        {
            string languageVariableValue;

            try
            {
                var fieldDesc = _surveyMetadataCacheService.Get(surveySid).GetRespondentFieldDesc(LanguageVariableName);

                EventDetailsScope.Current.AddTiming("GetLanguageForInterview:_surveyMetadataCacheService.Get");

                if (fieldDesc == null)
                {
                    return null;
                }

                var respondentValue = _interviewDataServiceFactory.CreateRespondentService(surveySid, interviewId)
                    .GetRespondentValue(LanguageVariableName);

                EventDetailsScope.Current.AddTiming("GetLanguageForInterview:_interviewDataServiceFactory.CreateRespondentService");

                languageVariableValue = respondentValue != null ? respondentValue.ToString() : null;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                return null;
            }

            if (string.IsNullOrEmpty(languageVariableValue))
            {
                return null;
            }

            int result;
            if (int.TryParse(languageVariableValue, out result))
            {
                return result;
            }

            Trace.TraceWarning(
                "Language variable does not contain valid language id for interviewId='{0}', surveySid='{1}'",
                interviewId, surveySid);
            return null;
        }
    }
}
