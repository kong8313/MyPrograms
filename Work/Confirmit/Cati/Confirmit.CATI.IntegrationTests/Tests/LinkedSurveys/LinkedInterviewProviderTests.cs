using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.LinkedSurveys
{
    [TestClass]
    public class LinkedInterviewProviderTests : BaseMockedIntegrationTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        
        [TestMethod]
        public void GetCatiInterviews_FilterByVariablesWhichAreExistedInBothSurveys_ReturnedInterviewsWereFilteredCorrectly()
        {
            var context = CreateTestContextWithTwoSurveys();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                context.Surveys.Select(x => x.Model.ProjectId).ToArray(), null, null, "q2=1 AnD q1 = 1", person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber   Filters
{   S1:Name} {     S1.I1}        resp111             111 q2=1,q1=1
{   S1:Name} {     S1.I2}        resp112             112 q2=1,q1=1
{   S2:Name} {     S2.I1}        resp111             111 q2=1,q1=1
{   S2:Name} {     S2.I2}        resp112             112 q2=1,q1=1"));
        }

        [TestMethod]
        public void GetCatiInterviews_NoFilterByVariables_ReturnedInterviewsWereFilteredCorrectly()
        {
            var context = CreateTestContextWithTwoSurveys();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                context.Surveys.Select(x => x.Model.ProjectId).ToArray(), null, null, null, person.Id);

            Assert.AreEqual(
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber Filters
{   S1:Name} {     S1.I1}        resp111             111        
{   S1:Name} {     S1.I2}        resp112             112        
{   S1:Name} {     S1.I3}        resp121             121        
{   S1:Name} {     S1.I4}        resp122             122        
{   S1:Name} {     S1.I5}        resp211             211        
{   S1:Name} {     S2.I6}        resp212             212        
{   S1:Name} {     S2.I7}        resp221             221        
{   S1:Name} {     S2.I8}        resp222             222        
{   S2:Name} {     S2.I1}        resp111             111        
{   S2:Name} {     S2.I2}        resp112             112        "),
                BackendTools.Format(interviews));
            
            interviews = new ManagementService().GetInterviews(
                context.Surveys.Select(x => x.Model.ProjectId).ToArray(), "111", "resp111", null, person.Id);

            Assert.AreEqual(
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber Filters
{   S1:Name} {     S1.I1}        resp111             111        
{   S2:Name} {     S2.I1}        resp111             111        "),
                BackendTools.Format(interviews));
        }
        
        [TestMethod]
        public void GetCatiInterviews_FilterByVariablesWhichAreNotAllExistedInBothSurveys_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = CreateTestContextWithTwoSurveys();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                context.Surveys.Select(x => x.Model.ProjectId).ToArray(), null, null, "q2=2 AnD q3 = 1", person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber   Filters
{   S1:Name} {     S1.I3}        resp121             121 q2=2,q3=1
{   S1:Name} {     S1.I7}        resp221             221 q2=2,q3=1
{   S2:Name} {     S2.I3}        resp121             121  q2=2,q3=
{   S2:Name} {     S2.I4}        resp122             122  q2=2,q3=
{   S2:Name} {     S2.I7}        resp221             221  q2=2,q3=
{   S2:Name} {     S2.I8}        resp222             222  q2=2,q3="));
        }

        [TestMethod]
        public void GetCatiInterviews_FilterByVariablesWhichContainsSqlInjectionInColumnName_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = CreateTestContextWithTwoSurveys();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                context.Surveys.Select(x => x.Model.ProjectId).ToArray(), null, null, "q2=2 AnD q3];SELECT = 1", person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber          Filters
{   S1:Name} {     S1.I3}        resp121             121 q2=2,q3];SELECT=
{   S1:Name} {     S1.I4}        resp122             122 q2=2,q3];SELECT=
{   S1:Name} {     S1.I7}        resp221             221 q2=2,q3];SELECT=
{   S1:Name} {     S1.I8}        resp222             222 q2=2,q3];SELECT=
{   S2:Name} {     S2.I3}        resp121             121 q2=2,q3];SELECT=
{   S2:Name} {     S2.I4}        resp122             122 q2=2,q3];SELECT=
{   S2:Name} {     S2.I7}        resp221             221 q2=2,q3];SELECT=
{   S2:Name} {     S2.I8}        resp222             222 q2=2,q3];SELECT="));
        }

        [TestMethod]
        public void GetCatiInterviews_FilterByVariablesWhichContainsSqlInjectionInValue_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q2", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q3", Precodes = new[] {"a", "b"}, SqlType = SqlDataType.Char}
                        },
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Call = new CallData(), Data="q2=2,q3=b"}
                        }},
                    new SurveyData(){ Tag = "S2", IsUseDb = true, IsOpen = true, Assigns = new []{"P1"},
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q2", Precodes = new[] {"1", "2"}},
                        },Interviews = new []
                        {
                            new InterviewData(){Tag="S2.I1", Call = new CallData(), Data="q2=2", RespondentName = "resp121", TelephoneNumber = "121"}
                        }},
                },
                Persons = new[] { new PersonData() { Tag = "P1" } }
            }.Create();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(null, null, null, "q2=2 And q3 = 1'\";SELECT 1", person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber  Filters
{   S2:Name} {     S2.I1}        resp121             121 q2=2,q3="));
        }

        [TestMethod]
        public void GetCatiInterviews_FilterByVariablesAndTelNumber_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = CreateTestContextWithTwoSurveys();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                context.Surveys.Select(x => x.Model.ProjectId).ToArray(), "11", null, "q2=1", person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber Filters
{   S1:Name} {     S1.I1}        resp111             111    q2=1
{   S1:Name} {     S1.I2}        resp112             112    q2=1
{   S2:Name} {     S2.I1}        resp111             111    q2=1
{   S2:Name} {     S2.I2}        resp112             112    q2=1"));
        }

        [TestMethod]
        public void GetCatiInterviews_FilterByVariablesAndResondentName_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = CreateTestContextWithTwoSurveys();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                context.Surveys.Select(x => x.Model.ProjectId).ToArray(), null, "resp11", "q2=1", person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber Filters
{   S1:Name} {     S1.I1}        resp111             111    q2=1
{   S1:Name} {     S1.I2}        resp112             112    q2=1
{   S2:Name} {     S2.I1}        resp111             111    q2=1
{   S2:Name} {     S2.I2}        resp112             112    q2=1"));
        }

        [TestMethod]
        public void GetCatiInterviews_FilterByVariablesAndResondentNameAndCallCenterForAllOpenedSurveys_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = CreateTestContextWithTwoSurveys();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                null, null, "resp11", "q2=1", person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber Filters
{   S1:Name} {     S1.I1}        resp111             111    q2=1
{   S1:Name} {     S1.I2}        resp112             112    q2=1
{   S2:Name} {     S1.I1}        resp111             111    q2=1
{   S2:Name} {     S1.I2}        resp112             112    q2=1"));
        }

        [TestMethod]
        public void GetCatiInterviews_FilterByAssignnmetsForAllOpenedSurveys_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, IsOpen = true,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Call = new CallData(), TelephoneNumber = "111"}
                        }},
                    new SurveyData(){ Tag = "S2", IsUseDb = true, IsOpen = true, Assigns = new []{"P1"},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S2.I1", Call = new CallData(), TelephoneNumber = "111"}
                        }},
                },
                Persons = new[] { new PersonData() { Tag = "P1" } }
            }.Create();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                null, null, null, null, person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber Filters
{   S2:Name} {     S2.I1}    respondent1             111        "));
        }

        [TestMethod]
        public void GetCatiInterviews_FilterByAssignnmetsForAllOpenedSurveysWithCalls_ReturnedInterviewsWereFilteredCorrectlly()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, IsOpen = true, Assigns = new []{"P1"},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Call = new CallData(), TelephoneNumber = "111"}
                        }},
                    new SurveyData(){ Tag = "S2", IsUseDb = true, IsOpen = true, Assigns = new []{"P1"},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S2.I1", TelephoneNumber = "111"}
                        }},
                },
                Persons = new[] { new PersonData() { Tag = "P1" } }
            }.Create();

            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(
                null, null, null, null, person.Id);

            Assert.AreEqual(
                BackendTools.Format(interviews),
                BackendTools.Format(context, @"
   ProjectId RespondentId RespondentName TelephoneNumber Filters
{   S1:Name} {     S1.I1}    respondent1             111        "));
        }

        [TestMethod]
        public void GetCatiInterviews_PersonAssignmentModeAllCalls_CallAssignedToOtherPersonReturned()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, IsUseDb = true, DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P2" }},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P2" }}
                    },
                }},

                Persons = new[]
                {
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment, AssignmentListMode = PersonAssignmentListMode.AllCalls} ,
                    new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment,  }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            // Set soft-delete state for the second call
            BackendTools.SetCallsStateToSoftDeleted(_framework.DbEngine, context.Surveys.First().Model.SID, 2, 2);
            var person = context.GetPerson("P1");

            var interviews = new ManagementService().GetInterviews(null, null, null, null, person.Id);
            Assert.IsTrue(interviews.Length == 1);
            
            interviews = new ManagementService().GetInterviews(
                null, null, null, "1=1", person.Id);
            Assert.IsTrue(interviews.Length == 1);
        }

        private static TestDataContext CreateTestContextWithTwoSurveys()
        {
            var context = new TestData()
            {
                CallCenters = new[]
                {
                    new CallCenterData(){Tag="CC1"},
                    new CallCenterData(){Tag="CC2"}
                },
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = true, IsOpen = true, CallCenters = new []{"CC1"}, Assigns = new []{"P1"},
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q2", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q3", Precodes = new[] {"1", "2"}}
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", TelephoneNumber = "111", RespondentName = "resp111", Data = "q1=1,q2=1,q3=1", Call = new CallData()},
                            new InterviewData {Tag = "S1.I2", TelephoneNumber = "112", RespondentName = "resp112", Data = "q1=1,q2=1,q3=2", Call = new CallData()},
                            new InterviewData {Tag = "S1.I3", TelephoneNumber = "121", RespondentName = "resp121", Data = "q1=1,q2=2,q3=1", Call = new CallData()},
                            new InterviewData {Tag = "S1.I4", TelephoneNumber = "122", RespondentName = "resp122", Data = "q1=1,q2=2,q3=2", Call = new CallData()},
                            new InterviewData {Tag = "S1.I5", TelephoneNumber = "211", RespondentName = "resp211", Data = "q1=2,q2=1,q3=1", Call = new CallData()},
                            new InterviewData {Tag = "S1.I6", TelephoneNumber = "212", RespondentName = "resp212", Data = "q1=2,q2=1,q3=2", Call = new CallData()},
                            new InterviewData {Tag = "S1.I7", TelephoneNumber = "221", RespondentName = "resp221", Data = "q1=2,q2=2,q3=1", Call = new CallData()},
                            new InterviewData {Tag = "S1.I8", TelephoneNumber = "222", RespondentName = "resp222", Data = "q1=2,q2=2,q3=2", Call = new CallData()},
                        }
                    },
                    new SurveyData
                    {
                        Tag = "S2", IsUseDb = true,  IsOpen = true, CallCenters = new []{"CC1"}, Assigns = new []{"P1"},
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q2", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q4", Precodes = new[] {"1", "2"}}
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S2.I1", TelephoneNumber = "111", RespondentName = "resp111", Data = "q1=1,q2=1,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S2.I2", TelephoneNumber = "112", RespondentName = "resp112", Data = "q1=1,q2=1,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S2.I3", TelephoneNumber = "121", RespondentName = "resp121", Data = "q1=1,q2=2,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S2.I4", TelephoneNumber = "122", RespondentName = "resp122", Data = "q1=1,q2=2,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S2.I5", TelephoneNumber = "211", RespondentName = "resp211", Data = "q1=2,q2=1,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S2.I6", TelephoneNumber = "212", RespondentName = "resp212", Data = "q1=2,q2=1,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S2.I7", TelephoneNumber = "221", RespondentName = "resp221", Data = "q1=2,q2=2,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S2.I8", TelephoneNumber = "222", RespondentName = "resp222", Data = "q1=2,q2=2,q4=2", Call = new CallData()},
                        }
                    },
                    new SurveyData
                    {
                        Tag = "S3", IsUseDb = true, IsOpen = true, CallCenters = new []{"CC2"},
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q2", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q4", Precodes = new[] {"1", "2"}}
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S3.I1", TelephoneNumber = "111", RespondentName = "resp111", Data = "q1=1,q2=1,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S3.I2", TelephoneNumber = "112", RespondentName = "resp112", Data = "q1=1,q2=1,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S3.I3", TelephoneNumber = "121", RespondentName = "resp121", Data = "q1=1,q2=2,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S3.I4", TelephoneNumber = "122", RespondentName = "resp122", Data = "q1=1,q2=2,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S3.I5", TelephoneNumber = "211", RespondentName = "resp211", Data = "q1=2,q2=1,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S3.I6", TelephoneNumber = "212", RespondentName = "resp212", Data = "q1=2,q2=1,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S3.I7", TelephoneNumber = "221", RespondentName = "resp221", Data = "q1=2,q2=2,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S3.I8", TelephoneNumber = "222", RespondentName = "resp222", Data = "q1=2,q2=2,q4=2", Call = new CallData()},
                        }
                    },
                    new SurveyData
                    {
                        Tag = "S4", IsUseDb = true,  IsOpen = false, CallCenters = new []{"CC1"}, Assigns = new []{"P1"},
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q2", Precodes = new[] {"1", "2"}},
                            new SingleFormData {Name = "q4", Precodes = new[] {"1", "2"}}
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S4.I1", TelephoneNumber = "111", RespondentName = "resp111", Data = "q1=1,q2=1,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S4.I2", TelephoneNumber = "112", RespondentName = "resp112", Data = "q1=1,q2=1,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S4.I3", TelephoneNumber = "121", RespondentName = "resp121", Data = "q1=1,q2=2,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S4.I4", TelephoneNumber = "122", RespondentName = "resp122", Data = "q1=1,q2=2,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S4.I5", TelephoneNumber = "211", RespondentName = "resp211", Data = "q1=2,q2=1,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S4.I6", TelephoneNumber = "212", RespondentName = "resp212", Data = "q1=2,q2=1,q4=2", Call = new CallData()},
                            new InterviewData {Tag = "S4.I7", TelephoneNumber = "221", RespondentName = "resp221", Data = "q1=2,q2=2,q4=1", Call = new CallData()},
                            new InterviewData {Tag = "S4.I8", TelephoneNumber = "222", RespondentName = "resp222", Data = "q1=2,q2=2,q4=2", Call = new CallData()},
                        }
                    },

                },
                Persons = new[] { new PersonData() { Tag = "P1", CallCenter = "CC1" } }
            }.Create();
            return context;
        }
    }
}
