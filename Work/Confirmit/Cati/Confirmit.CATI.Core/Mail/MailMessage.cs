using System;
using System.Net.Mail;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Mail
{
    public class MailMessage
    {
        public MailMessage()
        {
            To  = new MailAddressCollection();
            Bcc = new MailAddressCollection();
        }

        public MailAddressCollection To { get; set; }
        public MailAddressCollection Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string BodyHtml { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentName { get; set; }

        public static string CombineBody(string text, DateTime date, string timezoneName)
        {
            return string.Format(
                "Date: {0}  {1} ({5}) \r\nCompany: {2} ({3}) \r\nPlease find attached the daily exported {4} export from Forsta CATI containing data for the last 24hrs, for all surveys.",
                date.ToLongDateString(),
                date.ToLongTimeString(),
                BackendInstance.Current.CompanyName,
                BackendInstance.Current.CompanyId,
                text,
                timezoneName);
        }

        public static string CombineBodyAttachmentTooLarge(string text, DateTime date, string timezoneName)
        {
            return string.Format(
                "Date: {0}  {1} ({5}) \r\nCompany: {2} ({3}) There was an error in generating {4} report, this may be caused by the email attachment being too large.",
                 date.ToLongDateString(),
                 date.ToLongTimeString(),
                BackendInstance.Current.CompanyName,
                BackendInstance.Current.CompanyId,
                text,
                timezoneName);
        }
    }
}
