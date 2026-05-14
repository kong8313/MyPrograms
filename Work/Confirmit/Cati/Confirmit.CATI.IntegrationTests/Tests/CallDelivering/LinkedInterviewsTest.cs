using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.LinkedInterviews;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class LinkedInterviewsTest : BaseMockedIntegrationTest
    {
        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void LastInterviewInAListIsLinkedInterview_NoDialer_ProcessedAsPartOfFirstInterview()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, SchedulingScript = AllHoursSchedule.Name, DialMode = DialingMode.Automatic,

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S1.I4"));

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            var linkedInterviewSessionId = TaskRepository.GetByPerson(person.Id).LinkedInterviewSessionId;
            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id } }, person.Id);
            CheckLastHistoryRecord(linkedInterviewSessionId);

            var normalInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });
            Assert.AreEqual("S1.I2", normalInterview.Tag, "Not correct interview");
            CheckLastHistoryRecord(linkedInterviewSessionId);

            console.FinishInterview(normalInterview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });
            CheckLastHistoryRecord(null);

            Assert.IsNull(TaskRepository.GetByPerson(person.Id).LinkedChain);
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void LinkedInterviews_TryToSetCurrentInterviewAsNext_SetNextInterviewFails()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            Assert.AreEqual(context.GetInterview("S1.I1").Id, interview.Id, "Wrong current interview");
            Assert.IsFalse(console.SetLinkedInterview(context.GetInterview("S1.I1")), "SetLinkedInterview cannot set current interview as next");
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void ExecuteTwoLinkedInterviews_NoDialer_LinkedChainIsUpdateCorrectly()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S1.I4"));

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            var linkedInterviewSessionId = TaskRepository.GetByPerson(person.Id).LinkedInterviewSessionId;

            console.SetLinkedInterview(context.GetInterview("S1.I3"));

            CheckLastHistoryRecord(linkedInterviewSessionId);
            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id } }, person.Id);

            linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            CheckLastHistoryRecord(linkedInterviewSessionId);
            Assert.AreEqual("S1.I3", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem>
            {
                new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id },
                new LinkedChainItem { Id = 2, SurveyId = survey.Id, InterviewId = linkedInterview.Id }
            }, person.Id);

            var normalInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });
            CheckLastHistoryRecord(linkedInterviewSessionId);

            Assert.AreEqual("S1.I2", normalInterview.Tag, "Not correct interview");
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void ExecuteTwoLinkedInterviews_GoBackFromSecondInterview_OneInterviewInLinkedChain_NoDialer_LinkedChainIsUpdateCorrectly()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S1.I4"));

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id } }, person.Id);

            Assert.IsTrue(new ManagementService().SetNextLinkedInterviewToPrevious(person.Id)); //SetLinkedInterview(context, "S1.I1"); // Go back

            var gobackInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I1", gobackInterview.Tag, "Not correct goback interview");
            Assert.IsNull(TaskRepository.GetByPerson(person.Id).LinkedChain);
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void ExecuteGoBackToPreviousInterview_CallIsNotAvailable_RecreatingCall_SetNextLinkedInterviewToPreviousReturnsTrue()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S1.I4"));

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id } }, person.Id);

            Assert.IsTrue(new ManagementService().SetNextLinkedInterviewToPrevious(person.Id)); //SetLinkedInterview(context, "S1.I1"); // Go back
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void SetNextLinkedInterview_CallIsNotAvailable_SetNextLinkedInterviewReturnsFalse()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            console.StartInterview();

            Assert.IsFalse(console.SetLinkedInterview(context.GetInterview("S1.I4")));
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void GoBackFromNormalInterview_SetNextLinkedInterviewToPreviousReturnsFalse()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            console.StartInterview();

            Assert.IsFalse(new ManagementService().SetNextLinkedInterviewToPrevious(person.Id));
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void ExecuteTwoLinkedInterviews_GoBackFromSecondInterview_TwoInterviewsInLinkedChain_NoDialer_LinkedChainIsUpdateCorrectly()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1", CallState = (int) CallState.DisabledByUser}}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S1.I4")));

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S1.I3")));

            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id } }, person.Id);

            linkedInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I3", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem>
            {
                new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id },
                new LinkedChainItem { Id = 2, SurveyId = survey.Id, InterviewId = linkedInterview.Id }
            }, person.Id);

            new ManagementService().SetNextLinkedInterviewToPrevious(person.Id); // SetLinkedInterview(context, "S1.I4");   //go back
            var gobackInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I4", gobackInterview.Tag, "Not correct goback interview");
            AssertLinkedChain(new List<LinkedChainItem>
            {
                new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id }
            }, person.Id);
        }


        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void ExecuteTwoLinkedInterviews_GetLinkedInterviewsReturnsTwoRecords()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, TelephoneNumber = "123", RespondentName = "abc", Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, TelephoneNumber = "456", RespondentName = "def", Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S1.I4"));
            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            console.SetLinkedInterview(context.GetInterview("S1.I3"));
            console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" });

            var interviews = new ManagementService().GetLinkedInterviews(person.Id);
            Assert.AreEqual(2, interviews.Length, "Wrong number of linked interviews");

            Assert.AreEqual(interview.Id, interviews[0].RespondentId);
            Assert.AreEqual("123", interviews[0].TelephoneNumber);
            Assert.AreEqual("abc", interviews[0].RespondentName);

            Assert.AreEqual(linkedInterview.Id, interviews[1].RespondentId);
            Assert.AreEqual("456", interviews[1].TelephoneNumber);
            Assert.AreEqual("def", interviews[1].RespondentName);
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void SetNextInterview_PersonAssignmentModeOnlyAssignedCalls_CallAssignedToOtherPerson_SetNextInterviewReturnsFalse()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P2" }}
                    },
                }},

                Persons = new[]
                {
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, AssignmentListMode = PersonAssignmentListMode.AssignedCallsOnly} ,
                    new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment,  }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            console.StartInterview();

            Assert.IsFalse(console.SetLinkedInterview(context.GetInterview("S1.I4")));
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void SetNextInterview_PersonAssignmentModeOnlyAssignedCalls_SetNextInterview_CallDisabledByUser()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1", CallState = (int)CallState.DisabledByUser}}
                    },
                }},

                Persons = new[]
                {
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, AssignmentListMode = PersonAssignmentListMode.AssignedCallsOnly} ,
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S1.I4")));

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void SetNextInterview_PersonAssignmentModeAllCalls_SurveryAssignmentViaGroup_SetNextInterviewReturnsTrue()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", DialMode = DialingMode.Automatic,  Assigns = new[]{"PG1"},
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData() }
                    },
                }},

                Persons = new[]
                {
                    new PersonData { Tag = "P1", Memberships = "PG1", TaskChoice = TaskChoiceMode.SurveyAssignment, AssignmentListMode = PersonAssignmentListMode.AllCalls}
                },
                PersonGroups = new[]
                {
                    new PersonGroupData(){Tag="PG1", Name = "PG1"}
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            console.StartInterview();

            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S1.I4")));
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void SetNextInterview_PersonAssignmentModeAllCalls_PersonNotAssignedToSurvey_SetNextInterviewReturnsFalse()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData() }
                    },
                }},

                Persons = new[]
                {
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, AssignmentListMode = PersonAssignmentListMode.AllCalls}
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            console.StartInterview();

            Assert.IsFalse(console.SetLinkedInterview(context.GetInterview("S1.I4")));
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void GoBackFromSecondInterviewUsingSetNextLinkedInterview_NoDialer_SetNextLinkedInterviewActAsSetNextLinkedInterviewToPrevious()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S1.I4"));

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            console.SetLinkedInterview(context.GetInterview("S1.I3"));

            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id } }, person.Id);

            linkedInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I3", linkedInterview.Tag, "Not a linked interview");
            AssertLinkedChain(new List<LinkedChainItem>
            {
                new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id },
                new LinkedChainItem { Id = 2, SurveyId = survey.Id, InterviewId = linkedInterview.Id }
            }, person.Id);

            console.SetLinkedInterview(context.GetInterview("S1.I4"));   //go back
            var gobackInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I4", gobackInterview.Tag, "Not correct goback interview");
            AssertLinkedChain(new List<LinkedChainItem>
            {
                new LinkedChainItem { Id = 1, SurveyId = survey.Id, InterviewId = interview.Id }
            }, person.Id);
        }


        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void LastInterviewInAListIsLinkedInterview_DialerAvailable_DialingModeAutomatic_ProcessedAsPartOfFirstInterview()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1"},
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S1.I4"));

            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual(1, setNextInterviewParams.Count, "Linked interview - SetNextInterview was not called");
            Assert.AreEqual(survey.Model.CampaignId, setNextInterviewParams[0].CampaignId);
            Assert.AreEqual(survey.Model.CampaignId, setNextInterviewParams[0].NextCampaingId);
            Assert.AreEqual(linkedInterview.Id, setNextInterviewParams[0].NextInterviewId);

            Assert.AreEqual(0, completeCallParams.Count, "Linked interview - CompleteCall should not be called");
            Assert.AreEqual("S1.I4", linkedInterview.Tag, "Not a linked interview");

            var normalInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I2", normalInterview.Tag, "Not correct interview");
            Assert.AreEqual(1, setNextInterviewParams.Count, "Normal interview - SetNextInterview should not be called again");
            Assert.AreEqual(1, completeCallParams.Count, "Normal interview - CompleteCall should ne called");
        }


        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void LinkedInterviewIsInDifferentSurvey_DialerAvailable_DialingModeAutomatic_ProcessedAsPartOfFirstInterview()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                },
                new SurveyData
                    {
                        Tag="S2", IsOpen = true, DialMode = DialingMode.Automatic,
                        SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {
                            new InterviewData { Tag="S2.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                            new InterviewData { Tag="S2.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                            new InterviewData { Tag="S2.I3", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                            new InterviewData { Tag="S2.I4", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                        },
                    }
                },

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1"},
                }
            }.Create();

            var survey1 = context.GetSurvey("S1");
            var survey2 = context.GetSurvey("S2");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey1);

            console.Login();
            console.LoginToDialer();

            var interview = console.StartInterview();

            console.SetLinkedInterview(context.GetInterview("S2.I4"));

            TestDialerHelper.CompleteCallParams completeCallParams = null;
            TestDialerHelper.SetNextInterviewParams setNextInterviewParams = null;
            TestDialerHelper.SetCampaignParams setCampaignParams = null;

            dialer.DialerHelper.SetBehaviorForCompleteCall((args) =>
            {
                completeCallParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForNextInterview((args) =>
            {
                setNextInterviewParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForSetCampaign((args) =>
            {
                setCampaignParams = args;
                return 0;
            });

            var linkedInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            AssertLinkedChain(new List<LinkedChainItem> { new LinkedChainItem { Id = 1, SurveyId = survey1.Id, InterviewId = interview.Id } }, person.Id);
            Assert.IsNotNull(setNextInterviewParams, "Linked interview - SetNextInterview was not called");
            Assert.AreEqual(survey1.Model.CampaignId, setNextInterviewParams.CampaignId);
            Assert.AreEqual(survey2.Model.CampaignId, setNextInterviewParams.NextCampaingId);
            Assert.AreEqual(linkedInterview.Id, setNextInterviewParams.NextInterviewId);

            Assert.IsNull(completeCallParams, "Linked interview - CompleteCall should not be called");
            Assert.AreEqual("S2.I4", linkedInterview.Tag, "Not a linked interview");

            completeCallParams = null;
            setNextInterviewParams = null;
            var normalInterview = console.NextInterview(linkedInterview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I2", normalInterview.Tag, "Not correct interview");
            Assert.IsNull(setNextInterviewParams, "Normal interview - SetNextInterview should not be called");
            Assert.IsNotNull(completeCallParams, "Normal interview - CompleteCall was not be called");
            Assert.IsNotNull(setCampaignParams, "Normal interview - SetCampaign was not called");
            Assert.AreEqual(survey1.Model.CampaignId, setCampaignParams.CampaignId);

        }


        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void LastInterviewInAListIsLinkedInterview_DialerAvailable_DialingModePredictive_ProcessedAsPartOfFirstInterview()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive,  Assigns = new [] {"P1"},
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData()}
                    },
                }},

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
               {
                    new DialerData(){ Tag = "D1"},
                },
                StateData = new StateData[] {
                    new StateData { StateID = 33, Name = "CustomTestStatus1" },
                    new StateData { StateID = 34, Name = "CustomTestStatus2" }
                }
            }.Create();


            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);

            var requestCalls = console.LoginAndStart(2);
            var interview = console.WaitInterview(requestCalls, requestCalls.CallList.First());

            console.SetLinkedInterview(context.GetInterview("S1.I4"));

            TestDialerHelper.CompleteCallParams completeCallParams = null;
            TestDialerHelper.SetNextInterviewParams setNextInterviewParams = null;

            dialer.DialerHelper.SetBehaviorForCompleteCall((args) =>
            {
                completeCallParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForNextInterview((args) =>
            {
                setNextInterviewParams = args;
                return 0;
            });


            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "Complete", InterviewDuration = 10, Status = "Complete" });

            interview = context.GetInterview("S1.I4");
            Assert.IsNotNull(setNextInterviewParams, "Linked interview - SetNextInterview was not called");
            Assert.IsNull(completeCallParams, "Linked interview - CompleteCall should not be called");
            Assert.AreEqual(interview.Id, setNextInterviewParams.NextInterviewId, "Not a linked interview");
            Assert.AreEqual(13, setNextInterviewParams.InterviewStatus.Code);
            Assert.AreEqual("Complete", setNextInterviewParams.InterviewStatus.Name);

            completeCallParams = null;
            setNextInterviewParams = null;
            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "33", InterviewDuration = 10, Status = "Complete" });

            interview = console.WaitInterview(requestCalls, requestCalls.CallList[1]);
            Assert.AreEqual("S1.I2", interview.Tag, "Not correct interview");
            Assert.IsNull(setNextInterviewParams, "Normal interview - SetNextInterview should not be called");
            Assert.IsNotNull(completeCallParams, "Normal interview - CompleteCall was not be called");
            Assert.AreEqual(33, completeCallParams.InterviewStatus.Code);
            Assert.AreEqual("CustomTestStatus1", completeCallParams.InterviewStatus.Name);
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void LastInterviewInAListIsLinkedInterview_TwoSurveys_DialerAvailable_DialingModePredictive_OriginalSurveyIdIsRestored()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", DialMode = DialingMode.Predictive,  Assigns = new [] {"P1"},

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData()}
                    },

                },
                    new SurveyData
                    {
                        Tag="S2", IsOpen = true, DialMode = DialingMode.Automatic, Assigns = new [] {"P1"},
                        SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {
                            new InterviewData { Tag="S2.I1", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I2", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I3", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I4", ITS=CallOutcome.FreshSample, Call = new CallData ()}
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1"},
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);

            var requestCalls = console.LoginAndStart(2);
            var interview = console.WaitInterview(requestCalls, requestCalls.CallList.First());

            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S2.I4")));

            TestDialerHelper.CompleteCallParams completeCallParams = null;
            TestDialerHelper.SetCampaignParams setCampaignParams = null;

            dialer.DialerHelper.SetBehaviorForCompleteCall((args) =>
            {
                completeCallParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForSetCampaign((args) =>
            {
                setCampaignParams = args;
                return 0;
            });

            completeCallParams = null;
            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });
            interview = console.WaitInterview();

            Assert.AreEqual(context.GetSurvey("S2").Id, TaskRepository.GetByPerson(person.Id).SurveySID, "New survey id is not correct");

            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.IsNotNull(completeCallParams, "Normal interview - CompleteCall was not called");
            Assert.AreEqual(survey.Id, TaskRepository.GetByPerson(person.Id).SurveySID, "Original survey Id was not restored");
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void StartMasterInterviewInAutomaticSurvey_CompleteLinkedInterviewInPredicitveSurvey_ReturnToOriginalSurveysAndReceiveNewInterview()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", DialMode = DialingMode.Automatic,  Assigns = new [] {"P1"},

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData()}
                    },

                },
                    new SurveyData
                    {
                        Tag="S2", IsOpen = true, DialMode = DialingMode.Predictive, Assigns = new [] {"P1"},
                        SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {
                            new InterviewData { Tag="S2.I1", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I2", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I3", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I4", ITS=CallOutcome.FreshSample, Call = new CallData ()}
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1"},
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);

            var interview = console.LoginAndStart();

            Assert.AreEqual("S1.I1", interview.Tag, "New survey id is not correct");
            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S2.I4")));

            interview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S2.I4", interview.Tag, "New survey id is not correct");

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);
            Assert.AreEqual(1, setCampaignParams.Count);

            interview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I2", interview.Tag, "New survey id is not correct");

            Assert.AreEqual(2, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);
            Assert.AreEqual(2, setCampaignParams.Count);
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void StartMasterInterviewInPreviewSurvey_CompleteLinkedInterviewInPredicitveSurvey_ReturnToOriginalSurveysAndReceiveNewInterview()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", DialMode = DialingMode.Preview,  Assigns = new [] {"P1"},

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData()}
                    },

                },
                    new SurveyData
                    {
                        Tag="S2", IsOpen = true, DialMode = DialingMode.Predictive, Assigns = new [] {"P1"},
                        SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {
                            new InterviewData { Tag="S2.I1", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I2", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I3", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I4", ITS=CallOutcome.FreshSample, Call = new CallData ()}
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1"},
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var setNextInterviewParams = dialer.Behavior.Methods.SetNextInterview.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();

            var console = new AutomaticConsoleController(context, person, survey, dialer);

            var interview = console.LoginAndStart();

            Assert.AreEqual("S1.I1", interview.Tag, "Wrong interview was delivered.");

            Assert.AreEqual(0, sendNumberToAgentParams.Count);
            Assert.AreEqual(0, setNextInterviewParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);
            Assert.AreEqual(0, setCampaignParams.Count);

            console.Dial();

            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S2.I4")));

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(0, setNextInterviewParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);
            Assert.AreEqual(1, setCampaignParams.Count);

            interview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S2.I4", interview.Tag, "Wrong interview was delivered.");

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);
            Assert.AreEqual(1, setCampaignParams.Count);

            interview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.AreEqual("S1.I2", interview.Tag, "Wrong interview was delivered.");

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, setNextInterviewParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);
            Assert.AreEqual(2, setCampaignParams.Count);
        }

        [TestMethod, TestCategory(TestsCategoriesNames.LinkedInterviews)]
        public void LastInterviewInAListIsLinkedInterview_TwoSurveys_DialerAvailable_DialingModePredictive_OriginalSurveyCloseDuringInterviewInTheSecondSurvey_NoErrorAndPersonIsLoggedOut()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", DialMode = DialingMode.Predictive,  Assigns = new [] {"P1"},

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData { Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData()}
                    },

                },
                    new SurveyData
                    {
                        Tag="S2", IsOpen = true, DialMode = DialingMode.Automatic, Assigns = new [] {"P1"},
                        SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {
                            new InterviewData { Tag="S2.I1", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I2", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I3", ITS=CallOutcome.FreshSample, Call = new CallData ()},
                            new InterviewData { Tag="S2.I4", ITS=CallOutcome.FreshSample, Call = new CallData ()}
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1"},
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);

            var requestCalls = console.LoginAndStart(2);
            var interview = console.WaitInterview(requestCalls, requestCalls.CallList.First());

            Assert.IsTrue(console.SetLinkedInterview(context.GetInterview("S2.I4")));

            TestDialerHelper.CompleteCallParams completeCallParams = null;

            dialer.DialerHelper.SetBehaviorForCompleteCall((args) =>
            {
                completeCallParams = args;
                return 0;
            });

            completeCallParams = null;
            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });
            interview = console.WaitInterview();

            Assert.AreEqual(context.GetSurvey("S2").Id, TaskRepository.GetByPerson(person.Id).SurveySID, "New survey id is not correct");

            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.CloseSurvey(survey.Id);

            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });

            Assert.IsNotNull(completeCallParams, "Normal interview - CompleteCall was not called");
            Assert.IsNull(TaskRepository.GetByPerson(person.Id), "Person was not logged out");
        }

        private void CheckLastHistoryRecord(int? linkedInterviewSessionId)
        {
            Assert.IsTrue(BvHistoryAdapter.GetAll().OrderByDescending(x => x.FiredTime).First().LinkedInterviewSessionId == linkedInterviewSessionId, "Last LinkedInterviewSessionId is incorrect");
        }

        private void AssertLinkedChain(List<LinkedChainItem> expectedlinkedChain, int personSid)
        {
            var task = TaskRepository.GetByPerson(personSid);
            Assert.IsNotNull(task.TimeCallDelivered, "Time call delivered is null");
            var actual = JsonConvert.DeserializeObject<List<LinkedChainItem>>(task.LinkedChain);
            Assert.IsNotNull(actual, "Expected is null");

            actual.SequenceEqual(expectedlinkedChain, new Comparer<LinkedChainItem>());
        }

        class Comparer<T> : IEqualityComparer<T> where T : LinkedChainItem
        {
            public bool Equals(T x, T y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                return x.Id == y.Id && x.SurveyId == y.SurveyId && x.InterviewId == y.InterviewId;
            }

            public int GetHashCode(T obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
