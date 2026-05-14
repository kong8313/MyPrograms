using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Mail.Fakes;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Survey.Fakes;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Core.UnitTests.Logger;
using MailMessage = Confirmit.CATI.Core.Mail.MailMessage;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;

namespace Confirmit.CATI.Core.UnitTests.Scheduling.SchedulingScriptNotificatorTest
{
    /// <summary>
    /// Summary description for SchedulingScriptNotificatorTest
    /// </summary>
    [TestClass]
    public class SchedulingScriptNotificatorTest : BaseTest
    {
        private const int Limit = 3;
        private const int InterviewId = 123;
        private int _sendMessageCount;
        private TestTraceListener _testTraceListener;
        private MailMessage _sentMessage;

        private void RegisterStubs(
            string surveyName = "surveyName123",
            string scheduleName = "scheduleName123",
            string email = "email@firmsw.no",
            string emailBcc = "emailbcc@firmsw.no")
        {
            var surveyServiceStub = new StubISurveyService
            {
                GetProjectIdWithNameInt32 = surveyId => surveyName
            };
            ServiceLocator.RegisterInstance<ISurveyService>(surveyServiceStub);

            _sendMessageCount = 0;
            var mailSenderStub = new StubIMailSender
            {
                SendMailMailMessage = message => { _sendMessageCount++; _sentMessage = message; }
            };
            ServiceLocator.RegisterInstance<IMailSender>(mailSenderStub);

            var scheduleRepositoryStub = new StubIScheduleRepository
            {
                GetByIdInt32 = surveyId => new BvScheduleEntity { Name = scheduleName }
            };
            ServiceLocator.RegisterInstance<IScheduleRepository>(scheduleRepositoryStub);


            var emailSettingsStub = new StubIEmailSettings
            {
                NotificationExceptionLimitGet = () => Limit,
                NotificationEmailBCCGet = () => emailBcc
            };
            ServiceLocator.RegisterInstance<IEmailSettings>(emailSettingsStub);

            var surveyRepositoryStub = new StubISurveyRepository
            {
                GetByIdInt32 = sid => new BvSurveyEntity { NotificationEmail = email }
            };
            ServiceLocator.RegisterInstance<ISurveyRepository>(surveyRepositoryStub);

            RegistryStub<ITimezoneService, StubITimezoneService>().GetDefaultCallCenterTimezoneId = () => 1;
        }

        private void RegisterTraceListener()
        {
            _testTraceListener = new TestTraceListener();
            Trace.Listeners.Add(_testTraceListener);
            TestTraceListener.TraceEventCount = 0;
        }

        private void UnregisterTraceListener()
        {
            Trace.Listeners.Remove(_testTraceListener);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void SchedulingScriptNotificator_NewInstance_ReturnInstance()
        {
            RegisterStubs();

            ServiceLocator.RegisterInstance<ISurveyConnectionStringProvider>(new StubISurveyConnectionStringProvider());

            ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

            Assert.IsNotNull(notificator, "A notificatior was not created.");
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void NotifyIfNeeded_BatchIdIsSpecified_NoMailSending()
        {
            try
            {
                RegisterTraceListener();
                RegisterStubs();
                ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

                notificator.NotifyIfNeeded(new Exception("Test"), 1, 2, 3, 4);

                Assert.AreEqual(0, _sendMessageCount, "Send message from NotifyIfNeeded method with speciified batch id");
                Assert.AreEqual(0, TestTraceListener.TraceEventCount, "Trace wasn't expected");
            }
            finally
            {
                UnregisterTraceListener();
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void Notify_NoExceptionsInList_NoMailSending()
        {
            try
            {
                RegisterTraceListener();
                RegisterStubs();
                ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

                notificator.Notify(new List<SchedulingScriptNotificatorExceptionDescription>(), 1, 2, 3);

                Assert.AreEqual(0, _sendMessageCount, "Email message was sent from NotifyIfNeeded method with speciified batch id");
                Assert.AreEqual(0, TestTraceListener.TraceEventCount, "Trace wasn't expected");
            }
            finally
            {
                UnregisterTraceListener();
            }
        }
       
        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void NotifyIfNeeded_BatchIdIsZeroAndValidException_ExceptionCountIncreased()
        {
            RegisterStubs();
            ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

            notificator.NotifyIfNeeded(new Exception("Test"), 0, 2, 3, 4);

            Assert.AreEqual(1, _sendMessageCount, "Email message wasn't sent from NotifyIfNeeded method with empty batch id");
        }
        
        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void Notify_NoEmails_EmailWasNotSentAndInformationTraced()
        {
            try
            {
                RegisterTraceListener();
                RegisterStubs();

                StubIEmailSettings schedulingScriptNotificatorStub = (StubIEmailSettings)ServiceLocator.Resolve<IEmailSettings>();
                schedulingScriptNotificatorStub.NotificationEmailBCCGet = () => string.Empty;

                StubISurveyRepository surveyRepositoryStub = (StubISurveyRepository)ServiceLocator.Resolve<ISurveyRepository>();
                surveyRepositoryStub.GetByIdInt32 = sid => new BvSurveyEntity { NotificationEmail = string.Empty };

                ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

                var exceptions = new List<SchedulingScriptNotificatorExceptionDescription>
                {
                    new SchedulingScriptNotificatorExceptionDescription(InterviewId, new Exception("Test"))
                };

                notificator.Notify(exceptions, 1, 2, 3);

                Assert.AreEqual(0, _sendMessageCount, "Email message was sent from NotifyIfNeeded method with empty exception list");
                Assert.AreEqual(1, TestTraceListener.TraceEventCount, "Trace didn't occured");
            }
            finally
            {
                UnregisterTraceListener();
            }
        }
        
        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void Notify_ValidParameters_MessageContainsCorrectData()
        {
            int testBatchId = 14134124;
            int testSurveyId = 72371239;
            string testSurveyName = "Test survey name";
            int testScheduleId = 902334891;
            string testScheduleName = "Test schedule name";
            string testEmail = "testEmail@firmsw.no";
            string testEmailBcc = "testEmailBcc@firmsw.no";

            RegisterStubs(testSurveyName, testScheduleName, testEmail, testEmailBcc);
            ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

            string testExceptionMessage = "Test exception message";
            int interviewId = 191230170;

            var exceptions = new List<SchedulingScriptNotificatorExceptionDescription>();

            int testExceptionTotalCount = 10;
            for (int i = 0; i < testExceptionTotalCount; i++)
            {
                exceptions.Add(new SchedulingScriptNotificatorExceptionDescription(interviewId + i, new Exception(testExceptionMessage)));
            }

            notificator.Notify(exceptions, testBatchId, testSurveyId, testScheduleId);

            Assert.IsNotNull(_sentMessage, "Message was not created.");
            StringAssert.Contains(_sentMessage.To.ToString(), testEmail, "Field 'To' contains wrong email");
            StringAssert.Contains(_sentMessage.Bcc.ToString(), testEmailBcc, "Field 'Bcc' contains wrong email");
            StringAssert.Contains(_sentMessage.Subject, testSurveyName, "Subject does not contain the survey name");
            StringAssert.Contains(_sentMessage.Subject, testScheduleName, "Subject does not contain the schedule name");

            StringAssert.Contains(_sentMessage.Body, testSurveyName, "Message body does not contain the survey name");
            StringAssert.Contains(_sentMessage.Body, testScheduleName, "Message body does not contain the schedule name");
            StringAssert.Contains(_sentMessage.Body, testBatchId.ToString(), "Message body does not contain the batch id");

            StringAssert.Contains(_sentMessage.Body, testExceptionTotalCount.ToString(), "Message body does not contain the total exception count number.");

            StringAssert.Contains(_sentMessage.Body, testExceptionMessage, "Message body does not contain the exception message");

            for (int i = 0; i < 3; i++)
            {
                StringAssert.Contains(_sentMessage.Body, (interviewId + i).ToString(), "The respondent ID from exception #{0} is missed in the message body.", i + 1);
            }

            for (int i = 3; i < testExceptionTotalCount; i++)
            {
                Assert.IsFalse(_sentMessage.Body.Contains((interviewId + i).ToString()), "The respondent ID from exception #{0} can't be exist in the message body.", i + 1);
            }
        }
        
        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void Notify_ValidToAddresses_MessageHasToAddresses()
        {
            RegisterStubs("1", "2", "test1@firmsw.no;test2@firmsw.no", "test1@firmsw.no;test2@firmsw.no, test3@firmsw.no");
            
            ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

            var exceptions = new List<SchedulingScriptNotificatorExceptionDescription>
            {
                new SchedulingScriptNotificatorExceptionDescription(InterviewId, new Exception("Test")),
                new SchedulingScriptNotificatorExceptionDescription(InterviewId, new Exception("Test"))
            };

            notificator.Notify(exceptions, 1, 2, 3);

            Assert.IsNotNull(_sentMessage, "Message was not created.");
            Assert.AreEqual(2, _sentMessage.To.Count, "E-mails were not parsed correctly.");
            Assert.AreEqual(3, _sentMessage.Bcc.Count, "E-mails BCC were not parsed correctly.");
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void Notify_TwoMessagesForZeroAndNonZeroBatchId_MessagesContainDifferentSubjects()
        {
            RegisterStubs();
            ISchedulingScriptNotificator notificator = ServiceLocator.Resolve<ISchedulingScriptNotificator>();

            var exceptions = new List<SchedulingScriptNotificatorExceptionDescription>
            {
                new SchedulingScriptNotificatorExceptionDescription(InterviewId, new Exception("Test")),
            };

            notificator.Notify(exceptions, 0, 1, 2);
            string message1Subject = _sentMessage.Subject;

            notificator.Notify(exceptions, 1, 1, 2);
            string message2Subject = _sentMessage.Subject;

            Assert.AreNotEqual(message1Subject, message2Subject, "Subjects for exception with zero batch id and non zero batch id are the same");
            Assert.IsTrue(message2Subject.Contains("sample"), "Subjects for exception with zero batch id should containt 'sample' word");
        }
    }
}
