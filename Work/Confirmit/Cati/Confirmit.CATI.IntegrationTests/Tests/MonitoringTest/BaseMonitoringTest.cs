using System;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Monitoring.Common;
using Confirmit.CATI.Monitoring.Common.Contracts;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.MonitoringTest
{
    /// <summary>
    /// Summary description for BaseMonitoringTest
    /// </summary>
    [TestClass]
    public class BaseMonitoringTest : BaseMockedIntegrationTest
    {
        protected int SurveyId;
        protected string PersonName = "i1";
        protected string PersonPassword = "password";
        protected int PersonId;
        protected BvInterviewEntity Interview;
        protected BvCallEntity Call;
        protected IDeferredMonitoringService DeferredMonitoringService;
        protected IPersonDeferredMonitoringRepository PersonDeferredMonitoringRepository;

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            DeferredMonitoringService = ServiceLocator.Resolve<IDeferredMonitoringService>();
            PersonDeferredMonitoringRepository = ServiceLocator.Resolve<IPersonDeferredMonitoringRepository>();
        }
        
        
        protected virtual void CreateSurveyPersonInterviewCall()
        {
            SurveyId = BackendToolsObject.CreateSurvey("p000132");

            BackendToolsObject.LaunchAllHoursScript();
            _surveyStateService.Open(SurveyId);

            PersonId = PersonTools.CreatePerson(PersonName, PersonPassword, AgentTaskChoiceMode.Automatic);

            BackendTools.AssignCatiPersonToSurvey(SurveyId, PersonId);

            Interview = BackendTools.NewInterview(SurveyId);
            BackendTools.CreateInterview(Interview);

            Call = BackendTools.NewCall(Interview);
            BackendTools.CreateCall(Call);
            Call.CallID = CallQueueService.GetCallAndNoLock(SurveyId, Interview.ID).CallID;

            BackendTools.LoginPerson(PersonId, "");
            var task = TaskService.LookupByPersonSid(PersonId, SurveyId, Interview.ID);
            TaskRepository.Update(task);
        }
        
        protected IdentityObject Identity;
        protected DeferredIdentityObject DeferredIdentity;

        protected virtual BvPersonDeferredMonitoringPartEntity CreateIdentitiesAndEmptyRecord()
        {
            DeferredIdentity = new DeferredIdentityObject
            {
                InterviewID = Interview.ID,
                SurveySID = SurveyId
            };

            Identity = new IdentityObject
            {
                CompanyID = BackendInstance.Current.CompanyId,
                InterviewerID = PersonId,
                MonitoringSessionID = DateTime.UtcNow.Ticks,
                DeferredIdentity = DeferredIdentity
            };

            var entity = PersonDeferredMonitoringRepository.InsertEmptyDeferredRecord(
                PersonId,
                SurveyId,
                Interview.ID,
                callId: null,
                callCenterId: 0,
                respondentName: Interview.RespondentName,
                telephoneNumber: Interview.TelephoneNumber);

            DeferredIdentity.DeferredRecordId = entity.ID;

            return entity;
        }        

        protected virtual BvPersonDeferredMonitoringEntity GetActiveDeferredMonitoringRecord(int recordId)
        {
            var entity = GetPersonDeferredMonitoringEntityByIdWithCheck(recordId);

            Assert.IsFalse(entity.IsComplete, "IsComplete is expected to be 'false'");
            Assert.IsTrue(entity.IsRecording, "IsRecording is expected to be 'true'");
            Assert.AreEqual(Identity.InterviewerID, entity.PersonSID, "InterviewerID is not valid");

            return entity;
        }

        protected virtual BvPersonDeferredMonitoringEntity GetPersonDeferredMonitoringEntityByIdWithCheck(int id)
        {
            var entity = GetPersonDeferredMonitoringEntityById(id);

            Assert.IsNotNull(entity, "PersonDeferredMonitoringEntity was not found.");

            return entity;
        }

        protected virtual BvPersonDeferredMonitoringEntity GetPersonDeferredMonitoringEntityById(int id)
        {
            var entity = BvPersonDeferredMonitoringAdapter.GetByCondition(
                "ID=@ID",
                new SqlParameter("ID", id)).SingleOrDefault();

            return entity;
        }

        protected virtual BvPersonDeferredMonitoringEntity CreateRecordInDatabase(params Action<BvPersonDeferredMonitoringEntity>[] actions)
        {
            return CreateRecordInDatabase(out _, actions);
        }

        protected virtual BvPersonDeferredMonitoringEntity CreateRecordInDatabase(out BvPersonDeferredMonitoringEntity entity, params Action<BvPersonDeferredMonitoringEntity>[] actions)
        {
            var pe = CreatePartRecordInDatabase(out entity, actions);
            return (GetPersonDeferredMonitoringEntityByIdWithCheck(pe.ID));
        }

        protected virtual BvPersonDeferredMonitoringPartEntity CreatePartRecordInDatabase(params Action<BvPersonDeferredMonitoringEntity>[] actions)
        {
            return CreatePartRecordInDatabase(out _, actions);
        }

        protected virtual BvPersonDeferredMonitoringPartEntity CreatePartRecordInDatabase(out BvPersonDeferredMonitoringEntity entity, params Action<BvPersonDeferredMonitoringEntity>[] actions)
        {
            entity = new BvPersonDeferredMonitoringEntity
            {
                PersonSID = PersonId,
                InterviewID = Interview.ID,
                SurveySID = SurveyId,
                TimeStamp = new DateTime(2010, 1, 2, 3, 4, 5),
                HasAudio = true,
                EventsFile = new byte[0],
                StartingFile = null,
                IsRecording = true,
                IsComplete = false,
                ClientTimeUtc = new DateTime(2010, 1, 2, 3, 4, 6),
                ServerTimeUtc = new DateTime(2010, 1, 2, 3, 4, 6),
                RecordCreationTime = new DateTime(2010, 1, 2, 3, 4, 5),
            };

            foreach (var action in actions)
            {
                action?.Invoke(entity);
            }

            var pe = BvPersonDeferredMonitoringAdapterEx.Insert(entity);

            return pe;
        }
    }
}
