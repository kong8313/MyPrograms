using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CallCenter
{
    [TestClass]
    public class SurveyToCallCenterAssignmentProviderTest
    {
        readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private ISurveyToCallCenterAssignmentProvider _assignProvider;
        private ISurveyRepository _surveyRepository;
        private BackendTools _backendTools;
        private string _supervisorName;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _supervisorName = "SuperName";

            _framework.TestInitialize();
            _framework.BackendInitialize(false);
            _backendTools = new BackendTools(_framework);

            new SupervisorCallCentersRegistry().RegisterTypes(ServiceLocator.Resolve<IServiceRegistrator>());

            _assignProvider = ServiceLocator.Resolve<ISurveyToCallCenterAssignmentProvider>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetPage_SurveysWithAndWithoutPermissions_OnlySurveysWithPermissionsAreReturned()
        {
            const string permittedProjectId = "p123456";
            var permittedSurveyId = _backendTools.CreateSurvey(permittedProjectId, true);
            _backendTools.CreateSurvey("p654321", true);

            new ManagementService().UpdateSurveyAccessList(_supervisorName, permittedProjectId, true);

            int totalCount;
            var pagingArgs = new PagingArgs(
               1 /*PageIndex*/,
               20 /*PageSize*/,
               "SurveyId" /*SortedColumnKey*/,
               true /*SortIndicatorAsc*/);
            var result = _assignProvider.GetPage(_supervisorName, pagingArgs, out totalCount);

            Assert.AreEqual(1, result.Count(), "Invalid number of surveys");
            Assert.AreEqual(permittedSurveyId, result.ElementAt(0).SurveyId, "Wrong survey is returned");
        }
    }
}
