using System.Globalization;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class TcpaManualModeTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void PersonDialTypeIsManual_OnlyDialTypeManualCallsAreDelivered()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
 
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[]{
                    new PersonData { Tag="P1", TaskChoice = TaskChoiceMode.SurveyAssignment, DialType = DialType.Cellphone }
                },
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var expected = context.GetInterviewsInOrder("S1.I1", "S1.I3");
            var actual = console.ProcessAllInterviews();

            CollectionAssert.AreEqual(
                expected.Select(x => x.Id).ToArray(),
                actual.Select(x => x.Id).ToArray(),
                "expected: [{0}], actual: [{1}]",
                string.Join(", ", expected.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))),
                string.Join(", ", actual.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void PersonDialTypeIsAutomatic_OnlyDialTypeAutomaticCallsAreDelivered()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
 
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[]{
                    new PersonData { Tag="P1", TaskChoice = TaskChoiceMode.SurveyAssignment, DialType = DialType.Landline }
                },
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var expected = context.GetInterviewsInOrder("S1.I2", "S1.I4");
            var actual = console.ProcessAllInterviews();

            CollectionAssert.AreEqual(
                expected.Select(x => x.Id).ToArray(),
                actual.Select(x => x.Id).ToArray(),
                "expected: [{0}], actual: [{1}]",
                string.Join(", ", expected.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))),
                string.Join(", ", actual.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void PersonDialTypeIsDefault_OnlyDialTypeAutomaticCallsAreDelivered()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,
 
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I2", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I3", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                        new InterviewData { Tag="S1.I4", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},

                Persons = new[]{
                    new PersonData { Tag="P1", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var expected = context.GetInterviewsInOrder("S1.I2", "S1.I4");
            var actual = console.ProcessAllInterviews();

            CollectionAssert.AreEqual(
                expected.Select(x => x.Id).ToArray(),
                actual.Select(x => x.Id).ToArray(),
                "expected: [{0}], actual: [{1}]",
                string.Join(", ", expected.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))),
                string.Join(", ", actual.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void PersonDialTypeIsManualDialingModeIsPredictive_OnlyDialTypeManualCallsAreDelivered()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                 {
                     Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive,
  
                     Interviews = new[] {
                         new InterviewData { Tag="S1.I1", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                         new InterviewData { Tag="S1.I2", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                         new InterviewData { Tag="S1.I3", DialType = DialType.Cellphone, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }},
                         new InterviewData { Tag="S1.I4", DialType = DialType.Landline, ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                     },
                 }},

                Persons = new[]{
                     new PersonData { Tag="P1", TaskChoice = TaskChoiceMode.SurveyAssignment, DialType = DialType.Cellphone }
                 },
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();

            var expected = context.GetInterviewsInOrder("S1.I1", "S1.I3");
            var actual = console.ProcessAllInterviews();

            CollectionAssert.AreEqual(
                expected.Select(x => x.Id).ToArray(),
                actual.Select(x => x.Id).ToArray(),
                "expected: [{0}], actual: [{1}]",
                string.Join(", ", expected.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))),
                string.Join(", ", actual.Select(x => x.Id.ToString(CultureInfo.InvariantCulture))));
        }
    }
}
