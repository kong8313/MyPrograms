using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System;
using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.QuotaBalancing.Tools;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.WaitingService;
using Confirmit.CATI.Core.Services.WaitingService.Fakes;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaBalancing
{
    [TestClass]
    public class QuotaBalancingTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize(false);
            _framework.BackendInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _backendTools = new BackendTools(_framework);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void QuotaBalancing_Create2ProjectsWithTheSameQuotas_MarkQuotaAsBalancedWorkedFine()
        {
            const int quotaId = 1;
            const int promotionThreshold = 10;
            const int promotionPriority = 10;

            var confirmitDb1 = ConfirmitTools.GetConfirmitSurveyDbOnClass(out var projectId1);
            var confirmitDb2 = ConfirmitTools.GetConfirmitSurveyDbOnClass(out var projectId2);

            var surveyId1 = _backendTools.CreateSurvey(projectId1, confirmitDb1.ConnectionString);
            _surveyStateService.Open(surveyId1);

            var surveyId2 = _backendTools.CreateSurvey(projectId2, confirmitDb2.ConnectionString);
            _surveyStateService.Open(surveyId2);

            var quota1 = TestQuota.Create(confirmitDb1,
                surveyId1,
                quotaId,
                new[] { "q1", "q2" },
                new[] { 2, 2 },
                new[] { 1, 5, 5, 5 },
                new[] { 10, 10, 10, 10 });

            TestQuota.Create(confirmitDb2,
                surveyId2,
                quotaId,
                new[] { "q1", "q2" },
                new[] { 2, 2 },
                new[] { 1, 5, 5, 5 },
                new[] { 10, 10, 10, 10 });

            quota1.MarkQuotaAsBalanced(promotionPriority, new[] { "q1", "q2" }, promotionThreshold);
        }
    }
}
