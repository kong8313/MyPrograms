using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.CleaningService;
using Confirmit.CATI.Core.Services.CleaningService.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Confirmit.CATI.IntegrationTests.Tests.SurveyCleaningProcedure
{
    [TestClass]
    public class SurveyCleaningProcedureTests : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void OpenedSurveyButWasNotTouchedDuringLongTime_CallAutoCleanup_SurveyWasNotCleaned()
        {
            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();
            _surveyStateService.Open(surveyId);
            
            var lastTouch = ExpireLastTouchTimeToCleanup(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(0, notification.Count);
            CheckSurveyCleanupState(surveyId, false);
            CheckSurveyLastTouch(surveyId, lastTouch);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ClosedSurveyWhichWasNotTouchedDuringLongTime_CallAutoCleanup_NotificationAboutPlanedCleanupWasSentToAdminAndOwner()
        {
            const string adminEmail = "asd@firmsw.no";
            ServiceLocator.Resolve<ISystemSettings>().Email.AdministratorEmailAddress = adminEmail;

            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();
            var lastTouch = ExpireLastTouchTimeToCleanup(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(2, notification.Count);
            Assert.AreEqual(adminEmail, notification[0].To[0].Address);
            Assert.AreEqual(SurveyRepository.GetById(surveyId).NotificationEmail, notification[1].To[0].Address);
            CheckSurveyNotificationCount(surveyId, 1);
            CheckSurveyCleanupState(surveyId, false);
            CheckSurveyLastTouch(surveyId, lastTouch);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void SurveyWhichReadyToCleanupButInNotificationState_CallAutoCleanup_SurveyIsNotCleaned()
        {
            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();
            var lastTouch = ExpireLastTouchTimeToNotification(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(1, notification.Count);
            CheckSurveyNotificationCount(surveyId, 1);
            CheckSurveyCleanupState(surveyId, false);
            CheckSurveyLastTouch(surveyId, lastTouch);

            lastTouch = ExpireLastTouchTimeToNotification(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(1, notification.Count);
            CheckSurveyNotificationCount(surveyId, 1);
            CheckSurveyCleanupState(surveyId, false);
            CheckSurveyLastTouch(surveyId, lastTouch);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void SurveyWhichReadyToCleanup_CallAutoCleanup_SurveyIsCleaned()
        {
            const string adminEmail = "asd@firmsw.no";
            ServiceLocator.Resolve<ISystemSettings>().Email.AdministratorEmailAddress = adminEmail;

            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();
            var lastTouch = ExpireLastTouchTimeToNotification(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(2, notification.Count);
            CheckSurveyNotificationCount(surveyId, 1);
            CheckSurveyCleanupState(surveyId, false);
            CheckSurveyLastTouch(surveyId, lastTouch);
            
            ExpireLastTouchTimeToCleanup(surveyId);
            ExpireNotificationSentTimeToCleanup(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(4, notification.Count);

            Assert.AreEqual(adminEmail, notification[2].To[0].Address);
            Assert.AreEqual(SurveyRepository.GetById(surveyId).NotificationEmail, notification[3].To[0].Address);

            CheckSurveyNotificationCount(surveyId, 2);
            CheckSurveyCleanupState(surveyId, true);
            CheckSurveyLastTouch(surveyId, DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void ThreeSurveysWhichAreReadyToCleanup_CallAutoCleanup_CorrectMailCountAreSentToCorrectAddresses()
        {
            const string adminEmail = "asd@firmsw.no";
            ServiceLocator.Resolve<ISystemSettings>().Email.AdministratorEmailAddress = adminEmail;

            var notification = MockNotification();

            var surveyId1 = CreateTestSurveyWithData("p001");
            ExpireLastTouchTimeToNotification(surveyId1);

            var surveyId2 = CreateTestSurveyWithData("p002");
            ExpireLastTouchTimeToNotification(surveyId2);

            var surveyId3 = CreateTestSurveyWithData("p003");
            ExpireLastTouchTimeToNotification(surveyId3);

            var survey1 = SurveyRepository.GetById(surveyId1);
            var survey2 = SurveyRepository.GetById(surveyId2);
            var survey3 = SurveyRepository.GetById(surveyId3);

            survey2.NotificationEmail = survey1.NotificationEmail;
            SurveyRepository.Update(survey2);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            // 1 mail to admin with all surveys
            // 1 mail to owner of survey1 and survey2 because they have the same NotificationEmail
            // 1 mail to owner of survey3
            Assert.AreEqual(3, notification.Count);

            Assert.AreEqual(adminEmail, notification[0].To[0].Address);
            Assert.IsTrue(notification[0].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsTrue(notification[0].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsTrue(notification[0].BodyHtml.Contains(survey3.ProjectId));

            Assert.AreEqual(survey1.NotificationEmail, notification[1].To[0].Address);
            Assert.IsTrue(notification[1].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsTrue(notification[1].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsFalse(notification[1].BodyHtml.Contains(survey3.ProjectId));

            Assert.AreEqual(survey3.NotificationEmail, notification[2].To[0].Address);
            Assert.IsFalse(notification[2].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsFalse(notification[2].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsTrue(notification[2].BodyHtml.Contains(survey3.ProjectId));

            ExpireLastTouchTimeToCleanup(surveyId1);
            ExpireLastTouchTimeToCleanup(surveyId2);
            ExpireLastTouchTimeToCleanup(surveyId3);

            ExpireNotificationSentTimeToCleanup(surveyId1);
            ExpireNotificationSentTimeToCleanup(surveyId2);
            ExpireNotificationSentTimeToCleanup(surveyId3);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(6, notification.Count);

            Assert.AreEqual(adminEmail, notification[3].To[0].Address);
            Assert.IsTrue(notification[3].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsTrue(notification[3].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsTrue(notification[3].BodyHtml.Contains(survey3.ProjectId));

            Assert.AreEqual(survey1.NotificationEmail, notification[4].To[0].Address);
            Assert.IsTrue(notification[4].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsTrue(notification[4].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsFalse(notification[4].BodyHtml.Contains(survey3.ProjectId));

            Assert.AreEqual(survey3.NotificationEmail, notification[5].To[0].Address);
            Assert.IsFalse(notification[5].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsFalse(notification[5].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsTrue(notification[5].BodyHtml.Contains(survey3.ProjectId));
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void ThreeSurveysWithEmptyAndWrongNotificationEmails_CallAutoCleanup_CorrectMailsWereSentAndAllWorksWithoutExceptions()
        {
            const string adminEmail = "asd@firmsw.no";
            ServiceLocator.Resolve<ISystemSettings>().Email.AdministratorEmailAddress = adminEmail;

            var notification = MockNotification();

            var surveyId1 = CreateTestSurveyWithData("p001");
            ExpireLastTouchTimeToNotification(surveyId1);

            var surveyId2 = CreateTestSurveyWithData("p002");
            ExpireLastTouchTimeToNotification(surveyId2);

            var surveyId3 = CreateTestSurveyWithData("p003");
            ExpireLastTouchTimeToNotification(surveyId3);

            var survey1 = SurveyRepository.GetById(surveyId1);
            var survey2 = SurveyRepository.GetById(surveyId2);
            var survey3 = SurveyRepository.GetById(surveyId3);

            survey1.NotificationEmail = string.Empty;
            survey2.NotificationEmail = "Wrong@firmsw.noEmail@firmsw.no";
            var correctSurvey3NotificationEmail = survey3.NotificationEmail;
            survey3.NotificationEmail = survey3.NotificationEmail + ",Wrong@firmsw.noEmail@firmsw.no";
            SurveyRepository.Update(survey1);
            SurveyRepository.Update(survey2);
            SurveyRepository.Update(survey3);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            // 1 mail to admin with all surveys
            // 1 mail to a correct address of survey3
            Assert.AreEqual(2, notification.Count);

            Assert.AreEqual(adminEmail, notification[0].To[0].Address);
            Assert.IsTrue(notification[0].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsTrue(notification[0].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsTrue(notification[0].BodyHtml.Contains(survey3.ProjectId));

            Assert.AreEqual(correctSurvey3NotificationEmail, notification[1].To[0].Address);
            Assert.IsTrue(notification[1].BodyHtml.Contains(survey3.ProjectId));

            ExpireLastTouchTimeToCleanup(surveyId1);
            ExpireLastTouchTimeToCleanup(surveyId2);
            ExpireLastTouchTimeToCleanup(surveyId3);

            ExpireNotificationSentTimeToCleanup(surveyId1);
            ExpireNotificationSentTimeToCleanup(surveyId2);
            ExpireNotificationSentTimeToCleanup(surveyId3);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(4, notification.Count);

            Assert.AreEqual(adminEmail, notification[2].To[0].Address);
            Assert.IsTrue(notification[2].BodyHtml.Contains(survey1.ProjectId));
            Assert.IsTrue(notification[2].BodyHtml.Contains(survey2.ProjectId));
            Assert.IsTrue(notification[2].BodyHtml.Contains(survey3.ProjectId));

            Assert.AreEqual(correctSurvey3NotificationEmail, notification[3].To[0].Address);
            Assert.IsTrue(notification[3].BodyHtml.Contains(survey3.ProjectId));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void ClosedSurveyButWasTouchedDuringNotificationTime_CallAutoCleanup_SurveyWasNotCleaned()
        {
            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            Assert.AreEqual(0, notification.Count);
            CheckSurveyCleanupState(surveyId, false);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void SurveyWasNotTouchedDuringLongTime_OpenSurvey_LastTouchTimeWasUpdate()
        {
            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();
            ExpireLastTouchTimeToCleanup(surveyId);
            _surveyStateService.Open(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            CheckSurveyLastTouch(surveyId, DateTime.UtcNow, TimeSpan.FromMinutes(1));
            Assert.AreEqual(0, notification.Count);
            CheckSurveyCleanupState(surveyId, false);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void SurveyWasNotTouchedDuringLongTime_CloseSurvey_LastTouchTimeWasUpdate()
        {
            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();
            _surveyStateService.Open(surveyId);
            ExpireLastTouchTimeToNotification(surveyId);
            _surveyStateService.CloseSurvey(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            CheckSurveyLastTouch(surveyId, DateTime.UtcNow, TimeSpan.FromMinutes(1));
            Assert.AreEqual(0, notification.Count);
            CheckSurveyCleanupState(surveyId, false);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void SurveyWasNotTouchedDuringLongTime_ShutdownSurvey_LastTouchTimeWasUpdate()
        {
            var notification = MockNotification();

            var surveyId = CreateTestSurveyWithData();
            _surveyStateService.Open(surveyId);
            ExpireLastTouchTimeToCleanup(surveyId);
            _surveyStateService.ShutdownSurvey(surveyId);

            ServiceLocator.Resolve<ISurveyCleaningService>().CleanAllUnusedSurveys();

            CheckSurveyLastTouch(surveyId, DateTime.UtcNow, TimeSpan.FromMinutes(1));
            Assert.AreEqual(0, notification.Count);
            CheckSurveyCleanupState(surveyId, false);
        }

        private void CheckSurveyLastTouch(int surveyId, DateTime time)
        {
            CheckSurveyLastTouch(surveyId, time, TimeSpan.Zero);
        }

        private void CheckSurveyLastTouch(int surveyId, DateTime time, TimeSpan accuracy)
        {
            var survey = SurveyRepository.GetById(surveyId);
            if (!survey.LastTouchTime.HasValue)
            {
                Assert.Fail("Wrong LastTouchTime value");
                return;
            }

            var lastTouch = survey.LastTouchTime.Value;
            var diff = lastTouch - time;
            if( diff < TimeSpan.Zero)
            {
                diff = diff.Negate();
            }

            //note: last touch have smalldatetime type in DB, so we should take into account that accuracy of this type is 1 minutes.
            Assert.IsTrue(diff < TimeSpan.FromMinutes(1) + accuracy, $"LastTouchTime = {lastTouch} <> {time}");
        }

        public List<MailMessage> MockNotification()
        {
            var result = new List<MailMessage>();

            var stubIAuthoringService = new StubIAuthoringService 
            {
                SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteString = (addressesTo, addressBcc, messageSubject, messageBody, messageBodyHtml, attachment, attachmentName) => 
                {
                    var mm = new MailMessage { Body = messageBody, BodyHtml = messageBodyHtml, Subject = messageSubject, Attachment = attachment , AttachmentName = attachmentName };

                    addressesTo.ToList().ForEach(x=>mm.To.Add(x));
                    if (!string.IsNullOrEmpty(addressBcc))
                    {
                        mm.Bcc.Add(addressBcc);
                    }

                    result.Add(mm); 
                }
            };
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoringService);
            
            var stubISurveyCleaningConfirmitDataAccess = new StubISurveyCleaningConfirmitDataAccess
            {
                SetCreatorsListOfCleaningServiceEmailInfo = surveys => 
                    { surveys.ForEach(x=> x.Creator = "Creator Name"); }
            };
            ServiceLocator.RegisterInstance<ISurveyCleaningConfirmitDataAccess>(stubISurveyCleaningConfirmitDataAccess);
            
            return result;
        }

        private void CheckSurveyNotificationCount(int surveyId, int count)
        {
            var items = GetNotifications(surveyId);

            Assert.AreEqual(count, items.Count);
        }

        private void CheckSurveyCleanupState(int surveyId, bool isCleaned)
        {
            var callsCount = BvSvyScheduleAdapter.GetByCondition("SurveySID = @SurveySID", new SqlParameter("@SurveySID", surveyId)).Count;
            var assignmentCount = AssignmentService.GetSurveyAssignedPersons(surveyId, (int)Role.Interviewer, CallCenterTools.DefaultId).Count;
            var callHistoryCount = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveySID",new SqlParameter("@SurveySID", surveyId)).Count;
            var loginGroupsCount = BvLoginGroupAdapter.GetByCondition("SurveySID = @SurveySID", new SqlParameter("@SurveySID", surveyId)).Count;
            
            if( isCleaned)
            {
                Assert.AreEqual(0, callsCount, "Survey should be cleaned, but not all interviews were deleted");
                Assert.AreEqual(0, assignmentCount, "Survey should be cleaned, but not all assignments were deleted");
                Assert.AreEqual(0, callHistoryCount, "Survey should be cleaned, but not all call history records were deleted");
                Assert.AreEqual(0, loginGroupsCount, "Survey should be cleaned, but not all bvlogingroup records were deleted");
            }
            else
            {
                Assert.AreNotEqual(0, callsCount, "Survey should not be cleaned, but all interviews were deleted");
                Assert.AreNotEqual(0, assignmentCount, "Survey should not be cleaned, but all assignment were deleted");
                Assert.AreNotEqual(0, callHistoryCount, "Survey should not be cleaned, but all call history records were deleted");
                Assert.AreNotEqual(0, loginGroupsCount, "Survey should not be cleaned, but all BvLoginGroup records were deleted");
            }
        }

        private void ExpireNotificationSentTimeToCleanup(int surveyId)
        {
            var settings = ServiceLocator.Resolve<ISystemSettings>().RoutineMaintenance.Actions.SurveyCleanup;

            BvUserNotificationAdapter.GetAll().Where( x=> x.ObjectId == surveyId ).ToList().ForEach(x =>
                {
                    x.SendDate = DateTime.UtcNow - settings.CleanupTimeout - TimeSpan.FromHours(1);
                    BvUserNotificationAdapter.UpdateByCondition(x, "Id=@NId", new SqlParameter("@NId", x.Id));
                });
        }

        private DateTime ExpireLastTouchTimeToCleanup(int surveyId)
        {
            var settings = ServiceLocator.Resolve<ISystemSettings>().RoutineMaintenance.Actions.SurveyCleanup;

            return ExpireSurveyLastTouchTime(surveyId, settings.NotificationTimeout + settings.CleanupTimeout + TimeSpan.FromDays(1));
        }

        private DateTime ExpireLastTouchTimeToNotification(int surveyId)
        {
            var settings = ServiceLocator.Resolve<ISystemSettings>().RoutineMaintenance.Actions.SurveyCleanup;

            return ExpireSurveyLastTouchTime(surveyId, settings.NotificationTimeout + TimeSpan.FromSeconds(settings.CleanupTimeout.TotalSeconds / 2));
        }

        private DateTime ExpireSurveyLastTouchTime(int surveyId, TimeSpan expiredPeriod)
        {
            var survey = SurveyRepository.GetById(surveyId);
            var lastTouchTime = DateTime.UtcNow - expiredPeriod;

            survey.LastTouchTime = lastTouchTime;
            SurveyRepository.Update(survey);

            return lastTouchTime;
        }

        private int CreateTestSurveyWithData(string name = null)
        {
            var surveyId = BackendToolsObject.CreateSurvey(name);
            var personId = PersonTools.CreatePerson("p" + name, "pwd", AgentTaskChoiceMode.Manual);

            var survey = SurveyRepository.GetById(surveyId);
            survey.NotificationEmail = $"e{surveyId}@firmsw.no";
            SurveyRepository.Update(survey);

            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);
            BackendTools.CreateInterviewWithCall(surveyId);
            BvCallHistoryExAdapter.Insert(new BvCallHistoryExEntity() { SurveyId = surveyId, FiredTime = DateTime.Now});
            BvLoginGroupAdapter.Insert(new BvLoginGroupEntity() { SurveySID = surveyId, PersonSID = personId });
            
            return surveyId;

        }

        private static List<BvUserNotificationEntity> GetNotifications(int surveyId)
        {
            var items = BvUserNotificationAdapter.GetByCondition(
                "ObjectId = @ObjectId", new SqlParameter("@ObjectId", surveyId));
            return items;
        }
    }
}
