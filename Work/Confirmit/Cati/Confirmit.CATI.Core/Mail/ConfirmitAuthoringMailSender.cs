using System;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Core.Mail
{
    public class ConfirmitAuthoringMailSender : IMailSender
    {
        private readonly IAsyncManager _asyncManager;
        private readonly IAuthoringService _authoringService;

        public ConfirmitAuthoringMailSender(
            IAsyncManager asyncManager,
            IAuthoringService authoringService)
        {
            _asyncManager = asyncManager;
            _authoringService = authoringService;
        }

        private static void CheckMessage(MailMessage message)
        {
            ParameterValidator.ValidateNotNull(message, "message");
            ParameterValidator.ValidateCondition(false, ((message.To == null || message.To.Count == 0) && (message.Bcc == null || message.Bcc.Count == 0)),
                                                 "Email was not sent. To or BCC should contain valid emails.");
            ParameterValidator.ValidateNotNullOrEmpty(message.Subject, "message.Subject");
            ParameterValidator.ValidateCondition(false, string.IsNullOrEmpty(message.Body) && string.IsNullOrEmpty(message.BodyHtml),
                "Body or BodyHtml cannot be empty at the same time");
        }

        private void ConfirmitAuthoringSendMail(MailMessage message)
        {
            _authoringService.SendMailHtml(
                message.To.Select(a => a.ToString()).ToArray(),
                message.Bcc != null && message.Bcc.Count > 0 ? message.Bcc.ToString() : null,
                message.Subject,
                message.Body,
                message.BodyHtml,
                message.Attachment,
                message.AttachmentName);

            Trace.TraceInformation(
                "Email '{2}' has been successfully sent to: {0}, bcc: {1}",
                message.To,
                message.Bcc,
                message.Subject);
        }

        private void SendMailProc(object param)
        {
            try
            {
                var message = param as MailMessage;
                ConfirmitAuthoringSendMail(message);
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e, "Email was not sent due to the error.");
            }
        }

        public void SendMail(MailMessage mailMessage)
        {
            CheckMessage(mailMessage);

            _asyncManager.QueueWorkItem(() => SendMailProc(mailMessage));
            Trace.TraceInformation(
                "Email '{2}' has been scheduled for sending: {0}, bcc: {1}",
                mailMessage.To,
                mailMessage.Bcc,
                mailMessage.Subject);
        }
    }
}