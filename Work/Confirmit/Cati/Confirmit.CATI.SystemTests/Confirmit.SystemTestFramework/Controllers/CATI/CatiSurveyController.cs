using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Services;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers.CATI
{
    public class CatiSurveyController : TestController
    {
        private readonly ISurveyService _surveyService;
        private readonly string _pid;

        public CatiSurveyController(UserInfo userInfo, string pid, IRestClient restClient)
        {
            UserInfo = userInfo;
            _pid = pid;

            CallManagement = new CallManagementController(UserInfo, _pid);
            _surveyService = new SurveyService(restClient);
        }

        public CallManagementController CallManagement { get; set; }

        public void AssignSchedulingScript(string scriptId)
        {
            var basicProperties = _surveyService.GetBasicProperties(_pid).Result;
            basicProperties.Scheduling = scriptId;
            _surveyService.PutBasicProperties(basicProperties).Wait();
        }

        public async void Open()
        {
            await _surveyService.Open(_pid);
        }

        public void Close()
        {
            _surveyService.Close(_pid);
        }

        public int Sid => SurveyRepository.GetByName(_pid).SID;

        public void SetInboundBehavior(InboundSurveyBehavior behavior)
        {
            var survey = SurveyRepository.GetByName(_pid);
            survey.InboundBehavior = behavior;
            SurveyRepository.Update(survey);
        }
    }
}