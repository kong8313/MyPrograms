using System;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.FCDSpecificTests
{
    [TestClass]
    public class FCDTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private ISurveyStateService _surveyStateService;
        private IInterviewRepository _interviewRepository;
        private IPersonRepository _personRepository;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
            _personRepository = ServiceLocator.Resolve<IPersonRepository>();

            _backendTools.LaunchAllHoursScript();

            _surveySid = _backendTools.CreateSurvey(ProjectId);
            _surveyStateService.Open(_surveySid);

            var person = new BvPersonEntity { Name = "interviewer", CallCenterID = CallCenterTools.DefaultId };
            _personSid = _personRepository.Insert(person);

            BackendTools.AssignCatiPersonToSurvey(_surveySid, _personSid);

            _quota = TestQuota.Create(_framework.DbEngine,
                _surveySid,
                1,
                new[] { "q1" },
                new[] { 2 });

            _interviewRepository.InsertOnly(new BvInterviewEntity
            {
                ID = 1,
                SurveySID = _surveySid,
                TransientState = 16
            });

            _interviewRepository.InsertOnly(new BvInterviewEntity
            {
                ID = 2,
                SurveySID = _surveySid,
                TransientState = 16
            });

            const int cellId1 = 1;
            _cellId2 = 2;

            _quota.PutInterviewsInCells(
                new[] { 1, 2 },
                new[] { cellId1, _cellId2 });

            BackendTools.SyncResponseControl(_framework.DbEngine, _surveySid);

            var call1 = new BvCallEntity
            {
                SurveySID = _surveySid,
                InterviewID = 1
            };

            var call2 = new BvCallEntity
            {
                SurveySID = _surveySid,
                InterviewID = 2
            };

            CallManager.AddCall(call1);
            CallManager.AddCall(call2);
            
            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(_surveySid, (CancellationToken)default);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        const string ProjectId = "p00112233";
        private TestQuota _quota;

        private int _cellId2;
        private int _surveySid;
        private int _personSid;

        

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FCD_GetCallForPersonAfterCellClosingCell_CallShouldNotBeDelivered()
        {
            BackendTools.LoginPerson(_personSid, "");

            BvTasksEntity task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.AreEqual(1, task.CallID);

            _quota.CloseCell(_cellId2);

            Assert.IsNull(TaskService.LookupByPersonSid(_personSid, _surveySid));

            BackendTools.CheckResponseControl(_framework.DbEngine, _surveySid);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void FCD_GetCallForPersonAfterQuotaUpdate_CallShouldNotBeDelivered()
        {
            BackendTools.LoginPerson(_personSid, "");

            _quota.ConfirmitCloseCell(_cellId2);
            _quota.QuotaUpdate();

            BvTasksEntity task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.AreEqual(1, task.CallID);

            task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.IsNull(task);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Cr(56817), Ignore]
        public void FCD_MakeIgnoredByFcdInterviewAndMoveAndReschedule_InterviewIsNotFiltered()
        {
            DateTime appDate = DateTime.UtcNow.AddHours(2).CutMilliseconds();
            BackendTools.AddAppointmentAndLinkItWithCall(2, _surveySid, appDate);
            var interview = InterviewRepository.GetById(_surveySid, 2);
            interview.TransientState = 1;
            InterviewRepository.UpdateOnly(interview);

            BackendTools.LoginPerson(_personSid, "");

            _quota.ConfirmitCloseCell(_cellId2);
            _quota.QuotaUpdate();

            BvTasksEntity task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.AreEqual(1, task.CallID);

            task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.IsNull(task);

            CallTools.MoveAndRescheduleCalls(_surveySid, new[] { 2 }, 1);

            Assert.AreEqual(2, BvSvyScheduleAdapter.GetAll().First(x => x.InterviewID == 2).CallState, "CallState");
            Assert.AreEqual(CallOutcome.Appointment, (CallOutcome)BvInterviewAdapter.GetAll().First(x => x.ID == 2).TransientState, "TransientState");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(56817)]
        public void FCD_MakeIgnoredByFcdInterviewAndMoveAndRescheduleToNotIgnoredIts_InterviewIsFiltered()
        {
            DateTime appDate = DateTime.UtcNow.AddHours(2).CutMilliseconds();
            BackendTools.AddAppointmentAndLinkItWithCall(2, _surveySid, appDate);
            var interview = InterviewRepository.GetById(_surveySid, 2);
            interview.TransientState = 1;
            InterviewRepository.UpdateOnly(interview);

            BackendTools.LoginPerson(_personSid, "");

            _quota.ConfirmitCloseCell(_cellId2);
            _quota.QuotaUpdate();

            BvTasksEntity task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.AreEqual(1, task.CallID);

            task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.IsNull(task);

            CallTools.MoveAndRescheduleCalls(_surveySid, new[] { 2 }, 2);

            Assert.AreEqual(0, BvSvyScheduleAdapter.GetAll().First(x => x.InterviewID == 2).CallState, "CallState");
            Assert.AreEqual(CallOutcome.FilteredByCallDelivery, (CallOutcome)BvInterviewAdapter.GetAll().First(x => x.ID == 2).TransientState, "TransientState");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void FCD_MakeCallWithIgnoredItsCloseByFcd_CheckCallPhaseIsDelivered()
        {
            var survey = SurveyRepository.GetById(_surveySid);
            var state = StateRepository.GetById(survey.StateGroupID, 8);
            state.FcdAction = true;
            StateRepository.Update(state);

            var interview = InterviewRepository.GetById(_surveySid, 2);
            interview.TransientState = 8;
            InterviewRepository.UpdateOnly(interview);


            BackendTools.LoginPerson(_personSid, "");

            _quota.ConfirmitCloseCell(_cellId2);
            _quota.QuotaUpdate();

            BvTasksEntity task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.AreEqual(1, task.CallID);

            task = TaskService.LookupByPersonSid(_personSid, _surveySid);
            Assert.IsNotNull(task);
        }
    }
}
