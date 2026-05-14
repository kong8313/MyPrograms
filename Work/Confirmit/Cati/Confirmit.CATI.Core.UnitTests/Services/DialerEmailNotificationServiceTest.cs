using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Mail.Fakes;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ConfirmitServices = Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class DialerEmailNotificationServiceTest
    {
        private const int ExpectedDialerId = 1913189;
        private const string ExpectedDialerName = "Test Dialer Name 123";
        private const int ExpectedCompanyId = 32817810;
        private const string ExpectedCompanyName = "Test Company Name 456";

        private const string ExpectedNotificationEmailNotificationEmailRecipients = "notification@firmsw.no";
        private const string ExpectedNotificationEmailBcc = "bcc@firmsw.no";

        private const string ExpectedAlarms = "Alarm1; Alarm2; Alarm3";
        private const string ExpectedDateOfExpiration = "31.12.3017";

        public TestContext TestContext { get; set; }

        private StubIMailSender _stubIMailSender;

        [TestInitialize]
        public void TestInitialize()
        {
            ServiceLocator.StaticCleanup();
            ServiceLocator.StaticInitialize();

            _stubIMailSender = new StubIMailSender();

            var stubIEmailSettings = new StubIEmailSettings
            {
                NotificationEmailBCCGet = () => ExpectedNotificationEmailBcc,
                NotificationEmailRecipientsGet = () => ExpectedNotificationEmailNotificationEmailRecipients
            };

            var stubIDialersRepository = new StubIDialersRepository
            {
                GetByIdInt32 = id => new BvDialersEntity { Name = ExpectedDialerName }
            };

            var stubICompanyInfo = new StubICompanyInfo
            {
                CompanyIdGet = () => ExpectedCompanyId,
                CompanyNameGet = () => ExpectedCompanyName
            };

            ServiceLocator.RegisterInstance<IEmailSettings>(stubIEmailSettings);
            ServiceLocator.RegisterInstance<IMailSender>(_stubIMailSender);
            ServiceLocator.RegisterInstance<ICompanyInfo>(stubICompanyInfo);
            ServiceLocator.RegisterInstance<IDialersRepository>(stubIDialersRepository);

            ServiceLocator.Register<IEmailNotificationService, EmailNotificationService>();
            ServiceLocator.Register<IDialerEmailNotificationService, DialerEmailNotificationService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ServiceLocator.StaticCleanup();
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendDialerUnavailableEmailNotification_EmailMessageFieldsAreCorrect_MessegeDependsOnParams()
        {
            var sendMailMethodIsCalled = false;

            string actualTo = null;
            string actualBcc = null;
            string actualSubject = null;
            string actualBody = null;

            string withReconnectionBody = null;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                actualTo = message.To.ToString();
                actualBcc = message.Bcc.ToString();
                actualSubject = message.Subject;
                actualBody = message.Body;
            };

            var dialerEmailNotificationService = ServiceLocator.Resolve<IDialerEmailNotificationService>();

            dialerEmailNotificationService.SendDialerUnavailableEmailNotification(ExpectedDialerId, true);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");

            Assert.AreEqual(ExpectedNotificationEmailNotificationEmailRecipients, actualTo,
                "'To' address is not as expected");

            Assert.AreEqual(ExpectedNotificationEmailBcc, actualBcc,
                "'Bcc' address is not as expected");

            StringAssert.Contains(actualSubject, ExpectedDialerId.ToString(),
                "'Subject' field is expected to contain dialer id");
            StringAssert.Contains(actualSubject, ExpectedDialerName,
                "'Subject' field is expected to contain dialer name");

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");

            withReconnectionBody = actualBody;

            dialerEmailNotificationService.SendDialerUnavailableEmailNotification(ExpectedDialerId, false);

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");

            Assert.AreNotEqual(actualBody, withReconnectionBody, "'Body' field is expected to contain different text");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendDialerTrunkLinesAlarmsEmailNotification_EmailMessageFieldsAreCorrect()
        {
            var sendMailMethodIsCalled = false;

            string actualTo = null;
            string actualBcc = null;
            string actualSubject = null;
            string actualBody = null;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                actualTo = message.To.ToString();
                actualBcc = message.Bcc.ToString();
                actualSubject = message.Subject;
                actualBody = message.Body;
            };

            var dialerEmailNotificationService = ServiceLocator.Resolve<IDialerEmailNotificationService>();

            dialerEmailNotificationService.SendDialerTrunkLinesAlarmsEmailNotification(ExpectedDialerId, ExpectedAlarms);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");

            Assert.AreEqual(ExpectedNotificationEmailNotificationEmailRecipients, actualTo,
                "'To' address is not as expected");

            Assert.AreEqual(ExpectedNotificationEmailBcc, actualBcc,
                "'Bcc' address is not as expected");

            StringAssert.Contains(actualSubject, ExpectedDialerId.ToString(),
                "'Subject' field is expected to contain dialer id");
            StringAssert.Contains(actualSubject, ExpectedDialerName,
                "'Subject' field is expected to contain dialer name");

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");
            StringAssert.Contains(actualBody, ExpectedAlarms,
                "'Body' field is expected to contain alarms");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendDialerWsStartedEmailNotification_EmailMessageFieldsAreCorrect()
        {
            var sendMailMethodIsCalled = false;

            string actualTo = null;
            string actualBcc = null;
            string actualSubject = null;
            string actualBody = null;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                actualTo = message.To.ToString();
                actualBcc = message.Bcc.ToString();
                actualSubject = message.Subject;
                actualBody = message.Body;
            };

            var dialerEmailNotificationService = ServiceLocator.Resolve<IDialerEmailNotificationService>();

            dialerEmailNotificationService.SendDialerWsStartedEmailNotification(ExpectedDialerId);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");

            Assert.AreEqual(ExpectedNotificationEmailNotificationEmailRecipients, actualTo,
                "'To' address is not as expected");

            Assert.AreEqual(ExpectedNotificationEmailBcc, actualBcc,
                "'Bcc' address is not as expected");

            StringAssert.Contains(actualSubject, ExpectedDialerId.ToString(),
                "'Subject' field is expected to contain dialer id");
            StringAssert.Contains(actualSubject, ExpectedDialerName,
                "'Subject' field is expected to contain dialer name");

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendDialerLoggerProblemEmailNotification_EmailMessageFieldsAreCorrect()
        {
            var sendMailMethodIsCalled = false;

            string actualTo = null;
            string actualBcc = null;
            string actualSubject = null;
            string actualBody = null;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                actualTo = message.To.ToString();
                actualBcc = message.Bcc.ToString();
                actualSubject = message.Subject;
                actualBody = message.Body;
            };

            var dialerEmailNotificationService = ServiceLocator.Resolve<IDialerEmailNotificationService>();

            dialerEmailNotificationService.SendDialerLoggerProblemEmailNotification(ExpectedDialerId);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");

            Assert.AreEqual(ExpectedNotificationEmailNotificationEmailRecipients, actualTo,
                "'To' address is not as expected");

            Assert.AreEqual(ExpectedNotificationEmailBcc, actualBcc,
                "'Bcc' address is not as expected");

            StringAssert.Contains(actualSubject, ExpectedDialerId.ToString(),
                "'Subject' field is expected to contain dialer id");
            StringAssert.Contains(actualSubject, ExpectedDialerName,
                "'Subject' field is expected to contain dialer name");

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendDialerLicenseExpirationEmailNotification_EmailMessageFieldsAreCorrect()
        {
            var sendMailMethodIsCalled = false;

            string actualTo = null;
            string actualBcc = null;
            string actualSubject = null;
            string actualBody = null;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                actualTo = message.To.ToString();
                actualBcc = message.Bcc.ToString();
                actualSubject = message.Subject;
                actualBody = message.Body;
            };

            var dialerEmailNotificationService = ServiceLocator.Resolve<IDialerEmailNotificationService>();

            dialerEmailNotificationService.SendDialerLicenseExpirationEmailNotification(ExpectedDialerId, ExpectedDateOfExpiration);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");

            Assert.AreEqual(ExpectedNotificationEmailNotificationEmailRecipients, actualTo,
                "'To' address is not as expected");

            Assert.AreEqual(ExpectedNotificationEmailBcc, actualBcc,
                "'Bcc' address is not as expected");

            StringAssert.Contains(actualSubject, ExpectedDialerId.ToString(),
                "'Subject' field is expected to contain dialer id");
            StringAssert.Contains(actualSubject, ExpectedDialerName,
                "'Subject' field is expected to contain dialer name");

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");
            StringAssert.Contains(actualBody, ExpectedDateOfExpiration,
                "'Body' field is expected to contain date of expiration");
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void SendDialerStopReconnectingEmailNotification_EmailMessageFieldsAreCorrect()
        {
            var sendMailMethodIsCalled = false;

            string actualTo = null;
            string actualBcc = null;
            string actualSubject = null;
            string actualBody = null;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                actualTo = message.To.ToString();
                actualBcc = message.Bcc.ToString();
                actualSubject = message.Subject;
                actualBody = message.Body;
            };

            var dialerEmailNotificationService = ServiceLocator.Resolve<IDialerEmailNotificationService>();

            dialerEmailNotificationService.SendDialerStopReconnectingEmailNotification(ExpectedDialerId);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");

            Assert.AreEqual(ExpectedNotificationEmailNotificationEmailRecipients, actualTo,
                "'To' address is not as expected");

            Assert.AreEqual(ExpectedNotificationEmailBcc, actualBcc,
                "'Bcc' address is not as expected");

            StringAssert.Contains(actualSubject, ExpectedDialerId.ToString(),
                "'Subject' field is expected to contain dialer id");
            StringAssert.Contains(actualSubject, ExpectedDialerName,
                "'Subject' field is expected to contain dialer name");

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");
        }

        public void SendDialerAutoReconnectionEmailNotification_EmailMessageFieldsAreCorrect()
        {
            var sendMailMethodIsCalled = false;

            string actualTo = null;
            string actualBcc = null;
            string actualSubject = null;
            string actualBody = null;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                actualTo = message.To.ToString();
                actualBcc = message.Bcc.ToString();
                actualSubject = message.Subject;
                actualBody = message.Body;
            };

            var dialerEmailNotificationService = ServiceLocator.Resolve<IDialerEmailNotificationService>();

            dialerEmailNotificationService.SendDialerAutoReconnectionEmailNotification(ExpectedDialerId);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");

            Assert.AreEqual(ExpectedNotificationEmailNotificationEmailRecipients, actualTo,
                "'To' address is not as expected");

            Assert.AreEqual(ExpectedNotificationEmailBcc, actualBcc,
                "'Bcc' address is not as expected");

            StringAssert.Contains(actualSubject, ExpectedDialerId.ToString(),
                "'Subject' field is expected to contain dialer id");
            StringAssert.Contains(actualSubject, ExpectedDialerName,
                "'Subject' field is expected to contain dialer name");

            StringAssert.Contains(actualBody, ExpectedDialerId.ToString(),
                "'Body' field is expected to contain dialer id");
            StringAssert.Contains(actualBody, ExpectedDialerName,
                "'Body' field is expected to contain dialer name");
            StringAssert.Contains(actualBody, ExpectedCompanyId.ToString(),
                "'Body' field is expected to contain company id");
            StringAssert.Contains(actualBody, ExpectedCompanyName,
                "'Body' field is expected to contain company name");
        }
    }
}
