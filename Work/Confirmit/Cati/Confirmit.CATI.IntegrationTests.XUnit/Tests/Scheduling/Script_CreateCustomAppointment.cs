using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptCreateCustomAppointment : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\DmitryS")]
        [ClassData(typeof(TestDataGenerator))]
        public void GetRespondentValue_ReadCallAttemtCount_FilterAreTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);
            TimezoneManager.AddTimezone(16);

            var context = new TestData()
            {
                Surveys = new[] {
                  new SurveyData() {
                    Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                      Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", TimeZoneId = "16" }
                      }
                  }
                },
                Scripts = new[] {
                  new ScriptData() {
                    Tag = "SS1",
                      Script = new TestScript(
                        new SubRule(new [] {
                          new Action(Action.Operation.RunCustomScript, "CreateAppointment"),
                        }),
                        new Shift(1, 1, "0.00:00:00", "1.00:00:00")) {
                        CustomScript = @"
                        function CreateAppointment() {
                          var time: DateTime = DateTime.Parse('2017-05-15 13:00');
                            CreateCustomAppointment(time);
                        }
                        "
                      }

                }
            }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var appointment = AppointmentRepository.GetAppointmentForInterview(context.GetSurvey("S1").Id, interview.Id, AppointmentState.ActiveWithoutCall);

            Assert.IsNotNull(appointment);
            Assert.AreEqual(0, appointment.State);
            Assert.AreEqual(DateTime.Parse("2017-05-15 10:00"), appointment.Time);
        }
    }
}
