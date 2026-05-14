using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class OrderCallsDeliveringTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void OrderCallsDelivering_FullFillCache_OnlyRightCallsAreInCache()
        {
            
            var defaultTimeToCall = new DateTime(2000, 03, 20, 22, 22, 22); 
            /*
            var context = new TestData(){
                Surveys = new[]{
                    new Survey(){ Tag="S1", State=SurveyState.Open,
                        Interviews = new []{
                            new Interview(20){Tag="S1.IG1", Call = new Call(){TimeToCall = defaultTimeToCall}},
                            new Interview(){Tag="S1.IG2(priority)", Call = new Call(){TimeToCall = defaultTimeToCall.AddHours(10), Priority=10,  Assign="U1"}},
                            new Interview(){Tag="S1.IG3(early)", Size=1, Call = new Call(){TimeToCall = defaultTimeToCall.AddHours(-10), Assign="U1"}},
                        }
                    },
                    new Survey(){ Tag="S2", State=SurveyState.Open,
                        Interviews = new []{
                            new Interview(20){Tag="S2.IG1", Call = new Call(){TimeToCall = defaultTimeToCall}},
                            new Interview(){Tag="S2.IG2(priority)", Call = new Call(){TimeToCall = defaultTimeToCall.AddHours(10), Priority=10,  Assign="U1"}},
                            new Interview(){Tag="S2.IG3(early)", Size=1, Call = new Call(){TimeToCall = defaultTimeToCall.AddHours(-10), Assign="U1"}},
                        }
                    }
                },
                Persons = new [] {
                    new Person(){Tag="U1", Mode=AgentTaskChoiceMode.Automatic}
                }
            }.Create();
            */
            
            var surveyId1 = BackendToolsObject.CreateSurvey("p0123123");
            _surveyStateService.Open(surveyId1);
            var surveyId2 = BackendToolsObject.CreateSurvey("p0123124");
            _surveyStateService.Open(surveyId2);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic);

            var interviews1 = Tools.CreateInterviewWithCalls(surveyId1, Tools.AmountOfCallsPerGroup, defaultTimeToCall).ToArray();
            var interviews2 = Tools.CreateInterviewWithCalls(surveyId2, Tools.AmountOfCallsPerGroup, defaultTimeToCall).ToArray();

            var priorityInterview1 = Tools.CreateInterviewWithCall(surveyId1, 10, defaultTimeToCall.AddHours(10));
            var priorityInterview2 = Tools.CreateInterviewWithCall(surveyId2, 10, defaultTimeToCall.AddHours(10));

            var earlyInterview1 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall.AddHours(-10));
            var earlyInterview2 = Tools.CreateInterviewWithCall(surveyId2, 1, defaultTimeToCall.AddHours(-10));

            Tools.AssignPersonToInterviews(
                surveyId1, 
                personId,
                interviews1.Union(new[] { priorityInterview1, earlyInterview1 }).Select(x => x.ID));

            Tools.AssignPersonToInterviews(
                surveyId2,
                personId,
                interviews2.Union(new[] { priorityInterview2, earlyInterview2 }).Select(x => x.ID));
            /*

            BackendTools.LoginPerson(context.GetPerson("U1").Id, "");

            var mandatoryInterviews = context.GetInterviews("S1.IG2(priority)", "S1.IG2(priority)", "S1.IG3(early)", "S2.IG3(early)");

            TestAssert.AreEqual(
                mandatoryInterviews.Concat(context.GetInterviews("S1.IG1", "S2.IG1").OrderBy(x => x.ID).ThenByDescending(y => y.SurveySID)).
                Select(x => new BvTasksEntity { SurveySID = x.SurveySID, InterviewID = x.ID }),
                Tools.GetAllAccessibleTasks(context.GetPerson("U1").Id),
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
             */
            
            BackendTools.LoginPerson(personId, "");

            var mandatoryInterviews = new[] { priorityInterview2, priorityInterview1}
                .OrderBy(px => px.ID).ThenByDescending(py => py.SurveySID).Concat(
                new[] { earlyInterview2, earlyInterview1 }.OrderBy(x => x.ID).ThenByDescending(y => y.SurveySID)
            ).ToArray();
            ;

            var expected = mandatoryInterviews.Concat(interviews1.Union(interviews2).OrderBy(x => x.ID).ThenByDescending(y => y.SurveySID)).
                Select(x => new BvTasksEntity { SurveySID = x.SurveySID, InterviewID = x.ID }).ToArray();
            var actual = Tools.GetAllAccessibleTasks(personId).ToArray();

            Trace.TraceInformation("Expected interviews: {0}", 
                expected.Select(x => String.Format("({0}:{1})", x.SurveySID, x.InterviewID)).JoinInString(","));
            Trace.TraceInformation("Actual interviews: {0}",
                actual.Select(x => String.Format("({0}:{1})", x.SurveySID, x.InterviewID)).JoinInString(","));

            TestAssert.AreEqual(
                expected,
                actual,
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
           
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutoMode_LookUpOrder_CallsAreDeliveredInRightOrder()
        {
            var defaultTimeToCall = new DateTime(2000, 03, 20, 22, 22, 22);

            var surveyId1 = BackendToolsObject.CreateSurvey("p0123123");
            _surveyStateService.Open(surveyId1);
            var surveyId2 = BackendToolsObject.CreateSurvey("p0123124");
            _surveyStateService.Open(surveyId2);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var priorityInterview1 = Tools.CreateInterviewWithCall(surveyId2, 10, defaultTimeToCall.AddHours(10)); //should be third (priority - timetocall)
            var priorityInterview2 = Tools.CreateInterviewWithCall(surveyId1, 10, defaultTimeToCall); //should be second (priority)
            var priorityInterview3 = Tools.CreateInterviewWithCall(surveyId1, 10, defaultTimeToCall); //should be first (priority + assignment)

            BackendTools.AssignResourceToInterview(surveyId1, priorityInterview3.ID, personId);

            var earlyInterview1 = Tools.CreateInterviewWithCall(surveyId1, 5, defaultTimeToCall.AddHours(-20)); //should be forth (timetocall)
            var earlyInterview2 = Tools.CreateInterviewWithCall(surveyId2, 5, defaultTimeToCall.AddHours(-10)); //should be fifth (assignment)
            var earlyInterview3 = Tools.CreateInterviewWithCall(surveyId2, 5, defaultTimeToCall.AddHours(-10)); //should be six

            BackendTools.AssignResourceToInterview(surveyId2, earlyInterview2.ID, personId);

            var orderInterview1 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall); //should be eight
            var orderInterview2 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall); //should be ten
            var orderInterview3 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall); //should be seven (assignment)

            BackendTools.AssignResourceToInterview(surveyId1, orderInterview3.ID, personId);

            BackendTools.LoginPerson(personId, "");

            TestAssert.AreEqual(
                new[]{priorityInterview3, priorityInterview2, priorityInterview1,
                      earlyInterview1, earlyInterview2, earlyInterview3,
                      orderInterview3, orderInterview1, orderInterview2}.
                    Select(x => new BvTasksEntity { SurveySID = x.SurveySID, InterviewID = x.ID }),
                Tools.GetAllAccessibleTasks(personId),
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyAssignment_LookUpOrder_CallsAreDeliveredInRightOrder()
        {
            var defaultTimeToCall = new DateTime(2000, 03, 20, 22, 22, 22);

            var surveyId = BackendToolsObject.CreateSurvey("p0123123");
            _surveyStateService.Open(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.CampaignAssignment);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            var priorityInterview1 = Tools.CreateInterviewWithCall(surveyId, 10, defaultTimeToCall.AddHours(10)); //should be third (priority - timetocall)
            var priorityInterview2 = Tools.CreateInterviewWithCall(surveyId, 10, defaultTimeToCall); //should be second (priority)
            var priorityInterview3 = Tools.CreateInterviewWithCall(surveyId, 10, defaultTimeToCall); //should be first (priority + assignment)

            BackendTools.AssignResourceToInterview(surveyId, priorityInterview3.ID, personId);

            var earlyInterview1 = Tools.CreateInterviewWithCall(surveyId, 5, defaultTimeToCall.AddHours(-20)); //should be forth (timetocall)
            var earlyInterview2 = Tools.CreateInterviewWithCall(surveyId, 5, defaultTimeToCall.AddHours(-10)); //should be fifth (assignment)
            var earlyInterview3 = Tools.CreateInterviewWithCall(surveyId, 5, defaultTimeToCall.AddHours(-10)); //should be six

            BackendTools.AssignResourceToInterview(surveyId, earlyInterview2.ID, personId);

            var orderInterview1 = Tools.CreateInterviewWithCall(surveyId, 1, defaultTimeToCall); //should be eight
            var orderInterview2 = Tools.CreateInterviewWithCall(surveyId, 1, defaultTimeToCall); //should be ten
            var orderInterview3 = Tools.CreateInterviewWithCall(surveyId, 1, defaultTimeToCall); //should be seven (assignment)

            BackendTools.AssignResourceToInterview(surveyId, orderInterview3.ID, personId);

            BackendTools.LoginPerson(personId, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);

             TestAssert.AreEqual(
                new[]{priorityInterview3, priorityInterview2, priorityInterview1,
                      earlyInterview1, earlyInterview2, earlyInterview3,
                      orderInterview3, orderInterview1, orderInterview2}.
                    Select(x => new BvTasksEntity { SurveySID = x.SurveySID, InterviewID = x.ID }),
                Tools.GetAllAccessibleTasks(personId),
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
        }
    }
}
