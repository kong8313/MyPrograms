using System;
using System.Diagnostics;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Misc
{
    public class RedialNumberSaver : IRedialNumberSaver
    {
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly IInterviewDataServiceFactory _surveyFormDataServiceFactory;
        private readonly IInterviewRepository _interviewRepository;

        private const string VariableName = "AlternativeNumber";

        public RedialNumberSaver(
            ISurveyMetadataCacheService surveyMetadataCacheService,
            IInterviewDataServiceFactory surveyFormDataServiceFactory,
            IInterviewRepository interviewRepository)
        {
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _surveyFormDataServiceFactory = surveyFormDataServiceFactory;
            _interviewRepository = interviewRepository;
        }

        public void SaveAlternativeNumber(int surveySid, string currentPhoneNumber, int interviewId)
        {
            var interview = _interviewRepository.GetById(surveySid, interviewId);

            if (interview == null ||
                interview.TelephoneNumber.Equals(currentPhoneNumber, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var formDescription = _surveyMetadataCacheService.Get(surveySid).GetFormDesc(VariableName);

            if (formDescription == null)
            {
                return;
            }

            if (formDescription.OPEN == false)
            {
                Trace.TraceError(
                    string.Format("Alternative redial number cannot be saved. Variable '{0}' is not Open",
                        VariableName));
                return;
            }

            var fieldWidth = formDescription.FieldWidth;
            if (fieldWidth > 0 && fieldWidth < currentPhoneNumber.Length)
            {
                currentPhoneNumber = currentPhoneNumber.Substring(0, fieldWidth);

                Trace.TraceError(
                    string.Format("'{0}' field width is less than number length. Only {1} symbols will be saved.",
                        VariableName, fieldWidth));
            }

            var dataSource = _surveyFormDataServiceFactory.CreateFormService(surveySid, interviewId);
            dataSource.SetFormValue(formDescription, null, new string[]{},  currentPhoneNumber);
            dataSource.Commit();

            Trace.TraceInformation("Alternative redial number '{0}' was saved for interviewId='{1}', surveySid='{2}'",
                currentPhoneNumber, interviewId, surveySid);
        }
    }
}
