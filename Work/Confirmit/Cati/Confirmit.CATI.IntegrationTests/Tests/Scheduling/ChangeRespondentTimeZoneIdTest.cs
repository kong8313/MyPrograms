using System;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common.ServiceLocation;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.Scheduling
{
    [TestClass]
    public class ChangeRespondentTimeZoneIdTest : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            TimezoneManager.AddTimezone(1);     //GMT
            TimezoneManager.AddTimezone(71);     //GMT+8
            ServiceLocator.Resolve<ITimeZoneBalancingSettings>().EndOfShiftThreshold = 30;
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void LookupCallsAfterRespondentTimeZoneIdChanged()
        {
            var date = new DateTime(2021, 11, 11, 14, 35, 0);
            new DateTimeMocker(IntegrationTestingFramework.Instance).MockDate(date);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", DialMode = DialingMode.Automatic, IsUseDb = true, SchedulingScript = "SS1", Assigns = new []{"P1"},
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData(){ShiftType = 0}},
                            new InterviewData() {Tag = "S1.I3", Call = new CallData(){ShiftType = 0}},
                            new InterviewData() {Tag = "S1.I4", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I5", Call = new CallData(){ShiftType = 0}},
                            new InterviewData() {Tag = "S1.I6", Call = new CallData(){ShiftType = 0}},
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.DisableCall),
                            new Shift(1, 1, "0.13:00:00", "0.18:00:00"),
                            new Shift(2, 1, "1.13:00:00", "1.18:00:00"),
                            new Shift(3, 1, "2.13:00:00", "2.18:00:00"),
                            new Shift(4, 1, "3.13:00:00", "3.18:00:00"),
                            new Shift(5, 1, "4.13:00:00", "4.18:00:00"),
                            new Shift(6, 1, "5.13:00:00", "5.18:00:00"),
                            new Shift(7, 1, "6.13:00:00", "6.18:00:00")
                        )
                    }
                }
                
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            //after changing respondent TizoneId only calls with shiftType <=0 and > -2147483648 changed shift type to -71
            survey.SetRespondentTableColumnValue(new[] { 1,2,3,4 }, "TimeZoneId", "71");

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication(survey.Id, CancellationToken.None);

            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();
            BackendTools.RunSchedulingProcedure();

            var expectedInterviewIds = new[] {1,4,5,6 };

            TestAssert.AreEqual(expectedInterviewIds, expectedInterviewIds.Select(x => (int)TaskService.LookupByPersonSid(person.Id, survey.Id).InterviewID));
            Assert.IsNull(TaskService.LookupByPersonSid(person.Id, survey.Id));
        }
    }
}
