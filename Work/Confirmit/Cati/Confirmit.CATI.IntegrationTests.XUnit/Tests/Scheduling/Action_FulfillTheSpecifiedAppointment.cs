using System;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ActionFulfillTheSpecifiedAppointment : BaseMockedIntegrationTest
    {
        private void Test_Base(DateTime eventTime, DateTime appointermtTime, int param, DateTime timeInShift, bool withCall)
        {
            var script = new TestScript(
                    new Action(Action.Operation.FulfillTheSpecifiedAppointment, param.ToString(CultureInfo.InvariantCulture)),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(1, 1, "1.00:00:00", "0.00:00:00"));
            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = 1 /*Appointment*/;
            BackendTools.CreateInterview(interview);

            BackendTools.AddAppointment(interview.ID, surveySID, appointermtTime);
            var appt = SurveyService.GetAppointments(surveySID, interview.ID).FirstOrDefault();

            var call = BackendTools.NewCall(interview);
            if (withCall)
            {
                BackendTools.CreateCall(call);
            }

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview, eventTime);

            //
            // Action executed, so lets check execution results
            //

            BackendTools.CheckInterview(interview);

            call.ApptID = appt.ID;
            call.TimeInShift = timeInShift;
            call.TimeToExpire = appt.ExpTime;
            if (!withCall)
                call.Priority = 1000;
            call.ShiftID = (int)CallShiftType.None;

            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void WithCall_FulfillTheSpecifiedAppointmentWith0_CallIsCreatedCorrectly(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(DateTime.Parse("2010-01-25T00:00:00"),
                        DateTime.Parse("2010-01-25T10:00:00"),
                        0,
                        DateTime.Parse("2010-01-25T10:00:00"),
                        true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void WithoutCall_FulfillTheSpecifiedAppointmentWith0_CallIsCreatedCorrectly(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(DateTime.Parse("2010-01-25T00:00:00"),
                        DateTime.Parse("2010-01-25T10:00:00"),
                        0,
                        DateTime.Parse("2010-01-25T10:00:00"),
                        false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void WithCall_FulfillTheSpecifiedAppointmentWith30_CallIsCreatedCorrectly(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(DateTime.Parse("2010-01-25T00:00:00"),
                        DateTime.Parse("2010-01-25T10:00:00"),
                        30,
                        DateTime.Parse("2010-01-25T09:30:00"),
                        true);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void WithoutCall_FulfillTheSpecifiedAppointmentWith30_CallIsCreatedCorrectly(SecurityMode mode)
        {
            SetSecurityMode(mode);

            Test_Base(DateTime.Parse("2010-01-25T00:00:00"),
                        DateTime.Parse("2010-01-25T10:00:00"),
                        30,
                        DateTime.Parse("2010-01-25T09:30:00"),
                        false);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallWithActiveAppointment_RescheduleCall_CallWithoutAppointment(SecurityMode mode)
        {
            SetSecurityMode(mode);

            DateTime eventTime1 = DateTime.Parse("2010-01-25T00:00:00");
            DateTime appointermtTime = DateTime.Parse("2010-01-25T10:00:00");
            DateTime eventTime2 = DateTime.Parse("2010-01-25T01:00:00");

            var script = new TestScript(
                    new[]
                    {
                        new SubRule(
                            new[]
                            {
                                new Action(Action.Operation.FulfillTheSpecifiedAppointment, "0" ),
                                new Action(Action.Operation.SetNewITS, "31")
                            })
                            {
                                ItsId = 1/*Appointment*/
                            },
                        new SubRule(
                            new[]
                            {
                                new Action(Action.Operation.RecallAfterANumberOfMinutes, "10" ),
                                new Action(Action.Operation.SetNewITS, "32")
                            })
                            {
                                ItsId = 31
                            }
                    },
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(1, 1, "1.00:00:00", "0.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = 1 /*Appointment*/;
            BackendTools.CreateInterview(interview);

            BackendTools.AddAppointment(interview.ID, surveySID, appointermtTime);
            var appt = SurveyService.GetAppointments(surveySID, interview.ID).FirstOrDefault();

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview, eventTime1);

            //
            // Action executed, so lets check execution results
            //
            interview.TransientState = 31;
            BackendTools.CheckInterview(interview);

            call.ApptID = appt.ID;
            call.TimeInShift = appointermtTime;
            call.TimeToExpire = appt.ExpTime;
            call.Priority = 1000;
            call.ShiftID = (int)CallShiftType.None;

            BackendTools.CheckCall(call);

            //
            // Check appointment
            //

            var appts = BvAppointmentAdapter.GetAll();
            Assert.AreEqual(1, appts.Count);
            Assert.AreEqual(1/*Active*/, appts[0].State);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview, eventTime2);


            interview.TransientState = 32;
            BackendTools.CheckInterview(interview);

            call.ApptID = 0;
            call.TimeInShift = eventTime2.AddMinutes(10);
            call.TimeToExpire = new DateTime(9999, 1, 1);
            call.ShiftID = (int)CallShiftType.None;
            BackendTools.CheckCall(call);

            //
            // Check appointment
            //

            appts = BvAppointmentAdapter.GetAll();
            Assert.AreEqual(1, appts.Count);
            Assert.AreEqual(2/*Deactive*/, appts[0].State);
        }

        [Theory, Owner(@"FIRM\AlexanderL"), Cr(43372)]
        [ClassData(typeof(TestDataGenerator))]
        public void CallWithActiveAppointment_CallDoesntFitInShift_AppointmentIsAddedSuccessfully(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var appointermtTime = DateTime.Parse("2010-04-09T02:00:00");
            var eventTime = DateTime.Parse("2010-04-08T03:00:00");

            var script = new TestScript(
                    new[]
                    {
                        new SubRule(
                            new[]
                            {
                                new Action(Action.Operation.FulfillTheSpecifiedAppointment, "2" ),
                                new Action(Action.Operation.SetNewITS, "31")
                            })
                            {
                                ItsId = 1/*Appointment*/
                            }
                    },
                    new Shift(1, 1, "0.03:00:00", "0.07:00:00"),
                    new Shift(2, 1, "1.03:00:00", "1.07:00:00"),
                    new Shift(3, 1, "2.03:00:00", "2.07:00:00"),
                    new Shift(4, 1, "3.03:00:00", "3.07:00:00"),
                    new Shift(5, 1, "4.03:00:00", "4.07:00:00"),
                    new Shift(6, 1, "5.03:00:00", "5.07:00:00"),
                    new Shift(7, 1, "6.03:00:00", "6.07:00:00"));

            var surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = 1 /*Appointment*/;
            BackendTools.CreateInterview(interview);

            BackendTools.AddAppointment(interview.ID, surveySID, appointermtTime);
            var appt = SurveyService.GetAppointments(surveySID, interview.ID).FirstOrDefault();

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview, eventTime);

            //
            // Action executed, so lets check execution results
            //
            interview.TransientState = 31;
            BackendTools.CheckInterview(interview);

            call.ApptID = appt.ID;
            call.TimeInShift = appointermtTime.AddMinutes(-2);
            call.TimeToExpire = appt.ExpTime;
            call.Priority = 1000;
            call.ShiftID = (int)CallShiftType.None;

            BackendTools.CheckCall(call);
        }


        [Theory, Owner(@"FIRM\Egork")]
        [ClassData(typeof(TestDataGenerator))]
        public void SetAppointmentTimeZone_ExecuteSchedulingScript_InterviewTimeZoneIsCorrect(SecurityMode mode)
        {
            var timeZoneId = 3;
            TimezoneManager.AddTimezone(timeZoneId);
            ServiceLocator.Resolve<ISystemSettings>().Console.EnableAppointmentTimeZoneAdjustment = true;
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data="q1=1", TimeZoneId = "1"}
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(
                            new[]
                            {
                                new Action(Action.Operation.FulfillTheSpecifiedAppointment, "2" ),
                                new Action(Action.Operation.SetNewITS, "31")
                            })
                            {
                                ItsId = 1/*Appointment*/
                            }, new Shift(1, 1, "0.00:00:00", "6.23:59:59"))  {
                            CustomScript = @""
                        }

                    }
                }
            }.Create();

            SetSecurityMode(mode);

            var appointermtTime = DateTime.Parse("2010-04-09T02:00:00");
            var eventTime = DateTime.Parse("2010-04-08T03:00:00");


            var surveySID = context.GetSurvey("S1").Id;

            var interview = context.GetInterview("S1.I1").Model;
            interview.TransientState = 1 /*Appointment*/;

            BackendTools.AddAppointment(interview.ID, surveySID, appointermtTime, false, timeZoneId);
            var appt = SurveyService.GetAppointments(surveySID, interview.ID).FirstOrDefault();

            var call = BackendTools.NewCall(interview);

            //
            // Execute scheduling script
            // 
            BackendTools.FireEvent(interview, eventTime);

            //
            // Action executed, so lets check execution results
            //
            interview.TransientState = 31;
            interview.TimezoneID = 3;
            BackendTools.CheckInterview(interview);

            call.ApptID = appt.ID;
            call.TimeInShift = appointermtTime.AddMinutes(-2);
            call.TimeToExpire = appt.ExpTime;
            call.Priority = 1000;
            call.ShiftID = (int)CallShiftType.None;

            BackendTools.CheckCall(call);

            var sql = "SELECT TimeZoneId FROM <Schema>.respondent WHERE respid = @RespId";
            var respondentTimeZone = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int>(surveySID, sql, 
                new System.Data.SqlClient.SqlParameter("@RespId", interview.ID));

            Assert.AreEqual(timeZoneId, respondentTimeZone);
        }
    }
}
