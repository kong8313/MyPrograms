using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation.Fakes;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.MultiUserEnvironment.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Replication
{
    [TestClass]
    public class UpdateSchemeTest : BaseMockedIntegrationTest
    {
        private string _projectId;
        private DatabaseEngine _confirmitSurveyDb;

        public override void OnPostTestInitialize()
        {
            TestingFramework.RegistryStub<IQuotaInfoService, StubIQuotaInfoService>().GetQuotaInfosInt32 = id => new QuotaInfo[]{};

            _confirmitSurveyDb = ReplicationTools.GetConfirmitSurveyDb(out _projectId);
        }

        /// <summary>
        /// 1. Create survey.
        /// 2. Update replication scheme.
        /// 3. Remove survey using management service.
        /// 4. Check that survey successfully removed and no replication records in DB.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateScheme_UpdateSchemeAndDeleteSurvey_NoReplicationRecordsExists()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;

            TableInfo[] testData = ReplicationTools.GetTestData();
            
            RelaunchSurvey(projectId, testData);

            var destinationTableName = SurveyRepository.GetById(surveySid).DestinationTableName;

            BackendTools.DeleteSurvey(projectId);

            ReplicationTools.CheckReplicationSchemeIsEmpty(surveySid, destinationTableName, testData);
        }

        private void RelaunchSurvey(string projectId, TableInfo[] testData)
        {
            BackendToolsObject.LaunchSurvey(projectId, new LaunchSurveyParameters()
                                                            {
                                                                PermittedUsers = new[] {"User1"},
                                                                RemoveData = false,
                                                                ReplicatedTables = testData,
                                                                SurveyProperties = new SurveyProperties()
                                                                                   {
                                                                                       CfSqlServerConnectionString = _confirmitSurveyDb.ConnectionString,
                                                                                       ReplicationStatus = true,
                                                                                       ProjectName = projectId,
                                                                                       DialingMode = 0,
                                                                                       OpenEndReview = false,
                                                                                       VoiceRecording = false,
                                                                                       ScreenRecording = false
                                                                                   }
                                                            });
        }

        /// <summary>
        /// 1. Create survey.
        /// 2. Update replication scheme.
        /// 3. Check that replication scheme is saved properly and table for replicated data created.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateScheme_UpdateScheme_CheckValidRecordsExists()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;
            TableInfo[] testData = ReplicationTools.GetTestData();

            RelaunchSurvey(projectId, testData);

            ReplicationTools.CheckDbByTestData(surveySid, testData);
        }

        /// <summary>
        /// 1. Create survey.
        /// 2. Update replication scheme.
        /// 3. Update replication scheme with new data.
        /// 4. Check that new replication scheme is saved properly and table for replicated data created.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateScheme_UpdateSchemeTwice_CheckValidRecordsExists()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;

            TableInfo[] testData = ReplicationTools.GetTestData();
            RelaunchSurvey(projectId, testData);

            testData = ReplicationTools.GetTestData2();
            RelaunchSurvey(projectId, testData);

            ReplicationTools.CheckDbByTestData(surveySid, testData);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateScheme_SomeRespondentAndResponseHasSameFields_CorrespondingResponseFieldsRemoved()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;

            TableInfo[] testData = ReplicationTools.GetTestDataSomeResponseNamesEqualToSystemRespondentFields();
            RelaunchSurvey(projectId, testData);

            ReplicationTools.CheckDbByTestData(surveySid, testData);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateScheme_AllRespondentAndResponseHasSameFields_CorrespondingResponseTableRemoved()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;

            TableInfo[] testData = ReplicationTools.GetTestDataAllResponseNamesEqualToSystemRespondentFields();
            RelaunchSurvey(projectId, testData);

            ReplicationTools.CheckDbByTestData(surveySid, testData);
        }

        /// <summary>
        /// 1. Create 2 surveys.
        /// 2. Update replication scheme for both surveys.
        /// 3. Check that new replication scheme is saved properly and table for replicated data created for both surveys.
        /// 4. Remove 2nd survey and check that data removed for 2nd survey and still correct for the first one.
        /// 5. Remove 1st survey and check that data removed for both survey.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateScheme_2Surveys_CheckValidRecordsForBoth()
        {
            int surveySid1 = ReplicationTools.AddSurvey();
            int surveySid2 = ReplicationTools.AddSurvey();
            string projectId1 = SurveyRepository.GetById(surveySid1).Name;
            string projectId2 = SurveyRepository.GetById(surveySid2).Name;

            TableInfo[] testData1 = ReplicationTools.GetTestData();
            RelaunchSurvey(projectId1, testData1);

            TableInfo[] testData2 = ReplicationTools.GetTestData2();
            RelaunchSurvey(projectId2, testData2);

            ReplicationTools.CheckDbByTestData(surveySid1, testData1);
            ReplicationTools.CheckDbByTestData(surveySid2, testData2);

            string destinationTable1 = SurveyRepository.GetById(surveySid1).DestinationTableName;
            string destinationTable2 = SurveyRepository.GetById(surveySid2).DestinationTableName;

            BackendTools.DeleteSurvey(projectId2);

            ReplicationTools.CheckDbByTestData(surveySid1, testData1);
            ReplicationTools.CheckReplicationSchemeIsEmpty(surveySid2, destinationTable2, testData2);

            BackendTools.DeleteSurvey(projectId1);

            ReplicationTools.CheckReplicationSchemeIsEmpty(surveySid1, destinationTable1, testData1);
            ReplicationTools.CheckReplicationSchemeIsEmpty(surveySid2, destinationTable2, testData2);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(38676), MultiUserTest]
        public void UpdateScheme_Delete3SurveysSimultaniously_NoDeadlocksAndReplicationSchemeIsEmpty()
        {
            const int iterations = 3;
            var deleteSurveyJobs = new List<Job>();
            var surveySids = new List<int>();

            for (int i = 0; i < iterations; i++)
            {
                int surveySid = BackendToolsObject.CreateSurvey("p0000000" + i);
                string projectId = SurveyRepository.GetById(surveySid).Name;
                surveySids.Add(surveySid);

                deleteSurveyJobs.Add(new Job(delegate { BackendTools.DeleteSurvey(projectId); }));
            }

            new JobsExecutor(deleteSurveyJobs).Run();

            foreach (int sid in surveySids)
            {
                ReplicationTools.CheckReplicationSchemeIsEmpty(sid, ReplicationSchemaService.GetDestinationTableName(sid), new TableInfo[0]);
            }
        }

        /// <summary>
        /// 1. Create survey.
        /// 2. Update replication scheme.
        /// 3. Remove survey using management service. 
        ///    Simulate exception while survey addition after UpdateSurveyReplicationScheme.
        /// 4. Check that survey deletion was rolled back and all the data still exists in the DB.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT"), Bug(39024)]
        public void UpdateScheme_DeleteSurveyFailedAfterUpdateScheme_NoDataRemovedFromDb()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;

            var testData = ReplicationTools.GetTestData();
            RelaunchSurvey(projectId, testData);

            
            var originalRss = ServiceLocator.Resolve<IReplicationSchemaService>();
            var fakeRss = TestingFramework.RegistryStub<IReplicationSchemaService, StubIReplicationSchemaService>();
            fakeRss.UpdateSurveyReplicationSchemeInt32ArrayOfTableInfo += (surveyId, tables) =>
            {
                originalRss.UpdateSurveyReplicationScheme(surveyId, tables);
                throw new Exception();
            };

            TestAssert.AreEqual(BackendTools.DeleteSurvey(projectId).State, AsyncOperationState.Failed);
            
            ReplicationTools.CheckDbByTestData(surveySid, testData);
        }

        /// <summary>
        /// 1. Create survey.
        /// 2. Update replication scheme.
        ///    Simulate exception while RunForceReplication in the UpdateSurveyReplicationScheme method.
        /// 3. Check that next Update replication scheme will pass successfully.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void UpdateScheme_ReplicationFailed_NextUpdateSchemePassedSuccessfully()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;
            var testData = ReplicationTools.GetTestData();

            var rs = IntegrationTestingFramework.Instance.RegistryStub<IReplicationService, StubIReplicationService>();
            rs.RunForceReplicationInt32CancellationToken = (x, ct) => { throw new Exception(); };
            
            TestAssert.InvokeMethodAndVerifyExceptionThrown<Exception>(
                () => new ManagementService().UpdateSurveyReplicationScheme(projectId, testData));

            ReplicationTools.CheckDbByTestData(surveySid, testData);

            ServiceLocator.Resolve<IServiceRegistrator>().Register<IReplicationService, ReplicationService>();

            new ManagementService().UpdateSurveyReplicationScheme(projectId, testData);
            ReplicationTools.CheckDbByTestData(surveySid, testData);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Bug(39062), MultiUserTest]
        public void UpdateScheme_TryToDeleteAndAddSurveysSimultaneously_NoDeadlocksAndReplicationSchemeIsCorrect()
        {
            var projectIds1 = new[] { "p00000001", "p00000002", "p00000003" };
            var projectIds2 = new[] { "p00000004", "p00000005", "p00000006" };
            var surveyService = ServiceLocator.Resolve<ISurveyService>();

            var launchSurvey1Jobs = projectIds1.Select(x => 
                new Job(
                    delegate 
                    {
                        surveyService.CreateSurvey(x, "", IntegrationTestingFramework.Instance.DbEngine.ConnectionString, string.Empty, string.Empty);
                    }));

            new JobsExecutor(launchSurvey1Jobs).Run();

            var surveySids = projectIds1.Select(x => SurveyRepository.GetByName(x).SID).ToArray();

            var launchSurvey2Jobs = projectIds2.Select(x =>
                new Job(
                    delegate
                    {
                        surveyService.CreateSurvey(x, "", IntegrationTestingFramework.Instance.DbEngine.ConnectionString, string.Empty, string.Empty);
                    }));

            var deleteSurvey1Jobs = projectIds1.Select(x =>
                new Job(
                    delegate
                    {
                        BackendTools.DeleteSurvey(x); 
                    }));

            new JobsExecutor(launchSurvey2Jobs.Concat(deleteSurvey1Jobs)).Run();

            foreach (var sid in surveySids.Concat(projectIds2.Select(x => SurveyRepository.GetByName(x).SID)))
            {
                ReplicationTools.CheckReplicationSchemeIsEmpty(sid, ReplicationSchemaService.GetDestinationTableName(sid), new TableInfo[0]);
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(38845)]
        public void UpdateScheme_2SurveysWithQuotas_CheckValidRecordsAndQuotas()
        {
            string projectId1;
            DatabaseEngine confirmitSurveyDb1 = ReplicationTools.GetConfirmitSurveyDb(out projectId1);
            int surveySid1 = BackendToolsObject.CreateSurvey(projectId1, confirmitSurveyDb1.ConnectionString);
            TestQuota.Create(confirmitSurveyDb1, surveySid1, 1, new[] { "q1" }, new[] { 2 });
            TestQuota.Create(confirmitSurveyDb1, surveySid1, 2, new[] { "q1" }, new[] { 2 });

            string projectId2;
            DatabaseEngine confirmitSurveyDb2 = ReplicationTools.GetConfirmitSurveyDb(out projectId2);
            int surveySid2 = BackendToolsObject.CreateSurvey(projectId2, confirmitSurveyDb2.ConnectionString);
            TestQuota.Create(confirmitSurveyDb2, surveySid2, 1, new[] { "q1" }, new[] { 2 });

            TableInfo[] testData1 = ReplicationTools.GetTestData();
            testData1[0].ReplicationColumns[0].QuotaIds = new[] {1};
            testData1[0].ReplicationColumns[1].QuotaIds = new[] {1, 2};
            RelaunchSurvey(projectId1, testData1);

            TableInfo[] testData2 = ReplicationTools.GetTestData2();
            testData1[0].ReplicationColumns[0].QuotaIds = new[] {1};
            RelaunchSurvey(projectId2, testData2);

            ReplicationTools.CheckDbByTestData(surveySid1, testData1);
            ReplicationTools.CheckDbByTestData(surveySid2, testData2);

            string destinationTable1 = SurveyRepository.GetById(surveySid1).DestinationTableName;
            string destinationTable2 = SurveyRepository.GetById(surveySid2).DestinationTableName;

            BackendTools.DeleteSurvey(projectId2);

            ReplicationTools.CheckDbByTestData(surveySid1, testData1);
            ReplicationTools.CheckReplicationSchemeIsEmpty(surveySid2, destinationTable2, testData2);

            BackendTools.DeleteSurvey(projectId1);

            ReplicationTools.CheckReplicationSchemeIsEmpty(surveySid1, destinationTable1, testData1);
            ReplicationTools.CheckReplicationSchemeIsEmpty(surveySid2, destinationTable2, testData2);
        }
    }
}
