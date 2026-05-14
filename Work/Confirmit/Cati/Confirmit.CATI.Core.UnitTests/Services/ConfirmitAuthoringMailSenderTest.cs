using System;
using System.Diagnostics;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using MailMessage = Confirmit.CATI.Core.Mail.MailMessage;

namespace Confirmit.CATI.Core.UnitTests.Services
{
    /// <summary>
    /// Summary description for ConfirmitAuthoringMailSenderTest
    /// </summary>
    [TestClass]
    public class ConfirmitAuthoringMailSenderTest : BaseTest
    {
        private ConfirmitAuthoringMailSender CreateConfirmitAuthoringMailSender()
        {
            var asyncManager = ServiceLocator.Resolve<IAsyncManager>();
            var authoringService = ServiceLocator.Resolve<IAuthoringService>();
            Debug.WriteLine(authoringService.GetType());
            return new ConfirmitAuthoringMailSender(asyncManager, authoringService);
        }

        private MailMessage CreateMailMessage()
        {
            MailMessage message = new MailMessage
            {
                Body = "123",
                Subject = "123"
            };
            return message;
        }

        private MailMessage CreateMailMessage(string to)
        {
            var message = new MailMessage { To = new MailAddressCollection { to } };
            return message;
        }

        private MailMessage CreateMailMessage(string to, string subject, string body, string bodyHtml)
        {
            var message = new MailMessage { 
                To = new MailAddressCollection { to }, Subject = subject, Body = body, BodyHtml = bodyHtml };
            return message;
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        public void SendMail_Constructor_ObjectCreated()
        {
            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            Assert.IsNotNull(target);
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendMail_NullMessage_ThrowException()
        {
            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            target.SendMail(null);
        }


        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(UserMessageException))]
        public void SendMail_EmptyRecipients_ThrowException()
        {
            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            MailMessage message = CreateMailMessage();
            target.SendMail(message);

        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(ArgumentException))]
        public void SendMail_EmptySubject_ThrowException()
        {
            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            MailMessage message = CreateMailMessage("nonexistingaddressto@firmsw.no");
            message.Body = "Body";
            target.SendMail(message);
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(UserMessageException))]
        public void SendMail_EmptyBodyAndBodyHtml_ThrowException()
        {
            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            MailMessage message = CreateMailMessage("nonexistingaddressto@firmsw.no");
            message.Subject = "Subject";
            target.SendMail(message);
        }


        [TestMethod(), Owner(@"FIRM\SergeyL")]
        public void SendMail_ValidMessageWithBody_MessageIsSent()
        {
            bool sendMailCalled = false;

            var stubAsyncManager = new StubIAsyncManager
            {
                QueueWorkItemAction = action => { action(); }
            };

            var originalIAuthoring = ServiceLocator.Resolve<IAuthoringService>();
            var stubIAuthoring = new StubIAuthoringService
            {
                Inner = originalIAuthoring,
                SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteString =
                    (to, bcc, subject, body, bodyHtml, attachment, name) => { sendMailCalled = true; }
            };

            ServiceLocator.RegisterInstance<IAsyncManager>(stubAsyncManager);
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoring);

            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            MailMessage message = CreateMailMessage("nonexistingaddressto@firmsw.no", "subject", "body", null);

            target.SendMail(message);

            Assert.IsTrue(sendMailCalled);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void SendMail_ValidMessageWithBodyHtml_MessageIsSent()
        {
            bool sendMailCalled = false;

            var stubAsyncManager = new StubIAsyncManager
            {
                QueueWorkItemAction = action => { action(); }
            };

            var originalIAuthoring = ServiceLocator.Resolve<IAuthoringService>();
            var stubIAuthoring = new StubIAuthoringService
            {
                Inner = originalIAuthoring,
                SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteString =
                    (to, bcc, subject, body, bodyHtml, attachment, name) => { sendMailCalled = true; }
            };

            ServiceLocator.RegisterInstance<IAsyncManager>(stubAsyncManager);
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoring);

            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            MailMessage message = CreateMailMessage("nonexistingaddressto@firmsw.no", "subject", null, "bodyHtml");

            target.SendMail(message);

            Assert.IsTrue(sendMailCalled);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void SendMail_ValidMessageWithBodyAndBodyHtml_MessageIsSent()
        {
            bool sendMailCalled = false;

            var stubAsyncManager = new StubIAsyncManager
            {
                QueueWorkItemAction = action => { action(); }
            };

            var originalIAuthoring = ServiceLocator.Resolve<IAuthoringService>();
            var stubIAuthoring = new StubIAuthoringService
            {
                Inner = originalIAuthoring,
                SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteString =
                    (to, bcc, subject, body, bodyHtml, attachment, name) => { sendMailCalled = true; }
            };

            ServiceLocator.RegisterInstance<IAsyncManager>(stubAsyncManager);
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoring);

            ConfirmitAuthoringMailSender target = CreateConfirmitAuthoringMailSender();
            MailMessage message = CreateMailMessage("nonexistingaddressto@firmsw.no", "subject", "body", "bodyHtml");

            target.SendMail(message);

            Assert.IsTrue(sendMailCalled);
        }
    }
}
