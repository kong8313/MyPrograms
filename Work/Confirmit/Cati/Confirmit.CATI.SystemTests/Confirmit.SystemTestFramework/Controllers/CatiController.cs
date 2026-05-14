using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.SystemTestFramework.Controllers.CATI;
using Confirmit.SystemTestFramework.Settings;

namespace Confirmit.SystemTestFramework.Controllers
{
    public class CatiController : TestController
    {
        public CatiSurveysController Surveys { get; }

        public SchedulingController Scheduling { get; }

        public InterviewersController Interviewers { get; }

        public ActivityViewsController ActivityViews { get; }

        public InterviewsController Interviews { get; }

        public IRestClient RestClient { get; }

        public CatiController(UserInfo userInfo)
        {
            UserInfo = userInfo;
            RestClient = RestClientFactory.Create(UserInfo);

            Scheduling = new SchedulingController(UserInfo);
            Surveys = new CatiSurveysController(UserInfo, RestClient);
            Interviewers = new InterviewersController(UserInfo);
            ActivityViews = new ActivityViewsController(UserInfo);
            Interviews = new InterviewsController(UserInfo);            
        }
    }
}