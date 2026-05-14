using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers.CATI
{
    public class CatiSurveysController : TestController
    {
        private readonly IRestClient _restClient;

        public CatiSurveysController(UserInfo userInfo, IRestClient restClient)
        {
            UserInfo = userInfo;
            _restClient = restClient;
        }

        public CatiSurveyController this[string pid] => new CatiSurveyController(UserInfo, pid, _restClient);
    }
}