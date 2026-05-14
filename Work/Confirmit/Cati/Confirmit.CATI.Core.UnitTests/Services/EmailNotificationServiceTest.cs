using System.Linq;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Mail.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SL = Confirmit.CATI.Common.ServiceLocation.ServiceLocator;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    [TestClass]
    public class EmailNotificationServiceTest
    {
        private const string ExpectedEmailAddress = "someemail@firmsw.no";
        private const string ExpectedSubject = "Subject subj subj";
        private const string ExpectedBody = "Message body. Something here.";

        private const string ExpectedAdministratorEmailAddress = "administrator@firmsw.no";
        private const string ExpectedNotificationEmailBcc = "bcc@firmsw.no";
        private const string ExpectedNotificationEmailNotificationEmailRecipients = "notification@firmsw.no";

        public TestContext TestContext { get; set; }

        private StubIMailSender _stubIMailSender;

        [TestInitialize]
        public void TestInitialize()
        {
            SL.StaticCleanup();
            SL.StaticInitialize();

            _stubIMailSender = new StubIMailSender();

            var stubIEmailSettings = new StubIEmailSettings
            {
                AdministratorEmailAddressGet = () => ExpectedAdministratorEmailAddress,
                NotificationEmailBCCGet = () => ExpectedNotificationEmailBcc,
                NotificationEmailRecipientsGet = () => ExpectedNotificationEmailNotificationEmailRecipients
            };

            SL.RegisterInstance<IEmailSettings>(stubIEmailSettings);
            SL.RegisterInstance<IMailSender>(_stubIMailSender);
            SL.Register<IEmailNotificationService, EmailNotificationService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            SL.StaticCleanup();
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendEmail_EmailAddressAndSubjectAndBodyAreCorrectAndBccIsEmpty()
        {
            var sendMailMethodIsCalled = false;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                Assert.AreEqual(ExpectedEmailAddress, message.To.ToString(),
                    "'To' address is not as expected");

                Assert.AreEqual(string.Empty, message.Bcc.ToString(),
                    "'Bcc' address is not as expected");

                Assert.AreEqual(ExpectedSubject, message.Subject,
                    "'Subject' field is not as expected");

                Assert.AreEqual(ExpectedBody, message.Body,
                    "'Body' field is not as expected");
            };

            var target = SL.Resolve<IEmailNotificationService>();

            target.SendEmail(ExpectedEmailAddress, ExpectedSubject, ExpectedBody);

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendDialerToAdministratorIsTrue_AdministratorEmailAddressAndNotificationEmailBccAreUsed()
        {
            var sendMailMethodIsCalled = false;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                Assert.AreEqual(ExpectedAdministratorEmailAddress, message.To.ToString(),
                    "'To' address is not as expected");

                Assert.AreEqual(ExpectedNotificationEmailBcc, message.Bcc.ToString(),
                    "'Bcc' address is not as expected");
            };

            var target = SL.Resolve<IEmailNotificationService>();

            target.SendEmail(true, "", "");

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SendDialerToAdministratorIsFalse_NotificationEmailRecipientsAndNotificationEmailBccAreUsed()
        {
            var sendMailMethodIsCalled = false;

            _stubIMailSender.SendMailMailMessage = message =>
            {
                sendMailMethodIsCalled = true;

                Assert.AreEqual(ExpectedAdministratorEmailAddress, message.To.ToString(),
                    "'To' address is not as expected");

                Assert.AreEqual(ExpectedNotificationEmailBcc, message.Bcc.ToString(),
                    "'Bcc' address is not as expected");
            };

            var target = SL.Resolve<IEmailNotificationService>();

            target.SendEmail(true, "", "");

            Assert.IsTrue(sendMailMethodIsCalled, "IMailSender.SendMail() method was expected to be called, but it has not been called.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ParseEmailString_3EmailsWithCommasAndSemicolons_Return3Emails()
        {
            const string input = "   1@firmsw.no;,, ;2@firmsw.no   ;,  ,3@firmsw.no,;;;";
            var expactedParsedEmails = new[] { "1@firmsw.no", "2@firmsw.no", "3@firmsw.no" };

            var target = SL.Resolve<IEmailNotificationService>();

            var actualParsedEmails = target.ParseEmailString(input).ToList();

            CollectionAssert.AreEqual(expactedParsedEmails, actualParsedEmails,
                string.Format("Parsed emails are not as expected. Expected: [{0}]. Actual: [{1}]",
                string.Join(", ", expactedParsedEmails), string.Join(", ", actualParsedEmails)));
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void ParseEmailString_NoEmails_ReturnEmptyList()
        {
            const string input = "   ;,, ;   ;,  ,,;;;";

            var target = SL.Resolve<IEmailNotificationService>();

            var actualCount = target.ParseEmailString(input).Count();

            Assert.AreEqual(0, actualCount, "Count of parsed emails is expected to be zero, but it is not");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CleanEmailStringTest()
        {
            const string input = "   1@firmsw.no;,, ;2@firmsw.no   ;,  ,3@firmsw.no,;;; a@firmsw.no ;   ;, bbb@firmsw.no ;,  ,,;;  eeeee@firmsw.no ;,  ,,  ";
            const string expectedCleanedEmailString = "1@firmsw.no;2@firmsw.no;3@firmsw.no;a@firmsw.no;bbb@firmsw.no;eeeee@firmsw.no";

            var target = SL.Resolve<IEmailNotificationService>();

            var actualCleanedEmailString = target.CleanEmailString(input);

            Assert.AreEqual(expectedCleanedEmailString, actualCleanedEmailString, "Cleaned email string is not as expected");
        }
    }
}