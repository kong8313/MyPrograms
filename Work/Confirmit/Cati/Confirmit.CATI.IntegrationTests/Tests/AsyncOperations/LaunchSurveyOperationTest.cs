using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.Replication;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class LaunchSurveyOperationTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private string _projectId;
        private DatabaseEngine _confirmitSurveyDb;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _confirmitSurveyDb = ReplicationTools.GetConfirmitSurveyDb(out _projectId);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LaunchSurvey_FirstLaunch_SurveyIsLaunchedSuccessfuly()
        {
            var confirmitDatabaseProvider = ServiceLocator.Resolve<IConfirmitDatabaseProvider>();

            var replicatedTables = ReplicationTools.GetTestData();
            var parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "MaximL", "LeonidS" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 0,
                    EnforceHttps = false,
                    NotificationEmail = "a@firmsw.no",
                    OpenEndReview = false,
                    ProjectName = "Survey name",
                    ReplicationStatus = false,
                    ScreenRecording = false,
                    SupportBlacklist = false,
                    VoiceRecording = false,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.AreEqual(1, survey.DialMode);//DialingMode
            Assert.AreEqual(false, survey.EnforceHttps);//EnforceHttps
            Assert.AreEqual("a@firmsw.no", survey.NotificationEmail);//NotificationEmail
            Assert.AreEqual(0, survey.ForceOpnRev);//OpenEndReview
            Assert.AreEqual("Survey name", survey.Description);//ProjectName
            Assert.AreEqual(false, survey.ReplicationStatus);//ReplicationStatus
            Assert.AreEqual(false, survey.InterviewScreenRecording);//ScreenRecording
            Assert.AreEqual(false, survey.IsTelephoneBlacklistSupported);//SupportBlacklist
            Assert.AreEqual(0, survey.RecWholeInt);//VoiceRecording
            Assert.AreEqual(confirmitDatabaseProvider.GetSqlServerName(_projectId), survey.SurveySqlServerName);

            var users = BvUserSurveyPermissionAdapter.GetByCondition("SurveySID = @SurveySID",
                                                         new SqlParameter("@SurveySID", survey.SID)).Select(
                                                             x => x.UserName).ToArray();
            CollectionAssert.AreEquivalent(new[] { "MaximL", "LeonidS" }, users);
            ReplicationTools.CheckDbByTestData(survey.SID, replicatedTables);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LaunchSurvey_SecondLaunch_SurveyIsLaunchedSuccessfuly()
        {
            var replicatedTables = ReplicationTools.GetTestData();
            var parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "MaximL", "LeonidS" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 1,
                    EnforceHttps = false,
                    NotificationEmail = "a@firmsw.no",
                    OpenEndReview = false,
                    ProjectName = "Survey name",
                    ReplicationStatus = false,
                    ScreenRecording = false,
                    SupportBlacklist = false,
                    VoiceRecording = false,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            BackendTools.CreateInterviewWithCall(SurveyRepository.GetByName(_projectId).SID);

            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            replicatedTables = new[] { replicatedTables[0] };

            parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "AlexanderL", "SergeyC" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 2,
                    EnforceHttps = true,
                    NotificationEmail = "a1@firmsw.no",
                    OpenEndReview = true,
                    ProjectName = "Survey name(2)",
                    ReplicationStatus = true,
                    ScreenRecording = true,
                    SupportBlacklist = true,
                    VoiceRecording = true,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.AreEqual(2, survey.DialMode);//DialingMode
            Assert.AreEqual(true, survey.EnforceHttps);//EnforceHttps
            Assert.AreEqual("a1@firmsw.no", survey.NotificationEmail);//NotificationEmail
            Assert.AreEqual(1, survey.ForceOpnRev);//OpenEndReview
            Assert.AreEqual("Survey name(2)", survey.Description);//ProjectName
            Assert.AreEqual(true, survey.ReplicationStatus);//ReplicationStatus
            Assert.AreEqual(true, survey.InterviewScreenRecording);//ScreenRecording
            Assert.AreEqual(true, survey.IsTelephoneBlacklistSupported);//SupportBlacklist
            Assert.AreEqual(1, survey.RecWholeInt);//VoiceRecording

            var users = BvUserSurveyPermissionAdapter.GetByCondition("SurveySID = @SurveySID",
                                                         new SqlParameter("@SurveySID", survey.SID)).Select(
                                                             x => x.UserName).ToArray();
            CollectionAssert.AreEquivalent(new[] { "AlexanderL", "SergeyC" }, users);
            ReplicationTools.CheckDbByTestData(survey.SID, replicatedTables);
            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LaunchSurvey_LaunchWithRemoveData_SurveyIsLaunchedSuccessfuly()
        {
            var surveyId = _backendTools.CreateSurvey(_projectId);
            BackendTools.CreateInterviewWithCall(surveyId);
            var personId = PersonTools.CreatePerson("test_user");

            var tbhe = new BvTimeBreaksHistoryEntity()
            {
                CallCenterId = 1,
                Duration = 10,
                InterviewerId = personId,
                StartTime = DateTime.Now.AddMonths(-1),
            };
            BvTimeBreaksHistoryAdapter.Insert(tbhe);
            tbhe.SurveyId = surveyId;
            BvTimeBreaksHistoryAdapter.Insert(tbhe);

            BvSamplesAdapter.Insert(new BvSamplesEntity() { BatchID = 1, CountInterviews = 1, SampleType = 1, StartedTime = DateTime.Now, State = 1, SurveySID = surveyId });
            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            var replicatedTables = ReplicationTools.GetTestData();
            var parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "MaximL", "LeonidS" },
                RemoveData = true,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 3,
                    EnforceHttps = false,
                    NotificationEmail = "a@firmsw.no",
                    OpenEndReview = false,
                    ProjectName = "Survey name",
                    ReplicationStatus = false,
                    ScreenRecording = false,
                    SupportBlacklist = false,
                    VoiceRecording = false,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.AreEqual(3, survey.DialMode);//DialingMode
            Assert.AreEqual(false, survey.EnforceHttps);//EnforceHttps
            Assert.AreEqual("a@firmsw.no", survey.NotificationEmail);//NotificationEmail
            Assert.AreEqual(0, survey.ForceOpnRev);//OpenEndReview
            Assert.AreEqual("Survey name", survey.Description);//ProjectName
            Assert.AreEqual(false, survey.ReplicationStatus);//ReplicationStatus
            Assert.AreEqual(false, survey.InterviewScreenRecording);//ScreenRecording
            Assert.AreEqual(false, survey.IsTelephoneBlacklistSupported);//SupportBlacklist
            Assert.AreEqual(0, survey.RecWholeInt);//VoiceRecording

            var users = BvUserSurveyPermissionAdapter.GetByCondition("SurveySID = @SurveySID",
                                                         new SqlParameter("@SurveySID", survey.SID)).Select(
                                                             x => x.UserName).ToArray();
            CollectionAssert.AreEquivalent(new[] { "MaximL", "LeonidS" }, users);
            ReplicationTools.CheckDbByTestData(survey.SID, replicatedTables);

            Assert.AreEqual(0, BvSvyScheduleAdapter.GetAll().Count);
            Assert.AreEqual(0, BvSamplesAdapter.GetAll().Count);
            Assert.AreEqual(0, BvAsyncOperationQueueAdapter.GetByCondition("Type = @Type",
                new SqlParameter("@Type", (int)OperationTypes.SampleUpload)).Count);

            Assert.AreEqual(1, BvTimeBreaksHistoryAdapter.GetAll().Count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LaunchSurvey_LaunchWithRemoveData_SurveyIsLaunchedSuccessfulyAndAgregatedDataWereReseted()
        {
            var surveyId = _backendTools.CreateSurvey(_projectId);
            BackendTools.CreateInterviewWithCall(surveyId);

            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            BackendTools.ForceProcessingAsyncTriggers();
            var surveyAgregatedData = BvAggregateSurveyAdapter.GetAll().Single();
            Assert.AreEqual(1, surveyAgregatedData.ScheduledCallsCount);
            Assert.AreEqual(1, surveyAgregatedData.SuspendedCallsCount);


            var replicatedTables = ReplicationTools.GetTestData();
            var parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "MaximL", "LeonidS" },
                RemoveData = true,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 1,
                    EnforceHttps = false,
                    NotificationEmail = "a@firmsw.no",
                    OpenEndReview = false,
                    ProjectName = "Survey name",
                    ReplicationStatus = false,
                    ScreenRecording = false,
                    SupportBlacklist = false,
                    VoiceRecording = false,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            BackendTools.ForceProcessingAsyncTriggers();
            surveyAgregatedData = BvAggregateSurveyAdapter.GetAll().Single();
            Assert.AreEqual(0, surveyAgregatedData.ScheduledCallsCount);
            Assert.AreEqual(0, surveyAgregatedData.SuspendedCallsCount);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LaunchSurvey_SecondLaunchWithFailedReplication_SurveyIsLaunchedSuccessfulyAndSecondReplicationIsSuccess()
        {
            BackendTools.ResetInterviewId();

            var c1 = new ReplicationColumnInfo { DataType = SqlDataType.Int, Id = 32, Name = "ITS", QuotaIds = null };
            var p1 = new ColumnInfo { DataType = SqlDataType.Int, Name = "respid" };
            var t1 = new TableInfo { Name = "response_control", ReplicationColumns = new[] { c1 }, PrimaryKeyColumns = new[] { p1 } };

            var replicatedTables = new[] { t1};
            _confirmitSurveyDb.ExecuteNonQuery(@"alter table response_control alter column respid int not null", CommandType.Text);
            _confirmitSurveyDb.ExecuteNonQuery(@"alter table response_control add primary key( respid )
                                                 insert into response_control select 1, 1", CommandType.Text);

            BackendTools.EnableChangeTracking(_confirmitSurveyDb, replicatedTables);

            var parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "MaximL", "LeonidS" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 1,
                    EnforceHttps = false,
                    NotificationEmail = "a@firmsw.no",
                    OpenEndReview = false,
                    ProjectName = "Survey name",
                    ReplicationStatus = false,
                    ScreenRecording = false,
                    SupportBlacklist = false,
                    VoiceRecording = false,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            BackendTools.CreateInterviewWithCall(SurveyRepository.GetByName(_projectId).SID);

            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            replicatedTables = new[] { replicatedTables[0] };

            parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "AlexanderL", "SergeyC" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 4,
                    EnforceHttps = true,
                    NotificationEmail = "a1@firmsw.no",
                    OpenEndReview = true,
                    ProjectName = "Survey name(2)",
                    ReplicationStatus = true,
                    ScreenRecording = true,
                    SupportBlacklist = true,
                    VoiceRecording = true,
                }
            };

            var currentReplicationService = ServiceLocator.Resolve<IReplicationService>();
            var currentReplicationSchemaService = ServiceLocator.Resolve<IReplicationSchemaService>();
            var stub = _framework.RegistryStub<IReplicationService, StubIReplicationService>();
            stub.RereadSurveyReplicatedDataInt32StringCancellationToken = (surveyId, reason, cancellationToken) => { throw new Exception(); };
            var replicationSchemaServiceStub = _framework.RegistryStub<IReplicationSchemaService, StubIReplicationSchemaService>();
            replicationSchemaServiceStub.Inner = currentReplicationSchemaService;
            replicationSchemaServiceStub.IsReplicationSchemaChangedInt32ArrayOfTableInfo = (surveyId, tables) => { return true; };
           
            _backendTools.LaunchSurvey(_projectId, parameters);

            stub.RereadSurveyReplicatedDataInt32StringCancellationToken = (surveyId, reason, cancellationToken) => currentReplicationService.RereadSurveyReplicatedData(surveyId, reason, cancellationToken);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.AreEqual(4, survey.DialMode);//DialingMode
            Assert.AreEqual(true, survey.EnforceHttps);//EnforceHttps
            Assert.AreEqual("a1@firmsw.no", survey.NotificationEmail);//NotificationEmail
            Assert.AreEqual(1, survey.ForceOpnRev);//OpenEndReview
            Assert.AreEqual("Survey name(2)", survey.Description);//ProjectName
            Assert.AreEqual(true, survey.ReplicationStatus);//ReplicationStatus
            Assert.AreEqual(true, survey.InterviewScreenRecording);//ScreenRecording
            Assert.AreEqual(true, survey.IsTelephoneBlacklistSupported);//SupportBlacklist
            Assert.AreEqual(1, survey.RecWholeInt);//VoiceRecording

            var users = BvUserSurveyPermissionAdapter.GetByCondition("SurveySID = @SurveySID",
                                                         new SqlParameter("@SurveySID", survey.SID)).Select(
                                                             x => x.UserName).ToArray();
            CollectionAssert.AreEquivalent(new[] { "AlexanderL", "SergeyC" }, users);
            ReplicationTools.CheckDbByTestData(survey.SID, replicatedTables);
            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            var countOfRecordsInReplicatedTableQuery = String.Format("SELECT COUNT(*) FROM [{0}]", ReplicationSchemaService.GetDestinationTableName(survey.SID));
            Assert.AreEqual(0, new DatabaseEngine().ExecuteScalar<int>(countOfRecordsInReplicatedTableQuery, CommandType.Text));
            currentReplicationService.RunForceReplication(survey.SID, CancellationToken.None);
            Assert.AreEqual(1, new DatabaseEngine().ExecuteScalar<int>(countOfRecordsInReplicatedTableQuery, CommandType.Text));
        }


        [TestMethod, Owner(@"FIRM\Egork")]
        public void SecondSurveyLaunch_ReplicationSchemaNotChanged_ReplicationSchemaUpdateAndReplicationNotCalled()
        {
            var replicatedTables = ReplicationTools.GetTestData();
            var parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "MaximL", "LeonidS" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 1,
                    EnforceHttps = false,
                    NotificationEmail = "a@firmsw.no",
                    OpenEndReview = false,
                    ProjectName = "Survey name",
                    ReplicationStatus = false,
                    ScreenRecording = false,
                    SupportBlacklist = false,
                    VoiceRecording = false,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            BackendTools.CreateInterviewWithCall(SurveyRepository.GetByName(_projectId).SID);

            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "AlexanderL", "SergeyC" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 2,
                    EnforceHttps = true,
                    NotificationEmail = "a1@firmsw.no",
                    OpenEndReview = true,
                    ProjectName = "Survey name(2)",
                    ReplicationStatus = true,
                    ScreenRecording = true,
                    SupportBlacklist = true,
                    VoiceRecording = true,
                }
            };
            bool schemaUpdateCalled = false, replicationCalled = false;
            var currentReplicationService = ServiceLocator.Resolve<IReplicationService>();
            var currentReplicationSchemaService = ServiceLocator.Resolve<IReplicationSchemaService>();
            var replicationServieStub = _framework.RegistryStub<IReplicationService, StubIReplicationService>();
            var replicationSchemaServiceStub = _framework.RegistryStub<IReplicationSchemaService, StubIReplicationSchemaService>();
            replicationServieStub.Inner = currentReplicationService;
            replicationSchemaServiceStub.Inner = currentReplicationSchemaService;
            replicationServieStub.RereadSurveyReplicatedDataInt32StringCancellationToken = (surveyId, reason, cancellationToken) =>
            { 
                replicationCalled = true;
                currentReplicationService.RereadSurveyReplicatedData(surveyId, reason, cancellationToken);
            };
            replicationSchemaServiceStub.UpdateSurveyReplicationSchemeInt32ArrayOfTableInfo = (surveySid, tables) =>
            {
                schemaUpdateCalled = true;
                currentReplicationSchemaService.UpdateSurveyReplicationScheme(surveySid, tables);
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.AreEqual(2, survey.DialMode);//DialingMode
            Assert.AreEqual(true, survey.EnforceHttps);//EnforceHttps
            Assert.AreEqual("a1@firmsw.no", survey.NotificationEmail);//NotificationEmail
            Assert.AreEqual(1, survey.ForceOpnRev);//OpenEndReview
            Assert.AreEqual("Survey name(2)", survey.Description);//ProjectName
            Assert.AreEqual(true, survey.ReplicationStatus);//ReplicationStatus
            Assert.AreEqual(true, survey.InterviewScreenRecording);//ScreenRecording
            Assert.AreEqual(true, survey.IsTelephoneBlacklistSupported);//SupportBlacklist
            Assert.AreEqual(1, survey.RecWholeInt);//VoiceRecording

            var users = BvUserSurveyPermissionAdapter.GetByCondition("SurveySID = @SurveySID",
                                                         new SqlParameter("@SurveySID", survey.SID)).Select(
                                                             x => x.UserName).ToArray();
            CollectionAssert.AreEquivalent(new[] { "AlexanderL", "SergeyC" }, users);
            ReplicationTools.CheckDbByTestData(survey.SID, replicatedTables);
            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            Assert.AreEqual(false, schemaUpdateCalled);
            Assert.AreEqual(false, replicationCalled);
        }

        [TestMethod, Owner(@"FIRM\Egork")]
        public void SecondSurveyLaunch_ReplicationSchemaChanged_ReplicationSchemaUpdateAndReplicationCalled()
        {
            var replicatedTables = ReplicationTools.GetTestData();
            var parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "MaximL", "LeonidS" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 1,
                    EnforceHttps = false,
                    NotificationEmail = "a@firmsw.no",
                    OpenEndReview = false,
                    ProjectName = "Survey name",
                    ReplicationStatus = false,
                    ScreenRecording = false,
                    SupportBlacklist = false,
                    VoiceRecording = false,
                }
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            BackendTools.CreateInterviewWithCall(SurveyRepository.GetByName(_projectId).SID);

            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);
            replicatedTables = ReplicationTools.GetTestData2();
            parameters = new LaunchSurveyParameters
            {
                PermittedUsers = new[] { "AlexanderL", "SergeyC" },
                RemoveData = false,
                ReplicatedTables = replicatedTables,
                SurveyProperties = new SurveyProperties
                {
                    CreatedUserName = "MaximL",
                    CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                    DialingMode = 2,
                    EnforceHttps = true,
                    NotificationEmail = "a1@firmsw.no",
                    OpenEndReview = true,
                    ProjectName = "Survey name(2)",
                    ReplicationStatus = true,
                    ScreenRecording = true,
                    SupportBlacklist = true,
                    VoiceRecording = true,
                }
            };
            bool schemaUpdateCalled = false, replicationCalled = false;
            var currentReplicationService = ServiceLocator.Resolve<IReplicationService>();
            var currentReplicationSchemaService = ServiceLocator.Resolve<IReplicationSchemaService>();
            var replicationServieStub = _framework.RegistryStub<IReplicationService, StubIReplicationService>();
            var replicationSchemaServiceStub = _framework.RegistryStub<IReplicationSchemaService, StubIReplicationSchemaService>();
            replicationServieStub.Inner = currentReplicationService;
            replicationSchemaServiceStub.Inner = currentReplicationSchemaService;
            replicationServieStub.RereadSurveyReplicatedDataInt32StringCancellationToken = (surveyId, reason, cancellationToken) =>
            {
                replicationCalled = true;
                currentReplicationService.RereadSurveyReplicatedData(surveyId, reason, cancellationToken);
            };
            replicationSchemaServiceStub.UpdateSurveyReplicationSchemeInt32ArrayOfTableInfo = (surveySid, tables) =>
            {
                schemaUpdateCalled = true;
                currentReplicationSchemaService.UpdateSurveyReplicationScheme(surveySid, tables);
            };

            _backendTools.LaunchSurvey(_projectId, parameters);

            var survey = SurveyRepository.GetByName(_projectId);
            Assert.AreEqual(2, survey.DialMode);//DialingMode
            Assert.AreEqual(true, survey.EnforceHttps);//EnforceHttps
            Assert.AreEqual("a1@firmsw.no", survey.NotificationEmail);//NotificationEmail
            Assert.AreEqual(1, survey.ForceOpnRev);//OpenEndReview
            Assert.AreEqual("Survey name(2)", survey.Description);//ProjectName
            Assert.AreEqual(true, survey.ReplicationStatus);//ReplicationStatus
            Assert.AreEqual(true, survey.InterviewScreenRecording);//ScreenRecording
            Assert.AreEqual(true, survey.IsTelephoneBlacklistSupported);//SupportBlacklist
            Assert.AreEqual(1, survey.RecWholeInt);//VoiceRecording

            var users = BvUserSurveyPermissionAdapter.GetByCondition("SurveySID = @SurveySID",
                                                         new SqlParameter("@SurveySID", survey.SID)).Select(
                                                             x => x.UserName).ToArray();
            CollectionAssert.AreEquivalent(new[] { "AlexanderL", "SergeyC" }, users);
            ReplicationTools.CheckDbByTestData(survey.SID, replicatedTables);
            Assert.AreEqual(1, BvSvyScheduleAdapter.GetAll().Count);

            Assert.AreEqual(true, schemaUpdateCalled);
            Assert.AreEqual(true, replicationCalled);
        }
    }
}
