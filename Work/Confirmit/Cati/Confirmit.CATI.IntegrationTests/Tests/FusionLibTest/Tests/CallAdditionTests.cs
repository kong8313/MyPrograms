using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class CallAdditionTests : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddCall_CallDoesNotExist_CallCreated()
        {
            FusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default,
                out var surveySid,
                out var personSid);

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new int[] { 1 }).ToList();
            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            var call = new BvCallEntity 
            { 
                SurveySID = surveySid, 
                InterviewID = interviews[0].ID
            };

            CallManager.AddCall(call);

            BackendTools.LoginPerson(personSid, "");

            Assert.IsTrue(BackendTools.IsCallExists(surveySid, interviews[0].ID));

            // check if calls are given by LookupByPersonSID
            BvTasksEntity task = TaskService.LookupByPersonSid(personSid, surveySid);
            Assert.AreEqual(CallQueueService.GetCallAndNoLock(surveySid, interviews[0].ID).CallID, task.CallID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddCall_InterviewInClosedCell_CallsIsNotAdded()
        {
            FusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default,
                out var surveySid,
                out _);

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1 }).ToList();

            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 3 });

            const int cellId = 2;

            quota.PutInterviewsInCells(
                new[] { interviews[0].ID },
                new[] { cellId });

            quota.CloseCell(cellId);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            var call = new BvCallEntity
            {
                SurveySID = surveySid,
                InterviewID = interviews[0].ID
            };

            CallManager.AddCall(call);

            Assert.IsFalse(BackendTools.IsCallExists(surveySid, interviews[0].ID),
                "Calls should not be created");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddCall_ManyQuotasHasManyLongColumns_AllDynamicQueryShouldBePerformedCorrectly()
        {
            var columnsForQuota1 = new[]
            {
                "A1234567890123456789012345678901A",
                "A1234567890123456789012345678901B",
                "A1234567890123456789012345678901C",
                "A1234567890123456789012345678901D",
                "A1234567890123456789012345678901E"  
            };
            var columnsForQuota2 = new[]
            {
                "A234567890123456789012345678901F",
                "A234567890123456789012345678901G",
                "A234567890123456789012345678901H",
                "A234567890123456789012345678901J",
                "A234567890123456789012345678901K"
            };
            var columnsForQuota3 = new[]
            {
                "A234567890123456789012345678901L",
                "A234567890123456789012345678901M",
                "A234567890123456789012345678901N",
                "A234567890123456789012345678901O",
                "A234567890123456789012345678901P"
            };
            var columnsForQuota4 = new[]
            {
                "A234567890123456789012345678901Q",
                "A234567890123456789012345678901R",
                "A234567890123456789012345678901S",
                "A234567890123456789012345678901T",
                "A234567890123456789012345678901U"
            };

            var answerCountsForQuota1 = new[] { 2, 2, 2, 3, 2 };
            var answerCountsForQuota2 = new[] { 2, 3, 4, 2, 2 };
            var answerCountsForQuota3 = new[] { 2, 2, 2, 2, 2 };
            var answerCountsForQuota4 = new[] { 2, 2, 3, 3, 2 };

            FusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default,
                out var surveySid,
                out _);

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(surveySid, new[] { 1 }).ToList();

            //there are should be 2*3*2 = 12 cells
            var quota1 = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                1,
                columnsForQuota1,
                answerCountsForQuota1);

            //there are should be 2*3*2 = 12 cells
            var quota2 = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                2,
                columnsForQuota2,
                answerCountsForQuota2);

            //there are should be 2*3*2 = 12 cells
            var quota3 = TestQuota.Create(TestingFramework.DbEngine,
                surveySid,
                3,
                columnsForQuota3,
                answerCountsForQuota3);

            //there are should be 2*3*2 = 12 cells
            TestQuota.Create(TestingFramework.DbEngine,
                             surveySid,
                             4,
                             columnsForQuota4,
                             answerCountsForQuota4);

            const int openCellIdForQuota1 = 2;
            const int closeCellIdForQuota1 = 4;
            const int openCellIdForQuota2 = 3;
            const int closeCellIdForQuota2 = 5;
            const int closeCellIdForQuota3 = 6;
            const int closeCellIdForQuota4 = 8;
            const int closeCellIdForQuota4_1 = 9;
            const int closeCellIdForQuota4_2 = 10;
            const int closeCellIdForQuota4_3 = 11;
            const int closeCellIdForQuota4_4 = 12;

            quota1.PutInterviewsInCells(
                new[] { interviews[0].ID },
                new[] { openCellIdForQuota1 });

            quota2.PutInterviewsInCells(
                new[] { interviews[0].ID },
                new[] { openCellIdForQuota2 });

            quota1.CloseCell(closeCellIdForQuota1);
            quota2.CloseCell(closeCellIdForQuota2);
            quota3.CloseCell(closeCellIdForQuota3);
            //May be it is wrong( I mean that we use quota3, but close cells for quota 4), but it was did before refactoring
            quota3.CloseCell(closeCellIdForQuota4);
            quota3.CloseCell(closeCellIdForQuota4_1);
            quota3.CloseCell(closeCellIdForQuota4_2);
            quota3.CloseCell(closeCellIdForQuota4_3);
            quota3.CloseCell(closeCellIdForQuota4_4);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            var call = new BvCallEntity
            {
                SurveySID = surveySid,
                InterviewID = interviews[0].ID
            };

            CallManager.AddCall(call);

            Assert.IsTrue(BackendTools.IsCallExists(surveySid, interviews[0].ID),
                "Calls should be created");
        }

        private void AddCall_TestBase(Action.Operation operation, bool isOpen, CallState result)
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=isOpen ? 0 : 1, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1"}
                        }
                    }
                },
                Scripts = new[] { new ScriptData() { Tag="SS1", Script = 
                    new TestScript(new Action(operation), 
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00") )} }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            ServiceLocator.Resolve<IInterviewRepository>().Update(interview.Model,
                new SchedulingScriptExecutionOptions());

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)result);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddEnabledCall_CallIsOpenedByFcd_CallIsCreatedAndEnabled()
        {
            AddCall_TestBase(Action.Operation.EnableCall, true, CallState.Scheduled);
        }
        
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddEnabledCall_CallIsClosedByFcd_CallIsCreatedAndDisabledByFCD()
        {
            AddCall_TestBase(Action.Operation.EnableCall, false, CallState.DisabledByFCD);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddDisabledCall_CallIsOpenedByFcd_CallIsCreatedAndDisabledByFCD()
        {
            AddCall_TestBase(Action.Operation.DisableCall, true, CallState.DisabledByUser);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddDisabledCall_CallIsClosedByFcd_CallIsCreatedAndDisabledByFCD()
        {
            AddCall_TestBase(Action.Operation.DisableCall, false, CallState.DisabledByUser);
        }
    }
}
