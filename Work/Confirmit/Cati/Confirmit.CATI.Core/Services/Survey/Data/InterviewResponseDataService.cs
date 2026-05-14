using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services
{
    public class InterviewResponseDataService : IInterviewResponseDataService
    {
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly IInterviewDataServiceFactory _interviewDataServiceFactory;
        private readonly ISurveyRepository _surveyRepository;

        public InterviewResponseDataService(
            ISurveyMetadataCacheService surveyMetadataCacheService,
            IInterviewDataServiceFactory interviewDataServiceFactory,
            ISurveyRepository surveyRepository)
        {
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _interviewDataServiceFactory = interviewDataServiceFactory;
            _surveyRepository = surveyRepository;
        }

        public string GetInterviewVariableValue(string projectId, int interviewId, string variableName)
        {
            var survey = _surveyRepository.TryGetByProjectId(projectId);

            if (survey == null || survey.State == (int)SurveyState.SoftDeleted)
            {
                return null;
            }

            var formDescription = _surveyMetadataCacheService.Get(survey.SID).GetFormDesc(variableName);

            if (formDescription == null)
            {
                return null;
            }

            var respondentValue = _interviewDataServiceFactory.CreateFormService(survey.SID, interviewId)
                .GetFormValue(formDescription, null, new string[] { });

            return respondentValue;
        }
    }
}
