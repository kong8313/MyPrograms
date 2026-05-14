using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation.Fakes;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.ControllerExtensions;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = System.Action;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.IntegrationTests.Tests.FCDSpecificTests
{
    [TestClass]
    public class FCDWithDisableCallsTests : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void CloseCell_SeveralCallsHintToClosedCell_CallsAreDisabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, null);
            console.Login();

            var startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);

            quota.CloseCellById(1);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AddSample_FullSchedulingMode_TwoInterviewAreDisabledByDifferentOperations()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData()
                {
                    Tag = "S1", IsUseDb = true, IsSupportBlackList = true, SchedulingScript = "SS1",
                    Forms = new[] {
                        new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                    },
                    Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=0},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        }
                }},
                Scripts = new[] { new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Framework.Tools.Action(Framework.Tools.Action.Operation.DisableCall, ""),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    } }
            }.Create();

            ServiceLocator.Resolve<IFCDSettings>().BehaviorType = (int)FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", TelephoneNumber = "88001001010", Data="q1=1"},
                new InterviewData() {Tag = "S1.I2", TelephoneNumber = "88001001011", Data="q1=2"}
            };

            survey.AddSample(SchedulingMode.Full, interviews);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FreshSample);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FreshSample);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByUser);

            Assert.AreEqual((byte)OperationType.DisableByFcdDuringSample, context.GetInterview("S1.I1").GetCallHistory().Last().OperationType);
            Assert.AreEqual(0, context.GetInterview("S1.I2").GetCallHistory().Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AddSample_FullSchedulingMode_FirstInterviewIsDisabledByDisabledCell_OnlyFirstRecordIsDisabledByFCD()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData()
                {
                    Tag = "S1", IsUseDb = true, IsSupportBlackList = true, SchedulingScript = "SS1",
                    Forms = new[] {
                        new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                    },
                    Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=1, Limit=0, IsDisabled=true},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        }
                }},
                Scripts = new[] { new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Framework.Tools.Action(Framework.Tools.Action.Operation.DisableCall, ""),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                    } }
            }.Create();

            ServiceLocator.Resolve<IFCDSettings>().BehaviorType = (int)FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", TelephoneNumber = "88001001010", Data="q1=1"},
                new InterviewData() {Tag = "S1.I2", TelephoneNumber = "88001001011", Data="q1=2"}
            };

            survey.AddSample(SchedulingMode.Full, interviews);

            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FreshSample);
            context.GetInterview("S1.I2").Assert.IsTrue(x => x.TransientState == (int)CallOutcome.FreshSample);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByUser);

            Assert.AreEqual((byte)OperationType.DisableByFcdDuringSample, context.GetInterview("S1.I1").GetCallHistory().Last().OperationType);
            Assert.AreEqual(0, context.GetInterview("S1.I2").GetCallHistory().Count);
        }

        [TestMethod, WorkItem(1291)]
        public void CloseCell_SeveralCallsHintToClosedCell_LockSetTwice_TimeoutExceptionHandled_CallsAreDisabled()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData()
                            {
                                Id = 1,
                                Name = "quota1",
                                Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData() {Id = 1, Values = "q1=1", Counter = 0, Limit = 1},
                                    new CellData() {Id = 2, Values = "q1=2", Counter = 0, Limit = 1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=1", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Call = new CallData()},
                        },
                        Assigns = new[] {"P1"}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            TestingFramework.RegistryStub<IDatabaseLockTimeouts, StubIDatabaseLockTimeouts>().DefaultLockTimeoutInMsGet = () => 0;

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, null);
            console.Login();

            var startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);

            ManualResetEvent completeEvent = new ManualResetEvent(false);

            // locking the database
            new TaskFactory().StartNew(
             () =>
             {
                 using (
                     var dbLock =
                         ExclusiveDatabaseLock.CreateLock(
                             DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(survey.Id),
                             "OnQuotaCellChanged", 1000))
                 {
                     dbLock.TryEnterLock();

                     completeEvent.WaitOne(-1);
                 }
             });

            try
            {
                quota.CloseCellById(1);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("IsLockHeld"))
                {
                    Assert.Fail("Exception mesage does not containg IsLockHeld", ex.Message);
                }
            }

            completeEvent.Set(); // unlocking database

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress); // in given context mean close cell work for second call from thread
            context.GetCalls("S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled); // this one should not be affected
        }

        [TestMethod]
        public void OpenCell_SeveralCallsHintToReopeningCell_CallsAreEnabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, null);
            console.Login();

            var startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);

            quota.CloseCellById(1);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCall("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);

            quota.OpenCellById(1);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void OpenCell_SeveralCallsHintToClosedCellByOtherQuota_CallsAreEnabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            },
                            new QuotaData(){ Id = 2, Name="quota2", Fields = new[] {"q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q2=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q2=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Data="q1=2,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");
            var quota1 = survey.GetQuota("quota1");
            var quota2 = survey.GetQuota("quota2");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, null);
            console.Login();

            var startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);

            quota1.CloseCellById(1);
            quota2.CloseCellById(1);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCall("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCall("S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I5").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);

            quota1.OpenCellById(1);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCall("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I5").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void UpdateQuota_SeveralCallsHintToClosedCellByOtherQuota_CallsAreEnabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            },
                            new QuotaData(){ Id = 2, Name="quota2", Fields = new[] {"q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q2=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q2=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Data="q1=2,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");
            var quota1 = survey.GetQuota("quota1");
            var quota2 = survey.GetQuota("quota2");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, null);
            console.Login();

            var startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);

            quota1.CloseCellById(1);
            quota2.CloseCellById(1);
            survey.Database.OpenCell(1, 1);

            quota1.OnQuotaChanged();

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCall("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I5").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void CloseCell_SeveralClosedCallsAreDeliveredToDialer_FlushNumberAreSent()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, DialMode = ConfirmitDialerInterface.DialingMode.Predictive, IsOpen = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=1", Call = new CallData(){Priority = 10}},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1,q2=2", Call = new CallData(){Priority = 10}},
                            new InterviewData(){ Tag="S1.I4", Data="q1=2,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I6", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");
            var dialer = context.GetDialer("D1");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            dialer.RequestCalls(survey, 2);

            context.GetCalls("S1.I1", "S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);

            quota.CloseCellById(1);

            CollectionAssert.AreEqual(
                dialer.FlushedCalls.Select(x => x.interviewId).ToArray(),
                context.GetInterviews("S1.I2", "S1.I3").Select(x => x.Id).ToArray());

            context.GetCalls("S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void QuotaUpdate_SeveralClosedCallsAreDeliveredToDialer_FlushNumberAreSent()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, DialMode = ConfirmitDialerInterface.DialingMode.Predictive, IsOpen = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=1", Call = new CallData(){Priority = 10}},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1,q2=2", Call = new CallData(){Priority = 10}},
                            new InterviewData(){ Tag="S1.I4", Data="q1=2,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I6", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");
            var dialer = context.GetDialer("D1");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            dialer.RequestCalls(survey, 2);

            context.GetCalls("S1.I1", "S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);

            survey.Database.CloseCell(quota.Data.Id, 1);
            quota.OnQuotaChanged();

            CollectionAssert.AreEqual(
                dialer.FlushedCalls.Select(x => x.interviewId).ToArray(),
                context.GetInterviews("S1.I2", "S1.I3").Select(x => x.Id).ToArray());


            context.GetCalls("S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void LaunchSurvey_SeveralClosedCallsAreDeliveredToDialer_FlushNumberAreSent()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true, DialMode = ConfirmitDialerInterface.DialingMode.Predictive, IsOpen = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=1", Call = new CallData(){Priority = 10}},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1,q2=2", Call = new CallData(){Priority = 10}},
                            new InterviewData(){ Tag="S1.I4", Data="q1=2,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Data="q1=2,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I6", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            dialer.RequestCalls(survey, 2);

            context.GetCalls("S1.I1", "S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);

            survey.Data.Quotas = new[] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        };
            survey.Launch();

            CollectionAssert.AreEqual(
                dialer.FlushedCalls.Select(x => x.interviewId).ToArray(),
                context.GetInterviews("S1.I2", "S1.I3").Select(x => x.Id).ToArray());


            context.GetCalls("S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void EnableCall_OneCallIsClosedByFCD_FilteredCallIsNotEnabled()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=1, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interviewIds = context.GetInterviews("S1.I1", "S1.I2").Select(x => x.Id);
            CallTools.EnableCalls(survey.Id, true, interviewIds);

            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCall("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        private TestDataContext PrepareCallsWithDifferentStates()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByUser}},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByUser}},
                            new InterviewData() {Tag = "S1.I5", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.Scheduled}},
                            new InterviewData() {Tag = "S1.I6", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}}
                        }
                    }
                }
            }.Create();

            return context;
        }

        [TestMethod]
        public void EnableCallByUser_CallWithDifferentStates_ResultIsCorrect()
        {
            var context = PrepareCallsWithDifferentStates();

            var survey = context.GetSurvey("S1");
            var interviewIds = context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Select(x => x.Id).ToArray();

            new TestCallManagementOperationFactory().CreateEnableCallsSelected(survey.Id, interviewIds, true);

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void DisableCallByUser_CallWithDifferentStates_ResultIsCorrect()
        {
            var context = PrepareCallsWithDifferentStates();

            var survey = context.GetSurvey("S1");
            var interviewIds = context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Select(x => x.Id).ToArray();

            new TestCallManagementOperationFactory().CreateEnableCallsSelected(survey.Id, interviewIds, false);

            context.GetCalls("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByUser);
        }
        [TestMethod]
        public void EnableCallByFcd_CallWithDifferentStates_ResultIsCorrect()
        {
            var context = PrepareCallsWithDifferentStates();
            var survey = context.GetSurvey("S1");

            var quota = survey.GetQuota("quota");
            quota.OpenCellById(1);

            context.GetCalls("S1.I1", "S1.I3", "S1.I5").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            context.GetCalls("S1.I2", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByUser);
            context.GetCalls("S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void DisabledCallByFcd_CallWithDifferentStates_ResultIsCorrect()
        {
            var context = PrepareCallsWithDifferentStates();
            var survey = context.GetSurvey("S1");

            var quota = survey.GetQuota("quota");
            quota.CloseCellById(2);

            context.GetCalls("S1.I1", "S1.I3", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I2", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByUser);
        }

        [TestMethod]
        public void OnQuotaCellsChanged_CountOfOpenedCellsDoesnotExeedMaxQuestionsPerQuotaSetting_EachCellsAreOpenedInSeparatedAsyncOperations()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;
            ServiceLocator.Resolve<ISystemSettings>().Quotas.MaxQuestionsPerQuota = 2;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=1, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");
            var operationsCount = GetUpdateFcdQuotaOperationsCount();


            quota.OpenCellsById(1, 2);

            Assert.AreEqual(operationsCount + 2, GetUpdateFcdQuotaOperationsCount());
            context.GetCalls("S1.I1", "S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void OnQuotaCellsChanged_CountOfOpenedCellsExeedsMaxQuestionsPerQuotaSetting_AllCellsAreOpenedInSingleAsyncOperation()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;
            ServiceLocator.Resolve<ISystemSettings>().Quotas.MaxQuestionsPerQuota = 1;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=1, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");
            var operationsCount = GetUpdateFcdQuotaOperationsCount();

            quota.OpenCellsById(1, 2);

            Assert.AreEqual(operationsCount + 1, GetUpdateFcdQuotaOperationsCount());
            context.GetCalls("S1.I1", "S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        private static int GetUpdateFcdQuotaOperationsCount()
        {
            return BvAsyncOperationQueueAdapter.GetByCondition("Type=@Type", new SqlParameter("@Type", (int)OperationTypes.UpdateFcdQuota)).Count;
        }

        [TestMethod]
        public void LaunchSurvey_QuotaHasBeenRemoved_CallsAreEnabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            },
                            new QuotaData(){ Id = 2, Name="quota2", Fields = new[] {"q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q2=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q2=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Data="q1=2,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I5", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");
            var quota1 = survey.GetQuota("quota1");
            var quota2 = survey.GetQuota("quota2");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, null);
            console.Login();

            var startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);

            quota1.CloseCellById(1);
            quota2.CloseCellById(1);

            context.GetCall("S1.I1").Assert.CallState(CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.CallState(CallState.DisabledByFCD);
            context.GetCall("S1.I3").Assert.CallState(CallState.DisabledByFCD);
            context.GetCall("S1.I4").Assert.CallState(CallState.Scheduled);
            context.GetCall("S1.I5").Assert.CallState(CallState.Scheduled);

            // Delete 'quota1', keep only 'quota2'
            survey.Data.Quotas = new[]
            {
                new QuotaData
                {
                    Id = 2, Name = "quota2", Fields = new[] {"q2"},
                    Cells = new[]
                    {
                        new CellData() {Id = 1, Values = "q2=1", Counter = 1, Limit = 1},
                        new CellData() {Id = 2, Values = "q2=2", Counter = 0, Limit = 1},
                    }
                }
            };

            survey.Launch();

            context.GetCall("S1.I1").Assert.CallState(CallState.InterviewInProgress);
            context.GetCall("S1.I2").Assert.CallState(CallState.DisabledByFCD);
            context.GetCall("S1.I3").Assert.CallState(CallState.Scheduled);
            context.GetCall("S1.I4").Assert.CallState(CallState.Scheduled);
            context.GetCall("S1.I5").Assert.CallState(CallState.Scheduled);
        }
        
        [TestMethod]
        public void CloseCell_CallsMatchedToAnyCellsAreAlsoClosed()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1","q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=1,q2=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I4", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, null);
            console.Login();

            var startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);
            quota.CloseCellById(1);
            console.FinishInterview(startedInterview);
            startedInterview = console.StartInterview();
            Assert.IsNotNull(startedInterview);
            quota.CloseCellById(2);
            console.FinishInterview(startedInterview);

            context.GetCalls("S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);

        }
    }
}
