using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.SampleTest
{
    [TestClass]
    public class MultipleAssignmentSampleTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void UploadSample_CreateAssignmentWhitGroups_AssignmentCreated()
        {
            const int batchId = 4321;
            const int recordsCount = 10;
            const string projectId = "p0000001";

            int id = PersonGroupService.CreatePersonGroup("G1", string.Empty, new[] { PersonGroupService.RootGroupId });
            int id2 = PersonGroupService.CreatePersonGroup("G2", string.Empty, new[] { PersonGroupService.RootGroupId });

            ConfirmitTools.CreateRespondentTable(
                TestingFramework.DbEngine,
                new List<FormData> { new FormData { Name = "CatiAssignments", SqlType = SqlDataType.Text } });
            ConfirmitTools.FillRespondentTableWithRespondentIdsColumn(
                TestingFramework.DbEngine,
                GetRespondentRecords(recordsCount, id, id2),
                batchId
                );

            int surveySid = BackendToolsObject.CreateSurvey(projectId);
            var survey = SurveyRepository.GetById(surveySid);

            var data = new SampleBatchProcessor(
                ServiceLocator.Resolve<ISampleRecordProcessorFactory>(),
                ServiceLocator.Resolve<IFCDSettings>(),
                ServiceLocator.Resolve<IDatabaseLockTimeouts>());
            data.Process(
                new SampleContext
                {
                    BatchId = batchId,
                    PartitionSize = recordsCount,
                    Survey = survey,
                    RespondentBatchObtainer = ServiceLocator.Resolve<IRespondentBatchObtainer>(),
                    EventDetails = new DummyEventDetails(),
                    SampleDataStorageRepository = ServiceLocator.Resolve<ISampleDataStorageRepository>(),
                    SchedulingMode = SchedulingMode.Simple,
                    TimeZoneReolver = new TimezoneResolver(),
                    StateContainer = new SampleProcessingStateContainer(survey.SID, batchId)
                },
                0
                );
            var records = data.Records;
            Assert.AreEqual(recordsCount, records.Count(r => r.ResourceIds != null));
            var assignmentResources = BvAssignmentResourceAdapter.GetAll();
            Assert.AreEqual(1, assignmentResources.Count);
        }
        
        [TestMethod]
        public void UploadSample_CreateAssignmentWhitAdministrativeGroups_AdministrativeGroupsAreIgnored()
        {
            const int batchId = 4321;
            const int recordsCount = 3;
            const string projectId = "p0000001";
            
            int groupId1 = PersonGroupService.CreatePersonGroup("G1", string.Empty, new[] { PersonGroupService.RootGroupId });
            int groupId2 = PersonGroupService.CreatePersonGroup("G2", string.Empty, new[] { PersonGroupService.RootGroupId }, true);
            int groupId3 = PersonGroupService.CreatePersonGroup("G3", string.Empty, new[] { PersonGroupService.RootGroupId });
            
            ConfirmitTools.CreateRespondentTable(
                TestingFramework.DbEngine,
                new List<FormData> { new FormData { Name = "CatiAssignments", SqlType = SqlDataType.Text } });
            ConfirmitTools.FillRespondentTableWithRespondentIdsColumn(
                TestingFramework.DbEngine,
                new [] {
                    CreateRespondentRecord(1, groupId1, groupId2),
                    CreateRespondentRecord(2, groupId3, groupId2),
                    CreateRespondentRecord(3, groupId1, groupId3)
                },
                batchId);

            int surveySid = BackendToolsObject.CreateSurvey(projectId);
            var survey = SurveyRepository.GetById(surveySid);

            var data = new SampleBatchProcessor(
                ServiceLocator.Resolve<ISampleRecordProcessorFactory>(),
                ServiceLocator.Resolve<IFCDSettings>(),
                ServiceLocator.Resolve<IDatabaseLockTimeouts>());
            data.Process(
                new SampleContext {
                    BatchId = batchId,
                    PartitionSize = recordsCount,
                    Survey = survey,
                    RespondentBatchObtainer = ServiceLocator.Resolve<IRespondentBatchObtainer>(),
                    EventDetails = new DummyEventDetails(),
                    SampleDataStorageRepository = ServiceLocator.Resolve<ISampleDataStorageRepository>(),
                    SchedulingMode = SchedulingMode.Simple,
                    TimeZoneReolver = new TimezoneResolver(),
                    StateContainer = new SampleProcessingStateContainer(survey.SID, batchId)
                },
                0
            );
            var records = data.Records;
            var assignmentResources = BvAssignmentResourceAdapter.GetAll();
            Assert.AreEqual(1, assignmentResources.Count);
            Assert.AreEqual("G1,G3", assignmentResources[0].Name);
            var call1 = CallQueueService.GetCallAndNoLock(surveySid,1);
            var call2 = CallQueueService.GetCallAndNoLock(surveySid,2);
            var call3 = CallQueueService.GetCallAndNoLock(surveySid,3);
            Assert.AreEqual(0, call1.Resource);
            Assert.AreEqual(0, call2.Resource);
            Assert.AreEqual(assignmentResources[0].ID, call3.Resource);
        }

        [TestMethod]
        public void UploadSample_CreateAssignmentWhithoutGroups_AssignmentNotCreated()
        {
            const int batchId = 4322;
            const int recordsCount = 10;
            const string projectId = "p0000001";

            int id = PersonGroupService.CreatePersonGroup("G1", string.Empty, new[] { PersonGroupService.RootGroupId });
            int id2 = PersonGroupService.CreatePersonGroup("G2", string.Empty, new[] { PersonGroupService.RootGroupId });
            int id3 = PersonGroupService.CreatePersonGroup("G3", string.Empty, new[] { PersonGroupService.RootGroupId });
            int id4 = PersonGroupService.CreatePersonGroup("G4", string.Empty, new[] { PersonGroupService.RootGroupId });

            ConfirmitTools.CreateRespondentTable(
                TestingFramework.DbEngine,
                new List<FormData> { new FormData { Name = "CatiAssignments", SqlType = SqlDataType.Text } });
            ConfirmitTools.FillRespondentTableWithRespondentIdsColumn(
                TestingFramework.DbEngine,
                new[]
                {
                    CreateRespondentRecord(1, id, id2),
                    CreateRespondentRecord(1, id4, id3),
                    CreateRespondentRecord(1, id, (id + id2 + id3+ id4)),
                    CreateRespondentRecord(1, id2, (id + id2 + id3+ id4))
                },
                batchId
                );

            int surveySid = BackendToolsObject.CreateSurvey(projectId);
            var survey = SurveyRepository.GetById(surveySid);

            var data = new SampleBatchProcessor(
                ServiceLocator.Resolve<ISampleRecordProcessorFactory>(),
                ServiceLocator.Resolve<IFCDSettings>(),
                ServiceLocator.Resolve<IDatabaseLockTimeouts>());
            data.Process(
                new SampleContext
                {
                    BatchId = batchId,
                    PartitionSize = recordsCount,
                    Survey = survey,
                    RespondentBatchObtainer = ServiceLocator.Resolve<IRespondentBatchObtainer>(),
                    EventDetails = new DummyEventDetails(),
                    SampleDataStorageRepository = ServiceLocator.Resolve<ISampleDataStorageRepository>(),
                    SchedulingMode = SchedulingMode.Simple,
                    TimeZoneReolver = new TimezoneResolver(),
                    StateContainer = new SampleProcessingStateContainer(survey.SID, batchId)
                },
                0
                );

            var assignmentResources = BvAssignmentResourceAdapter.GetAll();
            Assert.AreEqual(2, assignmentResources.Count);
        }

        [TestMethod]
        public void UploadSample_CreateAssignmentForSpecificGroups_AssignmentParitiallyCreated()
        {
            const int batchId = 4322;
            const int recordsCount = 10;
            const string projectId = "p0000001";

            ConfirmitTools.CreateRespondentTable(
                TestingFramework.DbEngine,
                new List<FormData> { new FormData { Name = "CatiAssignments", SqlType = SqlDataType.Text } });
            ConfirmitTools.FillRespondentTableWithRespondentIdsColumn(
                TestingFramework.DbEngine,
                GetRespondentRecords(recordsCount, 1, 2),
                batchId
                );

            int surveySid = BackendToolsObject.CreateSurvey(projectId);
            var survey = SurveyRepository.GetById(surveySid);

            var data = new SampleBatchProcessor(
                ServiceLocator.Resolve<ISampleRecordProcessorFactory>(),
                ServiceLocator.Resolve<IFCDSettings>(),
                ServiceLocator.Resolve<IDatabaseLockTimeouts>());
            data.Process(
                new SampleContext
                {
                    BatchId = batchId,
                    PartitionSize = recordsCount,
                    Survey = survey,
                    RespondentBatchObtainer = ServiceLocator.Resolve<IRespondentBatchObtainer>(),
                    EventDetails = new DummyEventDetails(),
                    SampleDataStorageRepository = ServiceLocator.Resolve<ISampleDataStorageRepository>(),
                    SchedulingMode = SchedulingMode.Simple,
                    TimeZoneReolver = new TimezoneResolver(),
                    StateContainer = new SampleProcessingStateContainer(survey.SID, batchId)
                },
                0
                );

            var records = data.Records;
            Assert.AreEqual(recordsCount, records.Count(r => r.ResourceIds != null));
            var assignmentResources = BvAssignmentResourceAdapter.GetAll();
            Assert.AreEqual(0, assignmentResources.Count);
        }

        private RespondentRecord CreateRespondentRecord(int id, int groupId1, int groupId2)
        {
            return new RespondentRecord
            {
                Sid = id.ToString(CultureInfo.InvariantCulture),
                InterviewId = id,
                RespondentName = "resp" + id,
                RespondentPhone = string.Empty,
                LastCallTime = null,
                TotalDuration = 1000,
                ExtensionNumber = string.Empty,
                DialAttempts = 10,
                TimeZoneId = 4,
                LastChannelId = 0,
                Resource = id,
                ResourceIds = string.Format("{0},{1}", groupId1, groupId2)
            };
        }

        private IEnumerable<RespondentRecord> GetRespondentRecords(int count, int groupId1, int groupId2)
        {
            for (int i = 1; i <= count; ++i)
            {
                yield return CreateRespondentRecord(i, groupId1, groupId2);
            }
        }
    }
}
