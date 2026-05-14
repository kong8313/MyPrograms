using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.Supervisor.Core.Activity;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.InterviewerPerformance
{
    [TestClass]
    public class InterviewerPerformanceTests
    {
        const string CompletedITS = "13";
        const string AppointmentITS = "1";
        const string UserPassword = "password";
        private int _callCenterId;

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private ITimezoneService _timezoneService;
        private IActivityManager _activityManager;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);
            
            BackendTools.ResetInterviewId();
            _callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();
            _timezoneService = ServiceLocator.Resolve<ITimezoneService>();
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void InterviewerPerformanceList_InterviewerIsWorkingOnSurvey_ActiveSurveyIsSelected()
        {
            const string userName = "user";
            const string userName2 = "user2";

            // first interviewer with one survey
            var firstContext = CreateSurveyWithPersonAndCalls(userName, 3, null, null, null);

            // second interviewer with two surveys
            var secondContext = CreateSurveyWithPersonAndCalls(userName2, 3, null, null, BackendTools.GenerateSurveyName());
            var secondSurvey = CreateSurveyWithPersonAndCalls(userName2, 3, null, secondContext.PersonSID, BackendTools.GenerateSurveyName());

            firstContext.Login(userName, UserPassword, AgentTaskChoiceMode.Manual, false);
            secondContext.Login(userName2, UserPassword, AgentTaskChoiceMode.Manual, false);

            CompleteInterviewAndSendCallNotification(userName, UserPassword, firstContext.SurveyName, firstContext.PersonSID, 1, CompletedITS);

            CompleteInterviewAndSendCallNotification(userName2, UserPassword, secondContext.SurveyName, secondContext.PersonSID, 1, CompletedITS);
            CompleteInterviewAndSendCallNotification(userName2, UserPassword, secondSurvey.SurveyName, secondContext.PersonSID, 1, CompletedITS);

            firstContext.Logout();

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            var taskEntity = BvTasksAdapter.GetAll()[0];
            taskEntity.CallID = 1;
            taskEntity.SurveySID = secondSurvey.SurveySID;
            taskEntity.InterviewID = 1;

            BvTasksAdapter.Update(taskEntity);

            // act
            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);
            var result = _activityManager.GetInterviewerPerformanceData(false, true, true, _callCenterId);

            // assert 
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(result[0].ProjectId, secondSurvey.SurveyName);
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void InterviewerPerformanceList_BreakdownBySurveys_FieldsAreCalculatedProperly()
        {   
            // arrange
            const string userName = "user";

            var time = new DateTime(2016, 11, 23, 12, 0, 0);
            new DateTimeMocker(_framework).MockDate(time);
            
            var test1 = CreateSurveyWithPersonAndCalls(userName, 1, null, null, null);
            var interviewerId = test1.PersonSID;
            var test2 = CreateSurveyWithPersonAndCalls(userName, 2, null, interviewerId, BackendTools.GenerateSurveyName());

            // records for first survey, total seconds: 400 + 10 + 100 + 5 + 400 + 15 = 930
            _backendTools.CreateHistoryRecords(test1.SurveySID, interviewerId,
                new[] { time.AddHours(-3) }, 1, 400, 10);     
            _backendTools.CreateHistoryRecords(test1.SurveySID, interviewerId,
                new[] { time.AddHours(-2) }, 2, 100, 5);   
            _backendTools.CreateHistoryRecords(test1.SurveySID, interviewerId,
                new[] { time.AddMinutes(-40) }, 3, 400, 15);

            // records for second survey, total seconds: 160 + 5 + 200 + 5 = 370 
            _backendTools.CreateHistoryRecords(test2.SurveySID, interviewerId,
                new[] {time.AddHours(-3)}, 1, 160, 5); 
            _backendTools.CreateHistoryRecords(test2.SurveySID, interviewerId,
                new[] {time.AddHours(-2)}, 2, 200, 5);

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            // act
            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);
            var byInterviewer = _activityManager.GetInterviewerPerformanceData(false, false, false, _callCenterId);
            var bySurvey = _activityManager.GetInterviewerPerformanceData(false, true, false, _callCenterId);

            // assert   
            Assert.AreEqual(2, bySurvey.Count);

            Assert.AreEqual(bySurvey[0].ProjectId, test1.SurveyName);
            Assert.AreEqual(bySurvey[0].InterviewingTime.TotalSeconds, 930);
            Assert.AreEqual(bySurvey[0].CompletedInterviewCount, 3);
            Assert.AreEqual(bySurvey[0].CompletedInLastHourCount, 1);
            Assert.AreEqual(bySurvey[0].TotalInterviewCount, 3);
            Assert.AreEqual(bySurvey[0].StrikeRateAverage,
                (float)Math.Round(bySurvey[0].CompletedInterviewCount / ((float)bySurvey[0].InterviewingTime.TotalSeconds / 3600), 2));

            Assert.AreEqual(bySurvey[1].ProjectId, test2.SurveyName);
            Assert.AreEqual(bySurvey[1].InterviewingTime.TotalSeconds, 370);
            Assert.AreEqual(bySurvey[1].CompletedInterviewCount, 2);
            Assert.AreEqual(bySurvey[1].CompletedInLastHourCount, 0);
            Assert.AreEqual(bySurvey[1].TotalInterviewCount, 2);
            Assert.AreEqual(bySurvey[1].StrikeRateAverage,
                    (float)Math.Round(bySurvey[1].CompletedInterviewCount / ((float)bySurvey[1].InterviewingTime.TotalSeconds / 3600), 2));

            //   result by interviewer is equal the sum of results by survey
            Assert.AreEqual(1, byInterviewer.Count);

            Assert.AreEqual(byInterviewer[0].InterviewingTime,
                bySurvey[0].InterviewingTime + bySurvey[1].InterviewingTime);

            Assert.AreEqual(byInterviewer[0].CompletedInLastHourCount,
                bySurvey[0].CompletedInLastHourCount + bySurvey[1].CompletedInLastHourCount);

            Assert.AreEqual(byInterviewer[0].TotalInterviewCount,
                bySurvey[0].TotalInterviewCount + bySurvey[1].TotalInterviewCount);

            Assert.AreEqual(byInterviewer[0].CompletedInterviewCount,
                bySurvey[0].CompletedInterviewCount + bySurvey[1].CompletedInterviewCount);

            Assert.AreEqual(byInterviewer[0].StrikeRateAverage,
                (float)Math.Round(byInterviewer[0].CompletedInterviewCount / ((float)byInterviewer[0].InterviewingTime.TotalSeconds / 3600), 2));
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void InterviewerPerformanceList_TwoInterviewers_ActivityForSelectedInterviewer()
        {
            // arrange
            const string userName = "user";
            const string userName2 = "user2";

            var test1 = CreateSurveyWithPersonAndCalls(userName, 1, null, null, null);
            var test2 = CreateSurveyWithPersonAndCalls(userName2, 1, null, null, BackendTools.GenerateSurveyName());

            test1.Login(userName, UserPassword, AgentTaskChoiceMode.Manual, false); 
            test2.Login(userName2, UserPassword, AgentTaskChoiceMode.Manual, false);

            CompleteInterviewAndSendCallNotification(userName, UserPassword, test1.SurveyName, test1.PersonSID, 1, CompletedITS);
            CompleteInterviewAndSendCallNotification(userName2, UserPassword, test2.SurveyName, test2.PersonSID, 1, CompletedITS);

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);

            // act
            var result = _activityManager.GetInterviewerPerformanceData(false, false, false, _callCenterId, new[] { test2.PersonSID });

            // assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(userName2, result[0].InterviewerName);
            Assert.AreEqual(test2.PersonSID, result[0].InterviewerId); 
            Assert.AreEqual(1, result[0].TotalInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInLastHourCount);
        }   
        
        [TestMethod, Owner(@"FIRM\VictorR")]
        public void InterviewerPerformanceList_CompletedSurveys_ActivityForSelectedSurveys()
        {
            // arrange
            const string userName = "user";
            var test1 = CreateSurveyWithPersonAndCalls(userName, 1, null, null, null);
            var interviewerId = test1.PersonSID;
            var test2 = CreateSurveyWithPersonAndCalls(userName, 2, null, interviewerId, BackendTools.GenerateSurveyName());
            var test3 = CreateSurveyWithPersonAndCalls(userName, 2, null, interviewerId, BackendTools.GenerateSurveyName());
            var test4 = CreateSurveyWithPersonAndCalls(userName, 2, null, interviewerId, BackendTools.GenerateSurveyName());

            test1.Login(userName, UserPassword, AgentTaskChoiceMode.Manual, false);

            CompleteInterviewAndSendCallNotification(userName, UserPassword, test1.SurveyName, interviewerId, 1, CompletedITS);
            CompleteInterviewAndSendCallNotification(userName, UserPassword, test2.SurveyName, interviewerId, 1, CompletedITS);
            CompleteInterviewAndSendCallNotification(userName, UserPassword, test3.SurveyName, interviewerId, 1, CompletedITS);
            CompleteInterviewAndSendCallNotification(userName, UserPassword, test4.SurveyName, interviewerId, 1, CompletedITS);

            // act
            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(GetStartDateOfCallCenterUtcTime(), CompletedITS);
            var result = _activityManager.GetInterviewerPerformanceData(false, true, false, _callCenterId, null, new[] { test2.SurveySID, test4.SurveySID  });
           
            // asssert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(test2.SurveyName, result[0].ProjectId);
            Assert.AreEqual(test4.SurveyName, result[1].ProjectId);
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void InterviewerPerformanceList_InterviewerCompleteTwoSurvey_ActivitySeparatedBySurveys()
        {
            // arrange
            const string userName = "user";
            var test1 = CreateSurveyWithPersonAndCalls(userName, 1, null, null, null);
            var interviewerId = test1.PersonSID;
            var test2 = CreateSurveyWithPersonAndCalls(userName, 2, null, interviewerId, BackendTools.GenerateSurveyName());
            
            test1.Login(userName, UserPassword, AgentTaskChoiceMode.Manual, false);

            foreach(var interview in test1.Interviews)
                CompleteInterviewAndSendCallNotification(userName, UserPassword, test1.SurveyName, interviewerId, interview.ID, CompletedITS);
          
            foreach (var interview in test2.Interviews)
                CompleteInterviewAndSendCallNotification(userName, UserPassword, test2.SurveyName, interviewerId, interview.ID, CompletedITS);

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            // act
            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);
            var result = _activityManager.GetInterviewerPerformanceData(false, true, false, _callCenterId);

            // assert
            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].TotalInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInLastHourCount);
            Assert.AreEqual(test1.SurveyName, result[0].ProjectId);
            Assert.AreEqual(interviewerId, result[0].InterviewerId); 
   
            Assert.AreEqual(2, result[1].TotalInterviewCount);
            Assert.AreEqual(2, result[1].CompletedInterviewCount);
            Assert.AreEqual(2, result[1].CompletedInLastHourCount);
            Assert.AreEqual(test2.SurveyName, result[1].ProjectId);
            Assert.AreEqual(interviewerId, result[1].InterviewerId); 
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void OneInterviewerCompletesOneInterviewAndMakesAppointmentForSecond_CorrectAggregatedData()
        {
            const string userName = "userName";

            var time = new DateTime(2016, 11, 23, 12, 0, 0);
            new DateTimeMocker(_framework).MockDate(time);

            var test = CreateSurveyWithPersonAndCalls(userName, 2, null, null, null);

            test.Login(userName, UserPassword, AgentTaskChoiceMode.Manual, false);

            CompleteInterviewAndSendCallNotification(userName, UserPassword, test.SurveyName, test.PersonSID, 1, CompletedITS);
            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);
            var result = _activityManager.GetInterviewerPerformanceData(false, false, false, _callCenterId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].TotalInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInLastHourCount);

            CompleteInterviewAndSendCallNotification(userName, UserPassword, test.SurveyName, test.PersonSID, 2, AppointmentITS);

            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);

            result = _activityManager.GetInterviewerPerformanceData(false, false, false, _callCenterId);

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(userName, result[0].InterviewerName);
            Assert.AreEqual(2, result[0].TotalInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInLastHourCount);            
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void OneInterviewerCompletesOneInterviewAndMakesLogout_CorrectAggregatedData()
        {
            const string userName = "userName";

            var test = CreateSurveyWithPersonAndCalls(userName, 2, null, null, null);
            test.Login(userName, UserPassword, AgentTaskChoiceMode.Manual, false);

            CompleteInterviewAndSendCallNotification(userName, UserPassword, test.SurveyName, test.PersonSID, 1, CompletedITS);

            test.Logout(false);

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);

            Assert.AreEqual(1, _activityManager.GetInterviewerPerformanceData(false, false, false, _callCenterId).Count);
            Assert.AreEqual(0, _activityManager.GetInterviewerPerformanceData(true, false, false, _callCenterId).Count);                        
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void ThreeInterviewersInSystem_TwoWorkAndCompleteByOneInterview_CorrectAggregatedData()
        {
            const string userName1 = "user1";
            const string userName2 = "user2";
            const string userName3 = "user3";

            var test1 = CreateSurveyWithPersonAndCalls(userName1, 1, null, null, null);
            var test2 = CreateSurveyWithPersonAndCalls(userName2, 1, null, null, BackendTools.GenerateSurveyName());
            CreateSurveyWithPersonAndCalls(userName3, 1, null, null, BackendTools.GenerateSurveyName());

            test1.Login(userName1, UserPassword, AgentTaskChoiceMode.Manual, false);
            test2.Login(userName2, UserPassword, AgentTaskChoiceMode.Manual, false);
            
            CompleteInterviewAndSendCallNotification(userName1, UserPassword, test1.SurveyName, test1.PersonSID, 1, CompletedITS);
            CompleteInterviewAndSendCallNotification(userName2, UserPassword, test2.SurveyName, test2.PersonSID, 1, CompletedITS);

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);

            var result = _activityManager.GetInterviewerPerformanceData(false, false, false, _callCenterId);

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].TotalInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInLastHourCount);

            Assert.AreEqual(1, result[1].TotalInterviewCount);
            Assert.AreEqual(1, result[1].CompletedInterviewCount);
            Assert.AreEqual(1, result[1].CompletedInLastHourCount);                        
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void OneInterviewerCompletesTwoCallsOnlyOneInLastHour_CorrectAggregatedData()
        {
            const string userName = "userName";

            var test = CreateSurveyWithPersonAndCalls(userName, 2, null, null, null);

            test.Login(userName, UserPassword, AgentTaskChoiceMode.Manual, false);

            new DateTimeMocker(_framework).MockDate(DateTime.Parse("2016.10.10T14:00:00"));
        
            CompleteInterviewAndSendCallNotification(userName, UserPassword, test.SurveyName, test.PersonSID, 1, CompletedITS);

            new DateTimeMocker(_framework).MockDate(DateTime.Parse("2016.10.10T16:00:00"));

            CompleteInterviewAndSendCallNotification(userName, UserPassword, test.SurveyName, test.PersonSID, 2, CompletedITS);

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();
            var fixedDateTime = new DateTime(2016, 10, 10, 16, 1, 0);
            new DateTimeMocker(fixedDateTime);

            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);

            var result = _activityManager.GetInterviewerPerformanceData(false, false, false, _callCenterId);

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(2, result[0].TotalInterviewCount);
            Assert.AreEqual(2, result[0].CompletedInterviewCount);
            Assert.AreEqual(1, result[0].CompletedInLastHourCount);            
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void InterviewerPerformanceList_TwoInterviewersInDifferentCallCenters_ProperCallCenterInfoIsReturned()
        {
            const string defaultVccUser = "default";
            const string secondVccUser = "second";

            var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            var secondCallCenter = new BvCallCenterEntity {Name = "Second", LocalTimezoneId = 1};
            callCenterRepository.Insert(secondCallCenter);

            var testDefault = CreateSurveyWithPersonAndCalls(defaultVccUser, 1, null, null, null);
            var testSecond = CreateSurveyWithPersonAndCalls(secondVccUser, 1, secondCallCenter.ID, null, BackendTools.GenerateSurveyName());

            testDefault.Login(defaultVccUser, UserPassword, AgentTaskChoiceMode.Manual, false);
            testSecond.Login(secondVccUser, UserPassword, AgentTaskChoiceMode.Manual, false);

            CompleteInterviewAndSendCallNotification(defaultVccUser, UserPassword, testDefault.SurveyName, testDefault.PersonSID, 1, CompletedITS);
            CompleteInterviewAndSendCallNotification(secondVccUser, UserPassword, testSecond.SurveyName, testSecond.PersonSID, 1, CompletedITS);

            var startDateOfCallCenterUtcTime = GetStartDateOfCallCenterUtcTime();

            BvSpAggregateInterviewerPerformanceAdapter.ExecuteNonQuery(startDateOfCallCenterUtcTime, CompletedITS);

            var result = _activityManager.GetInterviewerPerformanceData(false, false, false, secondCallCenter.ID);

            Assert.AreEqual(1, result.Count, "Single record for one call center should be returned");
            Assert.AreEqual(testSecond.PersonSID, result[0].InterviewerId, "Wrong interviewer info");
        }

        private TestCati2 CreateSurveyWithPersonAndCalls(string userName, int callsCount, int? callCenterId, int? personId, string testSurveyName)
        {
            var test = new TestCati2(false, false, false, _backendTools, new TestDialer(), testSurveyName);
            test.CreateSurveyWithPerson(DialingMode.Manual, userName, UserPassword, AgentTaskChoiceMode.Manual, callCenterId, personId);
            test.CreateInterviewsWithCalls(callsCount);

            return test;
        }

        private void CompleteInterviewAndSendCallNotification(string userName, string password, string projectId, int interviewerId, int interviewId, string its)
        {            
            var consoleHelper = new CatiWsHelper(userName, password);
            consoleHelper.ConsoleService.StartInterview(projectId, interviewId);
            TestCati2.WaitInterviewState(consoleHelper, InterviewState.INTERVIEWING);
            consoleHelper.ConsoleService.WrapUp(interviewId, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = its });
        }

        private DateTime GetStartDateOfCallCenterUtcTime()
        {
            var defaultTimezoneId = _timezoneService.GetDefaultCallCenterTimezoneId();
            var startDateOfCallCenterLocalTime = _timezoneService.ConvertTimeFromUtc(defaultTimezoneId, ServiceLocator.Resolve<ITimeService>().GetUtcNow()).Date;
            var startDateOfCallCenterUtcTime = _timezoneService.ConvertTimeToUtc(defaultTimezoneId, startDateOfCallCenterLocalTime);

            return startDateOfCallCenterUtcTime;
        }
    }
}
