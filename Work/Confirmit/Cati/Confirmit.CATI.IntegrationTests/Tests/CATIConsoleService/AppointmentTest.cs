using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.Common.Exceptions;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Backend.WcfServices.External.ConsoleService.Fakes;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.IntegrationTests.Tests.CATIConsoleService
{
    [TestClass]
    public class AddingAppointment : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        /// <summary>
        /// 1. Add survey, launch 'all hours' script, open survey
        /// 2. Create person in automatic mode
        /// 3. Assign person to survey
        /// 4. Create interview with changed time zone
        /// 5. Create call
        /// 6. Add appointment in 24 hours and apply it
        /// 7. Check that BvAppointment table contains appointment with right time zone
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddingAppointment_AddAppointmentOnInterviewWithTZ_AppointmentHasTheSameTZ()
        {
            const int respondentTimeZone = 10;

            BackendToolsObject.LaunchAllHoursScript();
            int surveySID = BackendToolsObject.CreateSurvey("p000001");
            _surveyStateService.Open(surveySID);

            int personID = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveySID, personID);

            BvInterviewEntity interview = BackendTools.NewInterview(surveySID);

            TimezoneManager.AddTimezone(respondentTimeZone);
            interview.TransientState = 16; /*16 - fresh sample*/
            interview.TimezoneID = respondentTimeZone;
            BackendTools.CreateInterview(interview);

            BvCallEntity call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            DateTime now = DateTime.Now.AddHours(24);

            BackendTools.AddAppointment(interview.ID, surveySID, now);
            BackendTools.SetInterviewItsAppointment(surveySID, interview.ID);

            List<BvAppointmentEntity> res = BvAppointmentAdapter.GetByCondition(
              "InterviewSID = @InterviewSID and SurveySID = @SurveySID",
              new SqlParameter("@InterviewSID", interview.ID),
              new SqlParameter("@SurveySID", surveySID));

            Assert.AreEqual(1, res.Count, "BvAppointment table have wrong rows count");
            Assert.AreEqual(respondentTimeZone, res[0].TZID, "Respondent time zone for appointment is wrong");
        }

        /// <summary>
        /// 1. Create survey.
        /// 2. Create scheduling script. Script should contain gap between shifts.
        /// 3. Assign script to survey.
        /// 4. Create new interview for survey.
        /// 5. Create appointment which is out of shifts.
        /// 6. Set appointment to interview with flag 
        ///   "Allow appointments outside of permitted shift hours" set false.
        /// 7. UserMessageException should be thrown.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Cr(45793)]
        [ExpectedException(typeof(UserMessageException))]
        public void AddingAppointment_AddAppointmentOutOfShiftsWithCheck_ExceptionThrown()
        {
            var apptTime = new DateTime(2010, 8, 2, 12, 0, 0, 0); // 2010-08-02 12:00:00 Monday
            var script = new TestScript(new SubRule(Guid.NewGuid(), new Action[0]))
            {
                Shifts = new List<Shift>
                    {
                        new Shift(1, 1, "2.00:00:00", "2.23:59:59"), // Tuesday
                        new Shift(2, 1, "3.00:00:00", "3.23:59:59"), // Wednesday
                        new Shift(3, 1, "4.00:00:00", "4.23:59:59"), // Thursday
                        new Shift(4, 1, "5.00:00:00", "5.23:59:59"), // Friday
                        new Shift(5, 1, "6.00:00:00", "6.23:59:59"), // Saturday
                        new Shift(6, 1, "0.00:00:00", "0.23:59:59") // Sunday
                    }
            };

            // all days except monday

            int surveyId = BackendToolsObject.CreateSurvey(script);
            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);

            BackendTools.AddAppointment(interview.ID, surveyId, apptTime, false);
        }

        /// <summary>
        /// 1. Create survey.
        /// 2. Create scheduling script. Script should contain gap between shifts.
        /// 3. Assign script to survey.
        /// 4. Create new interview for survey.
        /// 5. Create appointment which is out of shifts.
        /// 6. Set appointment to interview with flag 
        ///   "Allow appointments outside of permitted shift hours" set true.
        /// 7. Appointment should be added successfully.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Cr(45793)]
        public void AddingAppointment_AddAppointmentOutOfShiftsWithNoCheck_Success()
        {
            var apptTime = new DateTime(2010, 8, 2, 12, 0, 0, 0); // 2010-08-02 12:00:00 Monday
            var script = new TestScript(new SubRule(Guid.NewGuid(), new Action[0]))
            {
                Shifts = new List<Shift>
                    {
                        new Shift(1, 1, "2.00:00:00", "2.23:59:59"), // Tuesday
                        new Shift(2, 1, "3.00:00:00", "3.23:59:59"), // Wednesday
                        new Shift(3, 1, "4.00:00:00", "4.23:59:59"), // Thursday
                        new Shift(4, 1, "5.00:00:00", "5.23:59:59"), // Friday
                        new Shift(5, 1, "6.00:00:00", "6.23:59:59"), // Saturday
                        new Shift(6, 1, "0.00:00:00", "0.23:59:59") // Sunday
                    }
            };

            // all days except monday

            int surveyId = BackendToolsObject.CreateSurvey(script);
            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);

            BackendTools.AddAppointment(interview.ID, surveyId, apptTime, true);

            var appt = SurveyService.GetAppointments(surveyId, interview.ID);
            Assert.AreEqual(1, appt.Length);
            Assert.AreEqual(apptTime, appt[0].Time);
        }

        #region GettingAppointmentList_GetAppointmentsAssignedOnPersonAndForOpenSurvey_AppointmentsAreReturned
        /// <summary>
        /// Create appointment
        /// </summary>
        /// <param name="interview"></param>
        /// <param name="surveySID"></param>
        /// <param name="apptID"></param>
        /// <param name="now"></param>
        private static void CreateAppointment(BvInterviewEntity interview, int surveySID, int apptID, DateTime now)
        {
            DateTime date = now.AddDays(apptID);
            BackendTools.AddAppointment(interview.ID, surveySID, date);
            BackendTools.SetInterviewItsAppointment(surveySID, interview.ID);
        }


        /// <summary>
        /// Create Interview with two appointments
        /// </summary>
        /// <param name="surveySID"></param>
        /// <param name="personSID"></param>
        /// <param name="sid"></param>
        /// <param name="now"></param>
        /// <param name="tzid"></param>
        private static void CreateInterviewWith2Appoinments(int surveySID, int? personSID, ref int sid, DateTime now, int tzid)
        {
            TimezoneManager.AddTimezone(tzid);

            BvInterviewEntity interview = BackendTools.NewInterview(surveySID);
            interview.TimezoneID = tzid;
            BackendTools.CreateInterview(interview);

            BvCallEntity call = BackendTools.NewCall(interview);
            call.CallState = 2;
            if (personSID != null)
                call.Resource = personSID.Value;
            BackendTools.CreateCall(call);

            CreateAppointment(interview, surveySID, ++sid, now);
            CreateAppointment(interview, surveySID, ++sid, now);
        }


        /// <summary>
        /// 1. Create 2 persons in manual mode
        /// 2. Add 5 surveys, launch 'all hours' script, open all surveys
        /// 3. Create 2 interview with call for each survey. First of them assigned on first person. 
        ///    Second assign on another person or does not assign. TZID = InterviewID. Time to
        ///    appointment it is datetime.NOW + ID days.
        /// 4. Create 2 appointment of each interview (first will be without call)
        /// 5. Close second and fourth surveys
        /// 6. Assign person1 on first, second and fifth surveys
        /// 7. Login person in caticonsole and BE
        /// 8. Call scheduling procedure
        /// 9. Call GetAllAppointmentList method of WS
        /// 
        /// We should get only second appointment (with cal).
        /// We should get appointment only for open surveys
        /// We should get appointment only for assigned surveys
        /// Result: second appointment for first, third and fifth survey.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GettingAppointmentList_GetAppointmentsAssignedOnPersonAndForOpenSurvey_AppointmentsAreReturned()
        {
            int sid = 0;
            int tzid = 1;
            var now = new DateTime(2040, 01, 01, 0, 0, 0);

            const int surveysCount = 5;
            const string user1 = "APerson1";
            const string password1 = "password1";
            const string user2 = "APerson2";
            const string password2 = "password2";

            string pr1, pr2, pr3, pr4, pr5;
            var db1 = ConfirmitTools.GetConfirmitSurveyDbOnTest(out pr1);
            var db2 = ConfirmitTools.GetConfirmitSurveyDbOnTest(out pr2);
            var db3 = ConfirmitTools.GetConfirmitSurveyDbOnTest(out pr3);
            var db4 = ConfirmitTools.GetConfirmitSurveyDbOnTest(out pr4);
            var db5 = ConfirmitTools.GetConfirmitSurveyDbOnTest(out pr5);

            var projectIDs = new[] { pr1, pr2, pr3, pr4, pr5 };
            var projectDbs = new[] { db1.ConnectionString, db2.ConnectionString, db3.ConnectionString, db4.ConnectionString, db5.ConnectionString };
            var surveySIDs = new int[surveysCount];

            int personSID1 = PersonTools.CreatePerson(user1, password1, AgentTaskChoiceMode.Manual);
            int personSID2 = PersonTools.CreatePerson(user2, password2, AgentTaskChoiceMode.Manual);

            BackendToolsObject.LaunchAllHoursScript();

            for (int i = 0; i < surveysCount; ++i)
            {
                surveySIDs[i] = BackendToolsObject.CreateSurvey(projectIDs[i], projectDbs[i]);

                _surveyStateService.Open(surveySIDs[i]);

                //first interview with call
                CreateInterviewWith2Appoinments(surveySIDs[i], personSID1, ref sid, now, tzid++);
                //second interview with call
                //every each second call assign on person2
                CreateInterviewWith2Appoinments(surveySIDs[i], (i % 2 != 0 ? (int?)personSID2 : null), ref sid, now, tzid++);

                if ((i & 1) == 1) /*every second survey should be closed*/
                    _surveyStateService.CloseSurvey(surveySIDs[i]);
            }

            //Assign person1 on first, second and fifth surveys
            BackendTools.AssignCatiPersonToSurvey(surveySIDs[0], personSID1);
            BackendTools.AssignCatiPersonToSurvey(surveySIDs[1], personSID1);
            BackendTools.AssignCatiPersonToSurvey(surveySIDs[4], personSID1);

            var serviceHelper = new CatiWsHelper(user1, password1);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            var stationId = string.Empty;

            var consoleDescriptor = new ConsoleDescription();

            serviceHelper.ConsoleService.Login(
                stationId,
                consoleDescriptor,
                out personInfo,
                out diallerInfo,
                out outProperties);

            BackendTools.LoginPerson(personSID1, "");

            Appointment[] appointments = serviceHelper.ConsoleService.GetAllAppointmentList();

            Assert.AreEqual(3, appointments.Length, "Wrong apoointments count");

            //first appointment for first survey and first interview
            Assert.AreEqual(2, appointments[0].id, "Wrong first apoointment id");
            Assert.AreEqual(1, appointments[0].appointmentTimeZone.Id, "Wrong first apoointment time zone id");
            Assert.AreEqual(now.AddDays(appointments[0].id), appointments[0].time, "Wrong first apoointment time");
            Assert.AreEqual("", appointments[0].projectName, "Wrong first apoointment project name");
            Assert.AreEqual(projectIDs[0], appointments[0].projectID, "Wrong first apoointment project id");

            //second appointment for third survey and first interview
            Assert.AreEqual(10, appointments[1].id, "Wrong second apoointment id");
            Assert.AreEqual(5, appointments[1].appointmentTimeZone.Id, "Wrong second apoointment time zone id");
            Assert.AreEqual(now.AddDays(appointments[1].id), appointments[1].time, "Wrong second apoointment time");
            Assert.AreEqual("", appointments[1].projectName, "Wrong second apoointment project name");
            Assert.AreEqual(projectIDs[2], appointments[1].projectID, "Wrong second apoointment project id");

            //third appointment for fivth survey and first interview
            Assert.AreEqual(18, appointments[2].id, "Wrong third apoointment id");
            Assert.AreEqual(9, appointments[2].appointmentTimeZone.Id, "Wrong third apoointment time zone id");
            Assert.AreEqual(now.AddDays(appointments[2].id), appointments[2].time, "Wrong third apoointment time");
            Assert.AreEqual("", appointments[2].projectName, "Wrong third apoointment project name");
            Assert.AreEqual(projectIDs[4], appointments[2].projectID, "Wrong third apoointment project id");
        }
        #endregion


        /// <summary>
        /// 1. Create person in manual mode
        /// 2. Login person in caticonsole
        /// 3. Call GetAllAppointmentList method of WS
        /// 
        /// We should get empty list
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GettingAppointmentList_GetAppointmentsForUserWithoutAppointments_EmptyListIsReturned()
        {
            const string user1 = "APerson1";
            const string password1 = "password1";

            PersonTools.CreatePerson(user1, password1, AgentTaskChoiceMode.Manual);

            var serviceHelper = new CatiWsHelper(user1, password1);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer outProperties;
            var stationId = string.Empty;

            var consoleDescriptor = new ConsoleDescription();

            serviceHelper.ConsoleService.Login(
                stationId,
                consoleDescriptor,
                out personInfo,
                out diallerInfo,
                out outProperties);

            Appointment[] appointments = serviceHelper.ConsoleService.GetAllAppointmentList();

            Assert.AreEqual(0, appointments.Length, "Wrong apoointments count");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        [Cr(60263)]
        public void AddingAppointment_AddAppointmentWithNullTimeznone_ReturnListOfAppointments()
        {
            BackendToolsObject.LaunchAllHoursScript();
            int surveyId = BackendToolsObject.CreateSurvey("p123");
            _surveyStateService.Open(surveyId);
            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);

            BackendTools.AddAppointment(interview.ID, surveyId, DateTime.UtcNow.AddDays(1), true);
            interview.TransientState = 1;
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false, opType = OperationType.MovedAndReschedule });
            var app = AppointmentRepository.GetById(surveyId, interview.ID);
            app.TZID = null;
            AppointmentRepository.InsertUpdate(app);

            var personId = PersonTools.CreatePerson("u1", "p1", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignResourceToInterview(surveyId, interview.ID, personId);

            PersonInfo pi;
            DiallerInfo di;
            CatiConsolePropertiesContainer ccp;
            var consoleDescriptor = new ConsoleDescription();

            new CatiWsHelper("u1", "p1").ConsoleService.Login("q", consoleDescriptor, out pi, out di, out ccp);
            var appt = new CatiWsHelper("u1", "p1").ConsoleService.GetAllAppointmentList();

            Assert.AreEqual(1, appt.Length);
        }
    }
}
