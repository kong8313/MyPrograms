using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using Confirmit.CATI.Core.Timezones;
using System;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class MultySurveyCallActivationTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private FusionLibTestTools _fusionLibTestTools;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _fusionLibTestTools = new FusionLibTestTools(new BackendTools(_framework));

            var dbTools = new DatabaseTools(BackendInstance.Current.MasterConnectionString);
            if ( dbTools.IsDatabaseExists( ProjectId1 ) )
            {
                dbTools.DropDatabase( ProjectId1 );
            }

            if ( dbTools.IsDatabaseExists( ProjectId2 ) )
            {
                dbTools.DropDatabase( ProjectId2 );
            }

            _survey1Db = CreateSurveyDataBase(ProjectId1);
            _survey2Db = CreateSurveyDataBase(ProjectId2);

            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            _now = TimezoneManager.GetCurrentTimeByTzId(_timezoneId);
        }

        [TestCleanup]
        public void Cleanup()
        {
            new DatabaseTools(BackendInstance.Current.MasterConnectionString).DropDatabase(ProjectId2);
            new DatabaseTools(BackendInstance.Current.MasterConnectionString).DropDatabase(ProjectId1);

            _framework.TestCleanup();
        }

        

        private const string ProjectId1 = "survey_p01234561";
        private const string ProjectId2 = "survey_p01234562";
        private DatabaseEngine _survey1Db;
        private DatabaseEngine _survey2Db;
        private DateTime _now;
        private int _timezoneId;

        private DatabaseEngine CreateSurveyDataBase(string projectId)
        {
            new DatabaseTools(BackendInstance.Current.MasterConnectionString).CreateEmptyDatabase(projectId);
            var db = new DatabaseEngine(_framework.GetCatiSqlServerConnectionString(projectId));

            ConfirmitTools.CreateQuotaTables(db);

            return db;
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ActivateCall_SomeQuotasForDifferentSurveys_WeShouldCheckOnlyOurCells()
        {
            int surveySid1, surveySid2;
            int personSid1, personSid2;
            const int newPriority = 19;

            _fusionLibTestTools.CreateSurveyWithPersonForTest(
                SchedulingScriptType.Default, 
                _survey1Db.ConnectionString,
                out surveySid1, out personSid1);

            _fusionLibTestTools.CreateSurveyWithPersonForTest(
                SchedulingScriptType.Default,
                _survey2Db.ConnectionString,
                out surveySid2, out personSid2);

            List<BvInterviewEntity> interviewsForSurvey1 = FusionLibTestTools.CreateInterviewsForTest(surveySid1, new[] { 1, 3 }).ToList();
            List<BvInterviewEntity> interviewsForSurvey2 = FusionLibTestTools.CreateInterviewsForTest(surveySid2, new[] { 1, 4 }).ToList();

            var quota1 = TestQuota.Create(_survey1Db,
                surveySid1,
                1,
                new[] { "q1_1", "q1_2" },
                new [] { 2, 2 });

            var quota2 = TestQuota.Create(_survey2Db,
                surveySid2,
                1,
                new[] { "q2_1", "q2_2" },
                new [] { 2, 3 });

            const int openCellIdForQuota1 = 1;
            const int closeCellIdForQuota1 = 2;
            const int openCellIdForQuota2 = 1;
            const int closeCellIdForQuota2 = 2;

            quota1.PutInterviewsInCells(
                new[] { interviewsForSurvey1[0].ID, interviewsForSurvey1[1].ID },
                new[] { openCellIdForQuota1, closeCellIdForQuota1 });

            quota2.PutInterviewsInCells(
                new[] { interviewsForSurvey2[0].ID, interviewsForSurvey2[1].ID },
                new[] { closeCellIdForQuota2, openCellIdForQuota2 });

            quota1.CloseCell(closeCellIdForQuota1);
            quota2.CloseCell(closeCellIdForQuota2);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid1, (CancellationToken)default);
            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid2, (CancellationToken)default);
            
            new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid1,
                0 /*filterSid*/, //all scheduled calls
                newPriority,
                personSid1,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            Assert.IsTrue(BackendTools.IsCallExists(surveySid1, interviewsForSurvey1[0].ID),
                String.Format("Interview with id {0} and surveySid {1} was not activated", interviewsForSurvey1[0].ID, surveySid1));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid1, interviewsForSurvey1[1].ID),
                String.Format("Call for Interview with id {0} and surveySid {1} should not be activated", interviewsForSurvey1[1].ID, surveySid1));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid2, interviewsForSurvey2[0].ID),
                String.Format("Call for Interview with id {0} and surveySid {1} should not be activated", interviewsForSurvey2[0].ID, surveySid2));
            Assert.IsFalse(BackendTools.IsCallExists(surveySid2, interviewsForSurvey2[1].ID),
                String.Format("Call for Interview with id {0} and surveySid {1} should not be activated", interviewsForSurvey2[1].ID, surveySid2));
        }
    }
}
