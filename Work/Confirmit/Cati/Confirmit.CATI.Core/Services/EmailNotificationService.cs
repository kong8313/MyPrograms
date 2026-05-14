using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Mail;
using Confirmit.CATI.Core.SystemSettings;
using MailMessage = Confirmit.CATI.Core.Mail.MailMessage;

namespace Confirmit.CATI.Core.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailSettings _emailSettings;
        private readonly IMailSender _mailSender;

        public EmailNotificationService(
            IEmailSettings emailSettings,
            IMailSender mailSender)
        {
            _emailSettings = emailSettings;
            _mailSender = mailSender;
        }

        public void SendEmail(bool sendToAdministrator, string subject, string body, string bodyHtml = null)
        {
            SendEmail(
                sendToAdministrator
                    ? _emailSettings.AdministratorEmailAddress
                    : _emailSettings.NotificationEmailRecipients,
                _emailSettings.NotificationEmailBCC,
                subject,
                body,
                bodyHtml);
        }

        public void SendEmail(string recipientAddressTo, string subject, string body, string bodyHtml = null)
        {
            SendEmail(recipientAddressTo, string.Empty, subject, body, bodyHtml);
        }

        private void SendEmail(string recipientAddressTo, string recipientAddressBcc, string subject, string body, string bodyHtml)
        {
            var mm = new MailMessage { Body = body, BodyHtml = bodyHtml, Subject = subject };

            if (!string.IsNullOrEmpty(recipientAddressTo))
            {
                ParseEmailString(recipientAddressTo).ToList().ForEach(r => AddMailAddress(mm.To, r));
                ParseEmailString(recipientAddressBcc).ToList().ForEach(r => AddMailAddress(mm.Bcc, r));
            }
            else if( !string.IsNullOrEmpty(recipientAddressBcc))
            {
                ParseEmailString(recipientAddressBcc).ToList().ForEach(r => AddMailAddress(mm.To, r));
            }

            if (mm.To.Count > 0)
            {
                _mailSender.SendMail(mm);
            }
        }

        private void AddMailAddress(MailAddressCollection mailCollection, string address)
        {
            try
            {
                mailCollection.Add(address);
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex, $"Cannot send email to address '{address}'");
            }
        }

        /// <summary>
        /// Gets emails from the string containing several emails separated by ',' and ';'
        /// </summary>
        /// <param name="inputString">String to parse</param>
        /// <returns>Collection of email strings</returns>
        public IEnumerable<string> ParseEmailString(string inputString)
        {
            return inputString
                .Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(y => !String.IsNullOrEmpty(y));
        }

        /// <summary>
        /// Cleans string with emails. Removes extra spaces and separators.
        /// </summary>
        /// <param name="inputString">String containing emails.</param>
        /// <returns>Well-formatted emails string.</returns>
        public string CleanEmailString(string inputString)
        {
            var emails = ParseEmailString(inputString).ToArray();
            return string.Join(";", emails);
        }
    }
}