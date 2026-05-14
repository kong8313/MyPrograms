using System;
using System.Globalization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptIsCallExpiredFunction : BaseMockedIntegrationTest
    {
        private readonly ISurveyStateService _surveyStateService;

        public ScriptIsCallExpiredFunction()
        {
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        private void Test_MoveAndReschedule_Base(DateTime schedulingTime, DateTime expiredTime, bool schedulingAfterInterviewing, bool result)
        {
            const int initIts = 16;
            const int trueIts = 31;
            const int falseIts = 32;
            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.SetNewITS, falseIts.ToString(CultureInfo.InvariantCulture), "IsCallExpired() == false"),
                    new Action(Action.Operation.SetNewITS, trueIts.ToString(CultureInfo.InvariantCulture), "IsCallExpired() == true")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));

            var surveyId = BackendToolsObject.CreateSurvey(script);
            
            var interview = BackendTools.NewInterview(surveyId);
            interview.TransientState = initIts;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.TimeToExpire = expiredTime;
            BackendTools.CreateCall(call);

            if (schedulingAfterInterviewing )
            {
                var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic);
                BackendTools.AssignCatiPersonToSurvey(surveyId, personId);
                _surveyStateService.Open(surveyId);
                BackendTools.LoginPerson(personId, "");

                TaskService.LookupByPersonSid(personId, 0);
            }

            BackendTools.FireEvent(interview, schedulingTime);

            interview.TransientState = result ? trueIts : falseIts;
            
            BackendTools.CheckInterview(interview);
        }

        void Test_ExpiredThread_Base(DateTime schedulingTime, DateTime expiredTime, bool result)
        {
            const int initIts = 16;
            const int trueIts = 31;
            const int falseIts = 32;
            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.SetNewITS, falseIts.ToString(CultureInfo.InvariantCulture), "IsCallExpired() == false"),
                    new Action(Action.Operation.SetNewITS, trueIts.ToString(CultureInfo.InvariantCulture), "IsCallExpired() == true")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"));

            var surveyId = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveyId);
            interview.TransientState = initIts;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.TimeToExpire = expiredTime;
            BackendTools.CreateCall(call);

            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.Expired, EventTime = schedulingTime });

            interview.TransientState = result ? trueIts : falseIts;

            BackendTools.CheckInterview(interview);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallIsExpired_MoveAndReschedule_ResultFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_MoveAndReschedule_Base(
                DateTime.Parse("2011-03-31T12:00:00"), 
                DateTime.Parse("2011-03-30T12:00:00"), 
                false, 
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallIsExpired_ExpiredThread_ResultTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_ExpiredThread_Base(
                DateTime.Parse("2011-03-31T12:00:00"),
                DateTime.Parse("2011-03-30T12:00:00"),
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallIsNotExpired_ExpiredThread_ResultTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_ExpiredThread_Base(
                DateTime.Parse("2011-03-30T12:00:00"),
                DateTime.Parse("2011-03-31T12:00:00"),
                true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallIsnotExpired_MoveAndReschedule_ResultFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_MoveAndReschedule_Base(
                DateTime.Parse("2011-03-31T12:00:00"),
                DateTime.Parse("2011-04-01T12:00:00"),
                false,
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallIsExpired_InterviewingInProgress_ResultFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_MoveAndReschedule_Base(
                DateTime.Parse("2011-03-31T12:00:00"), 
                DateTime.Parse("2011-03-30T12:00:00"), 
                true, 
                false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallIsnotExpired_InterviewingInProgress_ResultFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_MoveAndReschedule_Base(
                DateTime.Parse("2011-03-31T12:00:00"),
                DateTime.Parse("2011-04-01T12:00:00"),
                true,
                false);
        }
    }
}
