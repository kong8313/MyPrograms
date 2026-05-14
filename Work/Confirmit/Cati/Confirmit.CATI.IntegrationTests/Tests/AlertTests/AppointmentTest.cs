using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Activity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.AlertTests
{
    [TestClass]
    public class AppointmentTest
    {
        readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        public class AppointmentInfo
        {
            public TimeSpan TimeOffset;
            public int State;
            public int Resource;
            public int Alert;

            /*test data*/
            public BvAppointmentEntity Appointment;
            public BvInterviewEntity Interview;
            public BvCallEntity Call;
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RecalculateAppointment_SurveyWithoutAppointments_ResultAreCorrect()
        {
            var data = new AppointmentInfo[] {};
            TestBase(data, 5, 10);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RecalculateAppointment_SurveyWithoutAppointmentsInActiveState_ResultAreCorrect()
        {
            var data = new AppointmentInfo[]
                       {
                           new AppointmentInfo(){ Alert = 0, State = 2, TimeOffset = -TimeSpan.FromMinutes(12)},
                           new AppointmentInfo(){ Alert = 0, State = 0, TimeOffset = -TimeSpan.FromMinutes(12)}
                       };
            TestBase(data, 5, 10);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void RecalculateAppointment_SurveyWithAppointmentsInActiveState_ResultAreCorrect()
        {
            var data = new AppointmentInfo[]
                       {
                           new AppointmentInfo(){ Alert = 0, State = 2, TimeOffset = -TimeSpan.FromMinutes(12)},
                           new AppointmentInfo(){ Alert = 2, State = 1, TimeOffset = -TimeSpan.FromMinutes(12)},
                           
                           new AppointmentInfo(){ Alert = 0, State = 2, TimeOffset = -TimeSpan.FromMinutes(7)},
                           new AppointmentInfo(){ Alert = 0, State = 1, TimeOffset = -TimeSpan.FromMinutes(7)},

                           new AppointmentInfo(){ Alert = 0, State = 2, TimeOffset = -TimeSpan.FromMinutes(3)},
                           new AppointmentInfo(){ Alert = 0, State = 1, TimeOffset = -TimeSpan.FromMinutes(3)},

                           new AppointmentInfo(){ Alert = 0, State = 2, TimeOffset = +TimeSpan.FromMinutes(3)},
                           new AppointmentInfo(){ Alert = 1, State = 1, TimeOffset = +TimeSpan.FromMinutes(3)},
                       
                           new AppointmentInfo(){ Alert = 0, State = 2, TimeOffset = +TimeSpan.FromMinutes(7)},
                           new AppointmentInfo(){ Alert = 0, State = 1, TimeOffset = +TimeSpan.FromMinutes(7)},
                        };
            TestBase(data, 5, 10);
            
            var statuses = BvSpGetAppointmentActivityExtStatusesAdapter.ExecuteEntityList();
            Assert.AreEqual(1, statuses.Count);
            Assert.AreEqual(16,statuses[0].ExtendedStatusId);
        }

        private void TestBase(AppointmentInfo[] data, int amber, int red)
        {
            var surveyId = _backendTools.CreateSurvey(null, false);
            _surveyStateService.Open(surveyId);
            foreach (var info in data)
            {
                info.Interview = BackendTools.NewInterview(surveyId);
                info.Call = BackendTools.NewCall(info.Interview);
                info.Appointment = BackendTools.NewAppointment(info.Interview);
                
                info.Call.Resource = info.Resource;
                
                info.Appointment.Time = DateTime.UtcNow + info.TimeOffset;
                info.Appointment.State = info.State;

                BackendTools.CreateInterview(info.Interview);
                BackendTools.CreateCall(info.Call);
                BackendTools.CreateAppointment(info.Appointment);
            }

            ActivityManager.SetAppointmentAlert(amber, red);
            BvSpAlert_RecalculateAppointmentAdapter.ExecuteNonQuery(0, 0, 1);

            using (var batch = TransferBatch.Create())
            {
                batch.Insert(new [] { surveyId});

                var result = BvSpGetAppointmentActivityAdapter.ExecuteEntityList(batch.Value, null);
                var id2appt = data.Where(x => x.Alert != 0 ).ToDictionary(k => k.Appointment.InterviewSID);
            
                Assert.AreEqual(id2appt.Count, result.Count);
                foreach (var actual in result)
                {
                    var expected = id2appt[(int)actual.InterviewID];
                    Assert.AreEqual(expected.Appointment.InterviewSID, actual.InterviewID);
                    Assert.AreEqual(expected.Appointment.SurveySID, actual.SurveySID);
                    Assert.AreEqual(expected.Alert, actual.AlertStatus);
                    Assert.AreEqual("Fresh sample", actual.ExtendedStatusName);
                }
            }
        }
    }
}
