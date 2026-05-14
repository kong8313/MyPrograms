using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation.Fakes;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Common;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using System.Collections.Generic;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Rule = Confirmit.CATI.IntegrationTests.Framework.Tools.Rule;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.SampleTest
{
    [TestClass]
    public class SampleTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        /// <summary>
        /// Copy pasted in 2 files. backendTools.cs and SampleTest.cs
        /// </summary>
        private const int AddSampleGetStateTimeout = 500;
        private const int BatchId = 2;
        const int RecordsCount = 4;

        private static string _projectId;
        private int _surveySid;
        private static DatabaseEngine _confirmitDb;
        private static IEnumerable<RespondentRecord> _addedRecords;
        private static readonly IEnumerable<int> TimeZones = Enumerable.Range(1, RecordsCount);
        private const int StartRespId = 1;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _confirmitDb = ConfirmitTools.GetConfirmitSurveyDbOnClass(out _projectId);
            _addedRecords = ConfirmitTools.FillRespondentTable(_confirmitDb,
               BatchId,
               StartRespId,
               RecordsCount,
               TimeZones);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            IntegrationTestingFramework.ClassCleanup();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveySid = _backendTools.CreateSurvey(_projectId, _confirmitDb.ConnectionString);

            ConfirmitTools.ClearCatiInterviewerIdColumnInRespondentTable(_confirmitDb);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        private BvInterviewEntity[] GetExpectedInterviewsForSimpleMode()
        {
            return _addedRecords.Select((x, i) =>
                new BvInterviewEntity
                {
                    BatchID = BatchId,
                    ConfirmitSid = (i + 1).ToString(CultureInfo.InvariantCulture),
                    DialingMode = 0,
                    Duration = i + 1,
                    ID = i + 1,
                    SurveySID = _surveySid,
                    TimezoneID = i + 1,
                    TransientState = 16,
                    RespondentName = "resp" + (i + 1),
                }).ToArray();
        }

        private BvCallEntity[] GetExpectedCallsForSimpleMode()
        {
            return _addedRecords.Select((x, i) =>
                new BvCallEntity
                {
                    InterviewID = i + 1,
                    CallID = i + 1,
                    SurveySID = _surveySid,
                    ShiftID = -(i + 1),
                }).ToArray();
        }

        private void SetCatiInterviewer(int respId, string catiInterviewrId)
        {
            _confirmitDb.ExecuteNonQuery(String.Format(
               "update respondent set CatiInterviewerID = {0} where respid = {1}",
               catiInterviewrId,
               respId),
               CommandType.Text);
        }

        private void CheckAggregateData()
        {
            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();
            BvSpAggregateSurveyProcessDeltaAdapter.ExecuteNonQuery();

            var actualAggregateData = BvAggregateSurveyAdapter.GetAll();

            Assert.AreEqual(1, actualAggregateData.Count, "In bvaggregatesurvey should be 1 record");
            Assert.AreEqual(RecordsCount, actualAggregateData[0].ScheduledCallsCount, "ScheduledCallsCount");
            Assert.AreEqual(RecordsCount, actualAggregateData[0].SuspendedCallsCount, "SuspendedCallsCount");
        }

        private void CheckBvInterviewTable(IEnumerable<BvInterviewEntity> expectedInterviews)
        {
            var actualInterviews = BvInterviewAdapter.GetAll();
            TestAssert.AreEqual(expectedInterviews, actualInterviews);
        }

        private void CheckCallsTable(IEnumerable<BvCallEntity> expectedCalls)
        {
            TestAssert.AreEqual(expectedCalls, _addedRecords.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x.InterviewId)));
        }

        private void CheckFinishedTimeOfSample(int batchId)
        {
            var sampleEntity = BvSamplesAdapter.GetByCondition(
                "BatchID = @BatchID",
                new SqlParameter("@BatchID", batchId));

            Assert.IsNotNull(
                sampleEntity.First().FinishedTime,
                "Finished time should not be NULL");
        }

        private void AddSchedulingScript(
            int surveySid,
            int newIts,
            int newPriority)
        {
            var script = new TestScript(
                new SubRule(new[]
                {
                    new Action(Action.Operation.SetNewITS, newIts.ToString(CultureInfo.InvariantCulture)),
                    new Action(Action.Operation.SetNewCallPriority, newPriority.ToString(CultureInfo.InvariantCulture))
                }),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                new Shift(2, 1, "1.00:00:00", "2.00:00:00"),
                new Shift(3, 1, "2.00:00:00", "3.00:00:00"),
                new Shift(4, 1, "3.00:00:00", "4.00:00:00"),
                new Shift(5, 1, "4.00:00:00", "5.00:00:00"),
                new Shift(6, 1, "5.00:00:00", "6.00:00:00"),
                new Shift(7, 1, "6.00:00:00", "0.00:00:00"));

            AddSchedulingScript(surveySid, script);
        }

        private static void AddSchedulingScript(int surveySid, TestScript script)
        {
            int scheduleId = script.Create(null);
            var survey = SurveyRepository.GetById(surveySid);

            survey.ScheduleID = scheduleId;
            SurveyRepository.Update(survey);
        }

        private ProcessSampleAsyncResult AddSampleAndWaitAsyncResult(
            int batchId,
            out string stateDescription)
        {
            var managementService = new ManagementService();

            //
            // call testing method
            managementService.AddSample(_projectId, batchId, (int)SchedulingMode.Simple, RecordsCount);

            BackendTools.ExecuteAllAsyncOperations();
            
            //
            // wait for sample async state
            int sampleState;
            int numberOfAttempts = 25;

            while ((sampleState = managementService.AddSampleGetState(
                batchId,
                out stateDescription)) == (int)ProcessSampleAsyncResult.InProgress)
            {
                if (--numberOfAttempts == 0)
                {
                    break;
                }

                System.Threading.Thread.Sleep(AddSampleGetStateTimeout);
            }

            return (ProcessSampleAsyncResult)sampleState;
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_SimpleScheduling_Success()
        {
            var surveyBeforeSampleAddition = SurveyRepository.GetById(_surveySid);
            surveyBeforeSampleAddition.LastTouchTime = DateTime.UtcNow.AddMinutes(-1);
            SurveyRepository.Update(surveyBeforeSampleAddition);

            _backendTools.AddSample(
                _projectId,
                BatchId,
                (int)SchedulingMode.Simple,
                StartRespId,
                RecordsCount,
                TimeZones);

            var surveyAfterSampleAddition = SurveyRepository.GetById(_surveySid);

            var expectedInterviews = GetExpectedInterviewsForSimpleMode();

            var expectedCalls = GetExpectedCallsForSimpleMode();

            CheckBvInterviewTable(expectedInterviews);
            CheckAggregateData();
            CheckCallsTable(expectedCalls);
            Assert.AreNotEqual(surveyBeforeSampleAddition.LastTouchTime, surveyAfterSampleAddition.LastTouchTime);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void AddSample_CfDbSchemaPathIsEmpty_Success()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true, IsQuotaInCatiDb = false
                    }
                }
            }.Create();

            var surveyEntity = context.GetSurvey("S1").Model;
            surveyEntity.CfDbSchemaPath = string.Empty;
            BvSurveyAdapter.Update(surveyEntity);

            var interviews = new[]
            {
                new InterviewData {Tag = "S1.I1"},
                new InterviewData {Tag = "S1.I2"},
            };

            var survey = context.GetSurvey("S1");
            survey.AddSample(SchedulingMode.Simple, interviews);

            context.GetCalls("S1.I1", "S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void ProcessSample_SimpleScheduling_Success()
        {
            var surveyBeforeSampleAddition = SurveyRepository.GetById(_surveySid);
            surveyBeforeSampleAddition.LastTouchTime = DateTime.UtcNow.AddMinutes(-1);
            SurveyRepository.Update(surveyBeforeSampleAddition);

            _backendTools.ProcessSample(
                _projectId,
                BatchId,
                (int)ProcessSampleMode.Add,
                (int)SchedulingMode.Simple,
                StartRespId,
                RecordsCount,
                TimeZones);

            var surveyAfterSampleAddition = SurveyRepository.GetById(_surveySid);

            var expectedInterviews = GetExpectedInterviewsForSimpleMode();

            var expectedCalls = GetExpectedCallsForSimpleMode();

            CheckBvInterviewTable(expectedInterviews);
            CheckAggregateData();
            CheckCallsTable(expectedCalls);
            Assert.AreNotEqual(surveyBeforeSampleAddition.LastTouchTime, surveyAfterSampleAddition.LastTouchTime);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSample_FullScheduling_Success()
        {
            const int newIts = 19;
            const short newPriority = 21;

            AddSchedulingScript(_surveySid, newIts, newPriority);

            _backendTools.AddSample(
                _projectId,
                BatchId,
                (int)SchedulingMode.Full,
                StartRespId,
                RecordsCount,
                TimeZones);

            CheckBvInterviewTable(GetExpectedInterviewsForSimpleMode().Select(x => { x.TransientState = newIts; return x; }));
            CheckAggregateData();
            CheckCallsTable(GetExpectedCallsForSimpleMode().Select(x => { x.Priority = newPriority; x.ShiftID = (int)CallShiftType.None; return x; }));
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void ProcessSample_FullScheduling_Success()
        {
            const int newIts = 19;
            const short newPriority = 21;

            AddSchedulingScript(_surveySid, newIts, newPriority);

            _backendTools.ProcessSample(
                _projectId,
                BatchId,
                (int)ProcessSampleMode.Add,
                (int)SchedulingMode.Full,
                StartRespId,
                RecordsCount,
                TimeZones);

            CheckBvInterviewTable(GetExpectedInterviewsForSimpleMode().Select(x => { x.TransientState = newIts; return x; }));
            CheckAggregateData();
            CheckCallsTable(GetExpectedCallsForSimpleMode().Select(x => { x.Priority = newPriority; x.ShiftID = (int)CallShiftType.None; return x; }));
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSample_FullSchedulingWithDisabledCalls_Success()
        {
            var script = new TestScript(new SubRule(new[]
                {
                    new Action(Action.Operation.DisableCall, "", "Scheduling.Interview.ID % 2 == 1"),
                    new Action(Action.Operation.SetNewCallPriority, "10")
                }),
                new Shift(1, 1, "0.00:00:00", "6.00:00:00"));

            AddSchedulingScript(_surveySid, script);

            _backendTools.AddSample(
                _projectId,
                BatchId,
                (int)SchedulingMode.Full, StartRespId, RecordsCount, TimeZones);

            CheckBvInterviewTable(GetExpectedInterviewsForSimpleMode());
            CheckCallsTable(GetExpectedCallsForSimpleMode().Select(x =>
            {
                x.Priority = 10;
                x.ShiftID = (int)CallShiftType.None;
                x.CallState = x.InterviewID % 2 == 1 ? (int)CallState.DisabledByUser : (int)CallState.Scheduled;
                return x;
            }));
        }

        /// <summary>
        /// Add survey
        /// Add sample with simple assignment mode with NULL CatiInterviewerID
        /// Check that sample was added successfully and Resource=0 for added calls
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_SimpleWithoutCATIInterviewerID_Success()
        {
            SetCatiInterviewer(1, "null");

            _backendTools.AddSample(
                _projectId,
                BatchId,
                (int)SchedulingMode.Simple, StartRespId, RecordsCount, TimeZones);

            CheckBvInterviewTable(GetExpectedInterviewsForSimpleMode());
            CheckAggregateData();
            CheckCallsTable(GetExpectedCallsForSimpleMode());
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_SimpleWithDialMode_Success()
        {
            SetCatiInterviewer(1, "null");

            _confirmitDb.ExecuteNonQuery("UPDATE respondent SET DialMode = 2", CommandType.Text);
            try
            {
                _backendTools.AddSample(
                _projectId,
                BatchId,
                (int)SchedulingMode.Simple);

                CheckBvInterviewTable(GetExpectedInterviewsForSimpleMode().Select(x => { x.DialingMode = 2; return x; }));
                //rollback changes

            }
            finally
            {
                _confirmitDb.ExecuteNonQuery("UPDATE respondent SET DialMode = 0", CommandType.Text);
            }

        }

        /// <summary>
        /// Add survey
        /// Create 2 persons and 1 group
        /// Add sample with simple assignment mode with NOT NULL CatiInterviewerID
        /// Check that sample was added successfully and 
        /// Resource=CatiInterviewerID if such person/group exists in CATI and
        /// Resource=0 if such person/group does not exist in CATI
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_SimpleWithCATIInterviewerID_Success()
        {
            var parentCatiGroup = PersonGroupRepository.GetById(PersonGroupService.RootGroupId);
            int person1SID = PersonTools.CreatePerson("Person1");
            int person2SID = PersonTools.CreatePerson("Person2");
            int group1SID = PersonTools.CreatePersonGroup("Group1", new[] { parentCatiGroup.SID });

            SetCatiInterviewer(1, "100000");
            SetCatiInterviewer(2, person1SID.ToString(CultureInfo.InvariantCulture));
            SetCatiInterviewer(3, person2SID.ToString(CultureInfo.InvariantCulture));
            SetCatiInterviewer(4, group1SID.ToString(CultureInfo.InvariantCulture));

            int[] expectedResource = { 0, person1SID, person2SID, group1SID };

            _backendTools.AddSample(
                _projectId,
                BatchId,
                (int)SchedulingMode.Simple);

            CheckBvInterviewTable(GetExpectedInterviewsForSimpleMode());
            CheckAggregateData();
            CheckCallsTable(GetExpectedCallsForSimpleMode().Select((x, i) => { x.Resource = expectedResource[i]; return x; }));
        }

        /// <summary>
        /// 1.	Add a survey 1 with sample with timezones.
        /// 2.	Verify that timezone 3 is in active timezones list.
        /// 3.	Set timezone 3 as default.
        /// 4.	Verify that default timezone is timezone 3.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_WithTimezone_VerifyTimezoneActivated()
        {
            _backendTools.AddSample(
               _projectId,
               BatchId,
               (int)SchedulingMode.Simple, StartRespId, RecordsCount, TimeZones);

            Assert.IsTrue(TimezoneManager.ActiveTimezonesList.Any(x => x.ID == 3),
                          "Timezone 3 is not in active timezones list after sample adding with timezone 3");
        }

        /// <summary>
        /// 1. add survey and assign scheduling script
        /// 2. call AddSample method
        /// 3. produce an error in AddSampleThreadProc method on insert interview
        /// 4. check that async state for BatchID is error
        /// 5. check that record in BvSamples has proper values in FinishedTime and StateDescription fields
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_ErrorInAddSampleThreadOnInterviewInsert_AsyncStateSet()
        {
            // exception should be thrown inside of working thread (AddSampleThreadProc)
            var stubIInterviewRepository = new StubIInterviewRepository
            {
                InsertBvInterviewWithOriginEntitySchedulingScriptExecutionOptionsISampleDataStorage =
                    (interview, options, storage) => { throw new Exception(); }
            };
            ServiceLocator.RegisterInstance<IInterviewRepository>(stubIInterviewRepository);

            //
            // call tested method and wait result
            string stateDescription;
            ProcessSampleAsyncResult sampleState = AddSampleAndWaitAsyncResult(
                BatchId,
                out stateDescription);

            //
            // check results
            Assert.AreEqual(ProcessSampleAsyncResult.Error, sampleState, "async state is not correct");

            Assert.IsTrue(stateDescription.StartsWith("Error! System.Exception"), "invalid state description in BvSamples table");

            CheckFinishedTimeOfSample(BatchId);
        }

        /// <summary>
        /// 1. add survey and assign scheduling script
        /// 2. call AddSample method
        /// 3. produce an error in AddSampleThreadProc method on sample commit
        /// 4. check that async state for BatchID is error
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_ErrorInAddSampleThreadOnSampleDataStorageCommit_BatchIsRetried()
        {
            // exception should be thrown inside of working thread (AddSampleThreadProc)
            int commitCount = 0;
            SampleDataStorage originalSampleDataStorage;
            var stubISampleDataStorageFactory = new StubISampleDataStorageFactory
            {
                CreateInt32BvSurveyEntityInt32Boolean = (batchId, survey, startRangeOfInterviewId, isUpdateMode) =>
                {
                    originalSampleDataStorage = new SampleDataStorage(
                        ServiceLocator.Resolve<ISurveyConnectionStringProvider>(),
                        ServiceLocator.Resolve<IRemoteDataCopier>(),
                        ServiceLocator.Resolve<ISurveyDatabaseEngine>(),
                        batchId,
                        survey.SID,
                        (SurveySchedulingMode)survey.SurveySchedulingMode,
                        survey.IsRandomCallDeliveryEnabled,
                        startRangeOfInterviewId,
                        isUpdateMode);

                    return new StubISampleDataStorage
                    {
                        Inner = originalSampleDataStorage,
                        CommitIEventDetails = details =>
                        {
                            if (commitCount++ == 0)
                                throw new Exception();

                            originalSampleDataStorage.Commit(details);
                        }
                    };
                }
            };
            ServiceLocator.RegisterInstance<ISampleDataStorageFactory>(stubISampleDataStorageFactory);

            //
            // call tested method and wait result
            string stateDescription;
            ProcessSampleAsyncResult sampleState = AddSampleAndWaitAsyncResult(
                BatchId,
                out stateDescription);

            Assert.AreEqual(2, commitCount);
            // check results
            Assert.AreEqual(ProcessSampleAsyncResult.Success, sampleState, "async state is not an correct");

            Assert.AreEqual(string.Empty, stateDescription, "invalid state description in BvSamples table");

            CheckFinishedTimeOfSample(BatchId);
        }

        /// <summary>
        /// 1. add survey and assign scheduling script
        /// 2. call AddSample method
        /// 3. produce an error in AddSampleThreadProc method on sample commit
        /// 4. check that async state for BatchID is error
        /// </summary>
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSample_ErrorAddBatchPartition_CorrectMessageAreReturned()
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.AddSamplePortionSize = 1;
            int retryCount = ServiceLocator.Resolve<IRetryingServiceSettings>().NumberOfRetryAttempts;

            int commitCount = 0;
            SampleDataStorage originalSampleDataStorage;
            var stubISampleDataStorageFactory = new StubISampleDataStorageFactory
            {
                CreateInt32BvSurveyEntityInt32Boolean = (batchId, survey, startRangeOfInterviewId, isUpdateMode) =>
                {
                    originalSampleDataStorage = new SampleDataStorage(
                        ServiceLocator.Resolve<ISurveyConnectionStringProvider>(),
                        ServiceLocator.Resolve<IRemoteDataCopier>(),
                        ServiceLocator.Resolve<ISurveyDatabaseEngine>(),
                        batchId,
                        survey.SID,
                        (SurveySchedulingMode)survey.SurveySchedulingMode,
                        survey.IsRandomCallDeliveryEnabled,
                        startRangeOfInterviewId,
                        isUpdateMode);

                    return new StubISampleDataStorage
                    {
                        Inner = originalSampleDataStorage,
                        CommitIEventDetails = details =>
                        {
                            if (commitCount++ < retryCount)
                                throw new Exception();

                            originalSampleDataStorage.Commit(details);
                        }
                    };
                }
            };
            ServiceLocator.RegisterInstance<ISampleDataStorageFactory>(stubISampleDataStorageFactory);

            //
            // call tested method and wait result
            string stateDescription;
            ProcessSampleAsyncResult sampleState = AddSampleAndWaitAsyncResult(
                BatchId,
                out stateDescription);

            Assert.AreEqual(RecordsCount + retryCount - 1, commitCount);
            // check results
            Assert.AreEqual(ProcessSampleAsyncResult.Success, sampleState, "async state is not an correct");

            Assert.AreEqual("Warning! Sample is partially added. Following interviews weren't added: [1-1]", stateDescription,
                "invalid state description in BvSamples table");

            CheckFinishedTimeOfSample(BatchId);
        }

        /// <summary>
        /// 1. add survey and assign scheduling script
        /// 2. call AddSample method
        /// 3. produce an error in AddSampleThreadProc method on sample commit
        /// 4. check that async state for BatchID is error
        /// </summary>
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSample_ErrorInAddSampleThreadOnSampleRequestBatch_BatchIsRetried()
        {
            var respondentDataObtainer = _framework.RegistryStub<IRespondentBatchObtainer, StubIRespondentBatchObtainer>();
            int callCount = 0;
            respondentDataObtainer.GetRespondentBatchPartitionBvSurveyEntityInt32Int32Int32Boolean = (survey, batchId, startRangeOfInterviewId, partitionSize, isUpdateMode) =>
            {
                if (callCount++ == 0)
                    throw new Exception();

                var surveyConnectionStringProvider = ServiceLocator.Resolve<ISurveyConnectionStringProvider>();
                var connectionStrings = ServiceLocator.Resolve<IConnectionStrings>();
                var companyInfo = ServiceLocator.Resolve<ICompanyInfo>();
                return new RespondentDataObtainer(surveyConnectionStringProvider, new RemoteDataCopier(), connectionStrings, companyInfo).GetRespondentBatchPartition(survey, batchId, startRangeOfInterviewId, partitionSize, false);
            };

            // call tested method and wait result
            string stateDescription;
            ProcessSampleAsyncResult sampleState = AddSampleAndWaitAsyncResult(
                BatchId,
                out stateDescription);

            Assert.AreEqual(2, callCount);

            //
            // check results
            Assert.AreEqual(ProcessSampleAsyncResult.Success, sampleState, "async state is not an correct");

            Assert.AreEqual(string.Empty, stateDescription, "invalid state description in BvSamples table");

            CheckFinishedTimeOfSample(BatchId);
        }

        /// <summary>
        /// 1. create scheduling script that will generate an error on execute
        /// 2. create survey and assign scheduling script (1)
        /// 3. add sample and check that operation is success
        /// 4. check that ITS of all added interviews is Error == 30
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddSample_ErrorInFullScheduling_OperationIsSuccess_InterviewsHaveITSAsExpected()
        {
            // create scheduling script
            new TestScript(
                    new Action(
                        Action.Operation.SetNewITS,
                        "15",
                        "throw new UserMessageException(\"test error\");return false;"),
                    @"Scheduling2007\Schedule.xml");

            _backendTools.AddSample(
                _projectId,
                BatchId,
                (int)SchedulingMode.Full, StartRespId, RecordsCount, TimeZones);

            var interviews = BvInterviewAdapter.GetByCondition(
                "TransientState = @TransientState",
                new SqlParameter("@TransientState", (int)CallOutcome.Error));

            Assert.AreEqual(interviews.Count, RecordsCount, "not all interviews have expected TransientState");
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void AddSample_FullBatchFail_NoDuplicatesInReplicatedTable()
        {
            Exception expectedException = null;

            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData()
                            {
                                Id = 1, Name = "quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData() {Id = 1, Values = "q1=1", Counter = 0, Limit = 2},
                                    new CellData() {Id = 2, Values = "q1=2", Counter = 0, Limit = 3},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", ITS = CallOutcome.Busy }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();
            var surveySid = context.GetSurvey("S1").Id;

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1"},
                new InterviewData() {Tag = "S1.I2"},
            };

            BvInterviewAdapter.Insert(new BvInterviewEntity { ID = context.GetInterview("S1.I1").Id + 1, SurveySID = surveySid });
            try
            {
                context.GetSurvey("S1").AddSample(SchedulingMode.Simple, interviews);
            }
            catch (Exception e)
            {
                expectedException = e;
            }

            Assert.IsNotNull(expectedException);
            var countOfRecordsInReplicatedTableQuery = String.Format("SELECT COUNT(*) FROM [{0}]", ReplicationSchemaService.GetDestinationTableName(surveySid));
            Assert.AreEqual(1, new DatabaseEngine().ExecuteScalar<int>(countOfRecordsInReplicatedTableQuery, CommandType.Text));
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void AddSample_TwoPartitionsFail_FailedPartitionsDataIsRemovedFromReplicatedTable()
        {
            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.AddSamplePortionSize = 2;

            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData
                            {
                                Id = 1, Name = "quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values = "q1=1", Counter = 0, Limit = 2},
                                    new CellData {Id = 2, Values = "q1=2", Counter = 0, Limit = 3},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData { Tag = "S1.I1", ITS = CallOutcome.Busy }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();
            var surveySid = context.GetSurvey("S1").Id;

            var interviews = new[]
            {
                new InterviewData {Tag = "S1.I1"},
                new InterviewData {Tag = "S1.I2"},
                new InterviewData {Tag = "S1.I3"},
                new InterviewData {Tag = "S1.I4"},
                new InterviewData {Tag = "S1.I5"},
                new InterviewData {Tag = "S1.I6"}
            };

            BvSvyScheduleAdapter.Insert(new BvSvyScheduleEntity { InterviewID = context.GetInterview("S1.I1").Id + 1, SurveySID = surveySid, ExpireTime = DateTime.Now });
            BvSvyScheduleAdapter.Insert(new BvSvyScheduleEntity { InterviewID = context.GetInterview("S1.I1").Id + 5, SurveySID = surveySid, ExpireTime = DateTime.Now });

            context.GetSurvey("S1").AddSample(SchedulingMode.Simple, interviews);

            var countOfRecordsInReplicatedTableQuery = String.Format("SELECT COUNT(*) FROM [{0}]", ReplicationSchemaService.GetDestinationTableName(surveySid));
            Assert.AreEqual(1 + 2, new DatabaseEngine().ExecuteScalar<int>(countOfRecordsInReplicatedTableQuery, CommandType.Text));
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void AddSample_FcdIsOpen_UnknownDataAreFilteredByFCD()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData
                            {
                                Id = 1, Name = "quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values = "q1=1", Counter = 0, Limit = 2},
                                    new CellData {Id = 2, Values = "q1=2", Counter = 0, Limit = 3},
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var interviews = new[]
            {
                new InterviewData {Tag = "S1.I1", Data="q1="},
                new InterviewData {Tag = "S1.I2", Data="q1=1"},
                new InterviewData {Tag = "S1.I3", Data="q1=2"},
                new InterviewData {Tag = "S1.I4", Data="q1=3"}
            };

            var survey = context.GetSurvey("S1");
            survey.AddSample(SchedulingMode.Simple, interviews);

            context.GetCalls("S1.I4", "S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void SomeQuotasAreClosed_AddSample_NoErrorsCatched()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q2", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData
                            {
                                Id = 1, Name = "quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values = "q1=1", Counter = 2, Limit = 2},
                                    new CellData {Id = 2, Values = "q1=2", Counter = 3, Limit = 3},
                                }
                            },
                            new QuotaData
                            {
                                Id = 2, Name = "quota2", Fields = new[] {"q2"},
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values = "q2=1", Counter = 2, Limit = 2},
                                    new CellData {Id = 2, Values = "q2=2", Counter = 3, Limit = 3},
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var interviews = new[]
            {
                new InterviewData {Tag = "S1.I1", Data="q1=1,q2=1"}
            };

            var survey = context.GetSurvey("S1");
            survey.AddSample(SchedulingMode.Simple, interviews);

            context.GetCalls("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }


        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void UpdateSample_FcdIsOpen_UnknownDataAreFilteredByFCD()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData
                            {
                                Id = 1, Name = "quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values = "q1=1", Counter = 0, Limit = 2},
                                    new CellData {Id = 2, Values = "q1=2", Counter = 0, Limit = 3},
                                }
                            }
                        },
                        Interviews = new []
                        {
                            new InterviewData {Tag="S1.I1",Data="q1=", Call = new CallData()},
                            new InterviewData {Tag="S1.I2",Data="q1=1", Call = new CallData() },
                            new InterviewData {Tag="S1.I3",Data="q1=2", Call = new CallData() },
                            new InterviewData {Tag="S1.I4",Data="q1=3", Call = new CallData() },
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData
                    {
                        Tag="SS1", Script = new TestScript(
                        new Rule(new SubRule(new []{
                            new Action(Action.Operation.SetNewITS, "2"),
                            new Action(Action.Operation.SetNewCallPriority, "10")
                        }),
                            true),
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00")) }
                    }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            context.GetInterview("S1.I2").Data.Data = "q1=3";
            context.GetInterview("S1.I4").Data.Data = "q1=1";

            var survey = context.GetSurvey("S1");
            survey.ProcessSample(SchedulingMode.Full, SampleMode.Update, context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4").Select(x => x.Data).ToArray());

            context.GetCalls("S1.I1", "S1.I4", "S1.I3", "S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled && x.Priority == 10);
            //context.GetCalls("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD && x.Priority == 10);
            context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4").Assert.IsTrue(x => x.TransientState == 2);
        }
    }
}
