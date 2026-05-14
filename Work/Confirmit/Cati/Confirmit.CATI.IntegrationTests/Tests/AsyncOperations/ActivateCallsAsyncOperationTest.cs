using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{

    [TestClass]
    public class ActivateCallsAsyncOperationTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        const string surveyName = "p000001";
        const string personName = "TestPerson";


        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ActivateCalls_ActivateSelectedCall_CallIsActivated()
        {
            BvInterviewEntity interview;
            int personSID;
            var surveySID = InitActivateTest(out interview, out personSID);

            var operation = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySID, new[] { interview.ID }, 1, personSID, (int)CallShiftType.None, DateTime.UtcNow, CallStates.All, false, "super");

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);
            Assert.IsNotNull(operationEntity, "BvAsyncOperations records wasn't created");

            BackendTools.LoginPerson(personSID, string.Empty);
            var task = TaskService.LookupByPersonSid(personSID, surveySID);

            Assert.IsNotNull(task, "There is no task for user");
            Assert.AreEqual(interview.ID, task.InterviewID);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ActivateCalls_Activate3SelectedCalls_OperationPartiallyCompleted()
        {

            // Create survey
            int surveySID = BackendToolsObject.CreateSurvey(surveyName);

            // Open survey
            _surveyStateService.Open(surveySID);

            FillSchedulingScript();

            // Create person
            int personSID = PersonTools.CreatePerson(personName);

            const int portionSize = 1;

            var interview1 = BackendTools.NewInterview(surveySID);
            interview1.TimezoneID = 9;
            BackendTools.CreateInterview(interview1);

            var interview2 = BackendTools.NewInterview(surveySID);
            interview2.TimezoneID = 0;
            BackendTools.CreateInterview(interview2);

            var interview3 = BackendTools.NewInterview(surveySID);
            interview3.TimezoneID = 9;
            BackendTools.CreateInterview(interview3);

            var activationTime = new DateTime(2011, 10, 30, 14, 0, 0);
            var operation = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySID, new[] { interview1.ID, interview2.ID, interview3.ID }, 1, personSID, 2, activationTime, CallStates.All, false, portionSize);

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);
            Assert.AreEqual((int)AsyncOperationState.PartiallyCompleted, operationEntity.State, "Operation is not partially completed.");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ActivateCalls_Activate3SelectedCalls_OperationFailed()
        {

            // Create survey
            int surveySID = BackendToolsObject.CreateSurvey(surveyName);

            // Open survey
            _surveyStateService.Open(surveySID);

            FillSchedulingScript();

            // Create person
            int personSID = PersonTools.CreatePerson(personName);

            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.ActivatePortionSize = 1;

            var interview1 = BackendTools.NewInterview(surveySID);
            interview1.TimezoneID = 0;
            BackendTools.CreateInterview(interview1);

            var interview2 = BackendTools.NewInterview(surveySID);
            interview2.TimezoneID = 0;
            BackendTools.CreateInterview(interview2);

            var interview3 = BackendTools.NewInterview(surveySID);
            interview3.TimezoneID = 0;
            BackendTools.CreateInterview(interview3);

            var activationTime = new DateTime(2011, 10, 30, 14, 0, 0);
            var operation = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySID, new[] { interview1.ID, interview2.ID, interview3.ID }, 1, personSID, 2, activationTime, CallStates.All, false, "super");

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);
            Assert.AreEqual((int)AsyncOperationState.Failed, operationEntity.State, "Operation is not failed.");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ActivateCalls_ActivateFilteredCalls_OperationCompleted()
        {
            // Create survey
            int surveySID = BackendToolsObject.CreateSurvey(surveyName);

            // Open survey
            _surveyStateService.Open(surveySID);

            // Create person
            int personSID = PersonTools.CreatePerson(personName);

            var interview1 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview1);

            var interview2 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview2);

            var interview3 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview3);

            var operation = new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                    surveySID,
                    0,
                    1,
                    new SearchParameterCollection {
                        new SearchParameter
                            {
                                ColumnName = "InterviewID",
                                ColumnType = SearchColumnType.Number,
                                Operator = SearchOperator.Greater,
                                Value = interview1.ID
                            }
                    },
                    1,
                    CallStates.All,
                    personSID,
                    (int)CallShiftType.None,
                    DateTime.UtcNow,
                    false);
            var operationEntity = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);
            Assert.AreEqual((int)AsyncOperationState.Completed, operationEntity.State, "Operation is not completed.");

            BackendTools.LoginPerson(personSID, string.Empty);

            var task = TaskService.LookupByPersonSid(personSID, surveySID);
            Assert.IsNotNull(task, "There is no task for user");
            Assert.AreEqual(interview2.ID, task.InterviewID);

            task = TaskService.LookupByPersonSid(personSID, surveySID);
            Assert.IsNotNull(task, "There is no task for user");
            Assert.AreEqual(interview3.ID, task.InterviewID);

            task = TaskService.LookupByPersonSid(personSID, surveySID);
            Assert.IsNull(task, "Only interview 2 and 3 should be activated");
        }

        [TestMethod]
        public void ActivateCalls_ActivateOnNotExistingMultipleAssginment_ActivationSuccessed()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1"}}
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"},
                    new PersonGroupData {Tag = "PG3", Name = "PersonGroup3"}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            var group1 = context.GetResource("PG1");
            var group2 = context.GetResource("PG2");

            CallTools.ActivateCalls(survey.Id, 1, CallStates.All, new[] { group1.Id, group2.Id },
                (int)CallShiftType.None, null, false, new[] { interview.Id });

            var assignment = BvAssignmentResourceAdapter.GetAll().FirstOrDefault(a => a.Name == "PersonGroup1,PersonGroup2" && a.Qualifier == group1.Id + "," + group2.Id);
            Assert.IsNotNull(assignment, "Multuple assginment is not created.");

            context.GetCall("S1.I1").Assert.IsTrue(c => c.Resource == assignment.ID);
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void ActivateCalls_ActivatePortionSizeLessThanAmountOfCalls_ProcessedItemsCountIsCorrect()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[] { new InterviewData(5){ Tag = "S1.I1" } }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            systemSettings.AsyncOperation.ActivatePortionSize = 1;

            var operation = CallManager.ActivateCalls(survey.Id, 1, CallStates.All, new int[] { }, (int)CallShiftType.None,
                null, null, false, new SelectedBatchParameters(context.GetInterviews("S1.I1").Select(x => x.Id)));

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);
            operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);

            // assert
            Assert.AreEqual(5, operation.ProcessedItemsCount);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void ActivateCalls_ActivateAllCalls_2RecordsShouldBeInsertedIntoBvCallHistory()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[] { new InterviewData(1){ Tag = "S1.I1" }, new InterviewData(1) { Tag = "S1.I2" } }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();

            var operation = CallManager.ActivateCalls(survey.Id, 100, CallStates.All, new int[] { }, (int)CallShiftType.None, null,
                null, false, new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2").Select(x => x.Id)));

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);
            operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);

            // assert
            Assert.AreEqual(2, operation.ProcessedItemsCount);


            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId", new SqlParameter("@SurveyId", survey.Id));

            var first = history.First();
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual(operation.Id, first.OperationId);
            Assert.AreEqual((int)OperationType.ActivateCalls, (int)first.OperationType);
            Assert.AreEqual(1, first.CallCenterId);
        }


        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void ActivateCalls_ActivateAllAndScheduledCalls_3RecordsShouldBeInsertedIntoBvCallHistory()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[] { new InterviewData(1){ Tag = "S1.I1" }, new InterviewData(1) { Tag = "S1.I2" } }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var systemSettings = ServiceLocator.Resolve<ISystemSettings>();

            var operation1 = CallManager.ActivateCalls(survey.Id, 100, CallStates.All, new int[] { }, (int)CallShiftType.None,
                null, null, false, new SelectedBatchParameters(context.GetInterviews("S1.I1").Select(x => x.Id)));

            var operation2 = CallManager.ActivateCalls(survey.Id, 100, CallStates.All, new int[] { }, (int)CallShiftType.None,
                null, null, false, new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2").Select(x => x.Id)));


            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation1);
            operation1 = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation1.Id);

            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation2);
            operation2 = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation2.Id);

            // assert
            Assert.AreEqual(1, operation1.ProcessedItemsCount);
            Assert.AreEqual(2, operation2.ProcessedItemsCount);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId", new SqlParameter("@SurveyId", survey.Id));

            Assert.AreEqual(3, history.Count);

            Assert.AreEqual(1, history.Where(x => x.OperationId == operation1.Id).Count());
            Assert.AreEqual(2, history.Where(x => x.OperationId == operation2.Id).Count());
        }



        [TestMethod]
        public void ActivateCalls_ActivateOnExistingMultipleAssginment_ActivationSuccessed()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1"}}
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"},
                    new PersonGroupData {Tag = "PG3", Name = "PersonGroup3"}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            var group1 = context.GetResource("PG1");
            var group2 = context.GetResource("PG2");

            var assignmentId = ServiceLocator.Resolve<IAssignmentService>().GetAssignmentResourceId(new[] { group1.Id, group2.Id });
            Assert.AreNotEqual(0, assignmentId);

            CallTools.ActivateCalls(survey.Id, 1, CallStates.All, new[] { group1.Id, group2.Id },
                (int)CallShiftType.None, null, false, new[] { interview.Id });

            context.GetCall("S1.I1").Assert.IsTrue(c => c.Resource == assignmentId);
        }

        [TestMethod]
        public void ActivateCalls_ActivateWithSampleType_ActivateSuccessed()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData { Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", DialType = DialType.Cellphone},
                            new InterviewData {Tag = "S1.I2", DialType = DialType.Landline},
                            new InterviewData {Tag = "S1.I3", DialType = DialType.Assisted}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interviewIds = context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id).ToArray();

            CallTools.ActivateCalls(survey.Id, 1, CallStates.All, 0, (int)CallShiftType.None, null, false, interviewIds);

            context.GetCall("S1.I1").Assert.IsTrue(x => (DialType)x.DialTypeId == DialType.Cellphone);
            context.GetCall("S1.I2").Assert.IsTrue(x => (DialType)x.DialTypeId == DialType.Landline);
            context.GetCall("S1.I3").Assert.IsTrue(x => (DialType)x.DialTypeId == DialType.Assisted);
        }

        private int InitActivateTest(out BvInterviewEntity interview, out int personSID)
        {
            // Create survey
            int surveySID = BackendToolsObject.CreateSurvey(surveyName);

            // Open survey
            _surveyStateService.Open(surveySID);

            // Create person
            personSID = PersonTools.CreatePerson(personName);

            // Create interview            
            interview = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview);

            // Create call
            BvCallEntity call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);
            return surveySID;
        }

        private void FillSchedulingScript()
        {
            const int timezoneID = 9;
            TimezoneManager.AddTimezone(timezoneID);

            int scriptId = BackendTools.GetDefaultScheduleID();
            string path = Path.Combine(TestingFramework.Cfg.TestDataPath, @"AsyncOperations\Schedule.xml");
            string scriptContent = File.ReadAllText(path);

            var schedule = ScheduleRepository.GetById(scriptId);
            schedule.XmlUnderDev = scriptContent;
            ScheduleRepository.Update(schedule);
            ScheduleService.Launch(scriptId);
        }

        [TestMethod]
        public void ActivateCalls_WithEnablingOfDisabledCalls_CallStateAreUpdated()
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

            var survey = context.GetSurvey("S1");

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.Id,
                context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Select(x => x.Id).ToArray(),
                1, 0, (int)CallShiftType.None, CallStates.All, true);

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I4", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void ActivateCalls_WithoutEnablingOfDisabledCalls_CallStateAreUpdated()
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

            var survey = context.GetSurvey("S1");

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.Id,
                context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Select(x => x.Id).ToArray(),
                1, 0, (int)CallShiftType.None, CallStates.All, false);

            context.GetCalls("S1.I1", "S1.I3", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I2", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByUser);
            context.GetCalls("S1.I5").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void ActivateCalls_ActivateUnknownFCDCall_CallStateDisabledByFCD()
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
                            new InterviewData() {Tag = "S1.I1", Data = "q1=3", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=3", Call = new CallData(){CallState = (int)CallState.DisabledByUser}},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByUser}},
                            new InterviewData() {Tag = "S1.I5", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I6", Data = "q1=", Call = new CallData(){CallState = (int)CallState.DisabledByUser}}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.Id,
                context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").Select(x => x.Id).ToArray(),
                1, 0, (int)CallShiftType.None, CallStates.All, true);

            context.GetCalls("S1.I3", "S1.I4").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I1", "S1.I2", "S1.I5", "S1.I6").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void ActivateCalls_ActivateCallWithParameters_CheckIfAllParametersChangedCorrectly()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[] {
                            new InterviewData(1)
                            {
                                Tag = "S1.I1",
                            }
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interview = context.GetInterview("S1.I1");

            var operation = CallManager.ActivateCalls(survey.Id, 77, CallStates.All, new int[] { }, (int)CallShiftType.None, 13,
                new DateTime(2019, 1, 1), false, new SelectedBatchParameters(new[] { interview.Id }));

            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);

            interview.Assert.AreEqual(13, x => x.TransientState);

            var call = context.GetCall("S1.I1").Model;

            Assert.AreEqual(77, call.Priority);
            Assert.AreEqual(new DateTime(2019, 1, 1), call.TimeInShift);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void ActivateCalls_ActivateCallsWithDifferentCallStates_CheckIfTransientStateChangedCorrectly()
        {
            var initialTimeInShift = new DateTime(2018, 1, 1);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.InterviewInProgress, Priority = 1, TimeInShift = initialTimeInShift}},
                            new InterviewData() {Tag = "S1.I2", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.LoadedToDialerPredictively, Priority = 1, TimeInShift = initialTimeInShift}},
                            new InterviewData() {Tag = "S1.I3", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.ToBeAddedFromSample, Priority = 1, TimeInShift = initialTimeInShift}},
                            new InterviewData() {Tag = "S1.I4", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.ToBeDeleted, Priority = 1, TimeInShift = initialTimeInShift}},
                            new InterviewData() {Tag = "S1.I5", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.Scheduled, Priority = 1, TimeInShift = initialTimeInShift}},
                            new InterviewData() {Tag = "S1.I6", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.DisabledByFCD, Priority = 1, TimeInShift = initialTimeInShift}},
                            new InterviewData() {Tag = "S1.I7", ITS = CallOutcome.FreshSample, Call = new CallData(){CallState = (int)CallState.DisabledByUser, Priority = 1, TimeInShift = initialTimeInShift}}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviewIds = context.Interviews.Select(x => x.Id).ToList();

            //init 'response_control' table in survey db
            interviewIds.ForEach(x =>
                ExecuteSurveyQuery(survey.Id, $@"
                    MERGE response_control as target
                    USING (VALUES (0, {x})) AS source (ITS, respid)
                    ON (target.respid = source.respid)
                    WHEN MATCHED THEN
                        UPDATE SET target.ITS = 0
                    WHEN NOT MATCHED THEN
                        INSERT (ITS, respid)  
                        VALUES (0, {x})  
                    ;"));

            var operation = CallManager.ActivateCalls(survey.Id, 77, CallStates.All, new int[] { }, (int)CallShiftType.None, 13,
                new DateTime(2019, 1, 1), false, new SelectedBatchParameters(interviewIds));

            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);

            var interviewsWithNegativeCallStateAfterActivate = context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4");
            interviewsWithNegativeCallStateAfterActivate.Assert.IsTrue(x => x.TransientState == 16);
            var callsWithNegativeCallStatesAfterActivate = context.GetCalls("S1.I1", "S1.I2", "S1.I3");
            callsWithNegativeCallStatesAfterActivate.Assert.IsTrue(x =>
                x.Priority == 1 && x.TimeInShift == initialTimeInShift);

            context.GetCall("S1.I4").Assert.IsNull();

            var interviewsWithPositiveCallStateAfterActivate = context.GetInterviews("S1.I5", "S1.I6", "S1.I7");
            interviewsWithPositiveCallStateAfterActivate.Assert.IsTrue(x => x.TransientState == 13);
            var callsWithPositiveCallStatesAfterActivate = context.GetCalls("S1.I5", "S1.I6", "S1.I7");
            callsWithPositiveCallStatesAfterActivate.Assert.IsTrue(x =>
                x.Priority == 77 && x.TimeInShift == new DateTime(2019, 1, 1));

            //check negative calls ITS wasn't updated in 'response_control' table in survey db
            interviewsWithNegativeCallStateAfterActivate.Assert.AreEqual(0,
                x => ExecuteSurveyQuery(survey.Id, $@"SELECT its FROM response_control WHERE respid = {x.ID}"));

            //check positive calls ITS was updated in 'response_control' table in survey db
            interviewsWithPositiveCallStateAfterActivate.Assert.AreEqual(13,
                x => ExecuteSurveyQuery(survey.Id, $@"SELECT its FROM response_control WHERE respid = {x.ID}"));
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void ActivateCalls_ActivateSelectedCall_OperationStatusCompleted()
        {
            var surveySID = InitActivateTest(out var interview, out var personSID);

            // Activate call
            var operation = new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveySID, new[] { interview.ID }, 1, personSID, (int)CallShiftType.None, DateTime.UtcNow, CallStates.All, false, "super");

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);
            Assert.IsNotNull(operationEntity, "BvAsyncOperations records wasn't created");
            Assert.AreEqual((int)AsyncOperationState.Completed, operationEntity.State);
        }

        private object ExecuteSurveyQuery(int surveyId, string query)
        {
            var remoteConnectionString = ServiceLocator.Resolve<ISurveyConnectionStringProvider>()
                .GetConnectionInfo(surveyId).ConnectionString;

            using (var remoteConnectionProvider = new RemoteConnectionProvider(remoteConnectionString))
            {
                using (var command = new SqlCommand(query, remoteConnectionProvider.Connection))
                {
                    command.CommandType = CommandType.Text;
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
