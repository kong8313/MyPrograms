using System.Collections.Generic;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class CallCenterCallDeliveryTests : BaseMockedIntegrationTest
    {
       
        public class TestData
        {
            public const string User = "user";
            public const string Password = "pwd";
            public const string ProjectId = "p0000001";

            public BvCallCenterEntity CallCenter;
            public int SurveyId;
            public int PersonId;
            public AgentTaskChoiceMode PersonMode;

            public List<BvInterviewEntity> Interviews;
            public BvInterviewEntity SurveyInterview;
            public BvInterviewEntity PrioritySurveyInterview;
            public BvInterviewEntity PersonInterview;
            public BvInterviewEntity GroupInterview;
            
            public static TestData Create(AgentTaskChoiceMode personMode)
            {
                var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
                var callCenterService = ServiceLocator.Resolve<ICallCenterService>();
                var backendTools = new BackendTools(IntegrationTestingFramework.Instance);
                
                var callCenter = new BvCallCenterEntity()
                                    {
                                        Name = "CC1",
                                        LocalTimezoneId = 1,
                                        Description = "CCD1",
                                    };

                callCenterRepository.Insert(callCenter);

                var surveyId = backendTools.CreateSurvey(ProjectId);
                
                callCenterService.AssignSurvey(callCenter.ID, surveyId);

                var personId = PersonTools.CreatePerson(User, Password, personMode, callCenter.ID);

                List<BvInterviewEntity> interviews;
                List<BvCallEntity> calls;
                BackendTools.CreateInterviewsWithCalls(surveyId, 4, out interviews, out calls);

                CallTools.ChangeCallsPriority(surveyId, new[] { interviews[1].ID }, CallStates.Scheduled, 10);
                CallTools.AssignCalls(surveyId, new [] {interviews[2].ID }, personId);
                CallTools.AssignCalls(surveyId, new[] { interviews[3].ID }, PersonGroupService.RootGroupId);

                var _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
                _surveyStateService.Open(surveyId);

                return new TestData()
                       {
                           CallCenter = callCenter,
                           PersonId = personId,
                           PersonMode = personMode,
                           SurveyId = surveyId,
                           Interviews = interviews,
                           SurveyInterview = interviews[0],
                           PrioritySurveyInterview = interviews[1],
                           PersonInterview = interviews[2],
                           GroupInterview = interviews[3]
                       };

            }
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonNotLoggedInInSAMode_DeassignSurvey_SurveyAndPersonSpecificCallsAreDeassigned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            var personCall = CallQueueService.GetCallAndNoLock(data.SurveyId, data.PersonInterview.ID);
            var groupCall = CallQueueService.GetCallAndNoLock(data.SurveyId, data.GroupInterview.ID);

            var result = ServiceLocator.Resolve<ICallCenterService>().DeassignSurvey(data.CallCenter.ID, data.SurveyId);

            Assert.IsTrue(result);

            personCall.Resource = data.SurveyId;

            BackendTools.CheckCall(personCall);
            BackendTools.CheckCall(groupCall);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonLoggedInInSAMode_DeassignSurvey_SurveyAndPersonSpecificCallsAreDeassigned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            var personCall = CallQueueService.GetCallAndNoLock(data.SurveyId, data.PersonInterview.ID);
            var groupCall = CallQueueService.GetCallAndNoLock(data.SurveyId, data.GroupInterview.ID);
            AssignmentService.AssignResourceToSurvey(data.SurveyId, data.PersonId, data.CallCenter.ID);

            LoginPerson(data);
            
            var result = ServiceLocator.Resolve<ICallCenterService>().DeassignSurvey(data.CallCenter.ID, data.SurveyId);

            Assert.IsFalse(result);

            BackendTools.CheckCall(personCall);
            BackendTools.CheckCall(groupCall);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOnCallsInSAMode_GetOpenedSurvey_SurveyAreReturned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            var survey = BvSpGetOpenedSurveysAdapter.ExecuteEntityList(data.PersonId).SingleOrDefault();

            Assert.AreEqual(data.SurveyId, survey.SID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsNotAssignOnSurveyAndCallsInSAMode_GetOpenedSurvey_SurveyAreReturned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            CallTools.MoveAndRescheduleCalls(data.SurveyId, data.Interviews.Select(x => x.ID), 16);

            BackendTools.RunSchedulingProcedure();

            var survey = BvSpGetOpenedSurveysAdapter.ExecuteEntityList(data.PersonId).SingleOrDefault();

            Assert.IsNull(survey);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOSurveyDirectlyOnlyInSAMode_GetOpenedSurvey_SurveyAreReturned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            var survey = BvSpGetOpenedSurveysAdapter.ExecuteEntityList(data.PersonId).SingleOrDefault();

            AssignmentService.AssignResourceToSurvey(data.SurveyId, data.PersonId, data.CallCenter.ID);

            Assert.AreEqual(data.SurveyId, survey.SID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOSurveyThrouthGroupOnlyInSAMode_GetOpenedSurvey_SurveyAreReturned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            var survey = BvSpGetOpenedSurveysAdapter.ExecuteEntityList(data.PersonId).SingleOrDefault();

            AssignmentService.AssignResourceToSurvey(data.SurveyId, data.PersonId, PersonGroupService.RootGroupId);

            Assert.AreEqual(data.SurveyId, survey.SID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOSurveyThrouthGroupAndDirectryAndCallsInSAMode_GetOpenedSurvey_SurveyAreReturned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            var survey = BvSpGetOpenedSurveysAdapter.ExecuteEntityList(data.PersonId).SingleOrDefault();

            AssignmentService.AssignResourceToSurvey(data.SurveyId, data.PersonId, data.CallCenter.ID);

            AssignmentService.AssignResourceToSurvey(data.SurveyId, data.PersonId, PersonGroupService.RootGroupId);

            Assert.AreEqual(data.SurveyId, survey.SID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOnCallsInSAModeButDeasingFromCC_GetOpenedSurvey_SurveyAreNotReturned()
        {
            var data = TestData.Create(AgentTaskChoiceMode.CampaignAssignment);

            var result = ServiceLocator.Resolve<ICallCenterService>().DeassignSurvey(data.CallCenter.ID, data.SurveyId);
            Assert.IsTrue(result);

            var survey = BvSpGetOpenedSurveysAdapter.ExecuteEntityList(data.PersonId).SingleOrDefault();

            Assert.IsNull(survey); 
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOnCallsInAutoMode_GetCalls_CallsAreNotDelivered()
        {
            var data = TestData.Create(AgentTaskChoiceMode.Automatic);

            var interviewIds = GetCallsInAtotomatic(data);

            Assert.AreEqual(2, interviewIds.Count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOnCallsaNDSurveyInAutoMode_GetCalls_CallsAreNotDelivered()
        {
            var data = TestData.Create(AgentTaskChoiceMode.Automatic);

            AssignmentService.AssignResourceToSurvey(data.SurveyId, data.PersonId, data.CallCenter.ID);
            var interviewIds = GetCallsInAtotomatic(data);

            Assert.AreEqual(4, interviewIds.Count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonIsAssignOnCallsInAutoModeButDeasingFromCC_GetCalls_CallsAreNotDelivered()
        {
            var data = TestData.Create(AgentTaskChoiceMode.Automatic);

            var result = ServiceLocator.Resolve<ICallCenterService>().DeassignSurvey(data.CallCenter.ID, data.SurveyId);
            Assert.IsTrue(result);

            var interviewIds = GetCallsInAtotomatic(data);

            Assert.AreEqual(0, interviewIds.Count);           
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(74888)]
        public void PersonInGroup_DeasingGroupFromSurveyInOtherCC_PersonAssigmentNotDeletedAndCallDeliveryWorksCorrect()
        {
            var data = TestData.Create(AgentTaskChoiceMode.Automatic);

            var callCenter2 = CallCenterTools.Create();

            CallCenterTools.ReassignSurvey(data.SurveyId, data.CallCenter, callCenter2);

            AssignmentService.AssignResourceToSurvey(data.SurveyId, PersonGroupService.RootGroupId, data.CallCenter.ID);
            AssignmentService.AssignResourceToSurvey(data.SurveyId, PersonGroupService.RootGroupId, callCenter2.ID);

            AssignmentService.DeassignResourceFromSurvey(data.SurveyId, PersonGroupService.RootGroupId, callCenter2.ID);

            var interviewIds = GetCallsInAtotomatic(data);

            Assert.AreEqual(4, interviewIds.Count);
        }

        private List<int> GetCallsInAtotomatic(TestData data)
        {
            var deliveredInterviewIds = new List<int>();

            var test = new TestCati2(false, BackendToolsObject);

            test.InitializeWithExistsSurveyAndPerson(SurveyRepository.GetById(data.SurveyId), data.PersonId, data.Interviews);

            test.Login(TestData.User, TestData.Password, data.PersonMode, false);

            var interview = test.StartInterview_ManualOrPreview(null, 0);

            while (interview != null)
            {
                deliveredInterviewIds.Add(interview.ID);
                interview = test.CompleteInterviewAndWaitNext_Manual(interview);
            }
            return deliveredInterviewIds;
        }

        
        private void LoginPerson(TestData data)
        {
            var test = new TestCati2(false, BackendToolsObject);

            test.InitializeWithExistsSurveyAndPerson(SurveyRepository.GetById(data.SurveyId), data.PersonId, data.Interviews);

            test.Login(TestData.User, TestData.Password, data.PersonMode, false);

            var interview = test.StartInterview_ManualOrPreview(TestData.ProjectId, 0);
            Assert.AreEqual(interview.ID, data.PrioritySurveyInterview.ID);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RequestCalls_TwoCallCentersWithTwoPersonForFirstCallCenterWithAssignInSingleGroup_RequestCallByCampaignReturnsCorrentCountOfCalls()
        {
            var context = new Framework.Data.TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", DialMode = DialingMode.Predictive, IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData(1) {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData(1) {Tag = "S1.I2", Call = new CallData(){Resource = "PG1"}},
                            new InterviewData(1) {Tag = "S1.I3", Call = new CallData(){Resource = "PG2"}}
                        },
                        Assigns = new[]{"PG1", "PG2"}
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1"}, 
                    new PersonGroupData(){Tag="PG2"}
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1", Memberships="PG1", CallCenter = "CC1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2", Memberships="PG1", CallCenter = "CC1", TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                CallCenters = new[]
                {
                    new CallCenterData() {Tag = "CC1", Dialer = "D1"}, 
                    new CallCenterData() {Tag = "CC2", Dialer = "D2"}
                },
                Dialers = new[]
                {
                    new DialerData() { Tag = "D1"},
                    new DialerData() { Tag = "D2"}
                }

            }.Create();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            var console2 = new AutomaticConsoleController(context, person2, survey);
            
            console1.Login();
            console1.LoginToDialer();
            console2.Login();
            console2.LoginToDialer();

            dialer.RequestCalls(survey, 10, CallsSelectionAlgorithm.ByCampaign);

            context.GetCalls("S1.I1", "S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
            context.GetCalls("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RequestCalls_TwoCallCentersWithTwoPersonForFirstCallCenterWithAssignInTwoGroup_RequestCallByCampaignReturnsCorrentCountOfCalls()
        {
            var context = new Framework.Data.TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", DialMode = DialingMode.Predictive, IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData(1) {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData(1) {Tag = "S1.I2", Call = new CallData(){Resource = "PG1"}},
                            new InterviewData(1) {Tag = "S1.I3", Call = new CallData(){Resource = "PG2"}}
                        },
                        Assigns = new[]{"PG1", "PG2"}
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1"}, 
                    new PersonGroupData(){Tag="PG2"}
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1", Memberships="PG1", CallCenter = "CC1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2", Memberships="PG2", CallCenter = "CC1", TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                CallCenters = new[]
                {
                    new CallCenterData() {Tag = "CC1", Dialer = "D1"}, 
                    new CallCenterData() {Tag = "CC2", Dialer = "D2"}
                },
                Dialers = new[]
                {
                    new DialerData() { Tag = "D1"},
                    new DialerData() { Tag = "D2"}
                }

            }.Create();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            var console2 = new AutomaticConsoleController(context, person2, survey);

            console1.Login();
            console1.LoginToDialer();
            console2.Login();
            console2.LoginToDialer();

            dialer.RequestCalls(survey, 10, CallsSelectionAlgorithm.ByCampaign);

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RequestCalls_TwoCallCentersWithTwoPersonInBothCallCenterWithAssignInTwoGroupForDialerFromFirstCallCenter_RequestCallByCampaignReturnsCorrentCountOfCalls()
        {
            var context = new Framework.Data.TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", DialMode = DialingMode.Predictive, IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData(1) {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData(1) {Tag = "S1.I2", Call = new CallData(){Resource = "PG1"}},
                            new InterviewData(1) {Tag = "S1.I3", Call = new CallData(){Resource = "PG2"}}
                        },
                        Assigns = new[]{"PG1", "PG2"}
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1"}, 
                    new PersonGroupData(){Tag="PG2"}
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1", Memberships="PG1", CallCenter = "CC1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2", Memberships="PG2", CallCenter = "CC2", TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                CallCenters = new[]
                {
                    new CallCenterData() {Tag = "CC1", Dialer = "D1"}, 
                    new CallCenterData() {Tag = "CC2", Dialer = "D2"}
                },
                Dialers = new[]
                {
                    new DialerData() { Tag = "D1"},
                    new DialerData() { Tag = "D2"}
                }

            }.Create();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            var console2 = new AutomaticConsoleController(context, person2, survey);

            console1.Login();
            console1.LoginToDialer();
            console2.Login();
            console2.LoginToDialer();

            dialer.RequestCalls(survey, 10, CallsSelectionAlgorithm.ByCampaign);

            context.GetCalls("S1.I1", "S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
            context.GetCalls("S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RequestCalls_TwoCallCentersWithTwoPersonInBothCallCenterWithAssignInTwoGroupForDialerFromSecondCallCenter_RequestCallByCampaignReturnsCorrentCountOfCalls()
        {
            var context = new Framework.Data.TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", DialMode = DialingMode.Predictive, IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData(1) {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData(1) {Tag = "S1.I2", Call = new CallData(){Resource = "PG1"}},
                            new InterviewData(1) {Tag = "S1.I3", Call = new CallData(){Resource = "PG2"}}
                        },
                        Assigns = new[]{"PG1", "PG2"}
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1"}, 
                    new PersonGroupData(){Tag="PG2"}
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1", Memberships="PG1", CallCenter = "CC1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2", Memberships="PG2", CallCenter = "CC2", TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                CallCenters = new[]
                {
                    new CallCenterData() {Tag = "CC1", Dialer = "D1"}, 
                    new CallCenterData() {Tag = "CC2", Dialer = "D2"}
                },
                Dialers = new[]
                {
                    new DialerData() { Tag = "D1"},
                    new DialerData() { Tag = "D2"}
                }

            }.Create();

            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D2");

            var console1 = new AutomaticConsoleController(context, person1, survey);
            var console2 = new AutomaticConsoleController(context, person2, survey);

            console1.Login();
            console1.LoginToDialer();
            console2.Login();
            console2.LoginToDialer();

            dialer.RequestCalls(survey, 10, CallsSelectionAlgorithm.ByCampaign);

            context.GetCalls("S1.I1", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.LoadedToDialerPredictively);
            context.GetCalls("S1.I2").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }
    }
}
