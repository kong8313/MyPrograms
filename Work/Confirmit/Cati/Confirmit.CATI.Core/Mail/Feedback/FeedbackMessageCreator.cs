using System;
using System.Net.Mail;
using System.Text;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Mail.Feedback
{
    public class FeedbackMessageCreator : IFeedbackMessageCreator
    {
        private readonly IEmailSettings _emailSettings;

        public FeedbackMessageCreator(IEmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public MailMessage GetMailMessage(FeedbackForm form)
        {
            var mailMessage = new MailMessage
            {
                To = GetToEmails(form.Category),
                Subject = GetSubject(),
                Body = GetBody(form)
            };

            return mailMessage;
        }

        private string GetBody(FeedbackForm form)
        {
            var result = new StringBuilder();

            var category = "Category: {0}" + NewLine();
            var companyName = "Company name: \"{0}\"" + NewLine();
            var companyId = "Company Id: {0}" + NewLine();
            var authorizedUserLogin = "Authorized user login: {0}" + NewLine();
            var authorizedUserName = "Authorized user name: {0}" + NewLine();
            var authorizedUserEmail = "Authorized user email: {0}" + NewLine();
            var contactEmail = "Contact email: {0}" + NewLine();
            var contactName = "Contact name: {0}" + NewLine();
            var summary = "Summary:" + NewLine() + "{0}" + NewLine();
            var description = "Description:" + NewLine() + "{0}" + NewLine();
            var note = "NOTE: please do not reply to this email address as your reply will not be handled";

            result.AppendFormat(category, form.Category.ToString().ToLower());

            result.AppendLine();

            result.AppendFormat(companyId, form.CompanyId);
            result.AppendFormat(companyName, form.CompanyName);

            result.AppendLine();

            result.AppendFormat(authorizedUserLogin, form.AuthorizedUserLogin);
            result.AppendFormat(authorizedUserName, form.AuthorizedUserName);
            result.AppendFormat(authorizedUserEmail, form.AuthorizedUserEmail);


            if (!string.IsNullOrWhiteSpace(form.ContactName) || !string.IsNullOrWhiteSpace(form.ContactEmail))
            {
                result.AppendLine();

                if (!string.IsNullOrWhiteSpace(form.ContactName))
                {
                    result.AppendFormat(contactName, form.ContactName);
                }

                if (!string.IsNullOrWhiteSpace(form.ContactEmail))
                {
                    result.AppendFormat(contactEmail, form.ContactEmail);
                }    
            }

            result.Append(GetSeparetor());

            if (!string.IsNullOrWhiteSpace(form.Summary))
            {
                result.AppendFormat(summary, form.Summary);
                result.AppendLine();
            }

            result.AppendFormat(description, form.Description);
            result.Append(GetSeparetor());
            result.Append(note);

            return result.ToString();
        }

        private string GetSubject()
        {
            var result = "Horizons CATI user feedback";

            return result;
        }

        private MailAddressCollection GetToEmails(FeedbackCategory category)
        {
            var result = new MailAddressCollection { _emailSettings.NotificationEmailBCC };

            if (category == FeedbackCategory.Bug)
            {
                result.Add(_emailSettings.FeedbackSupportEmailAddress);
            }

            return result;
        }

        private MailAddressCollection GetBccEmails()
        {
            return new MailAddressCollection() { _emailSettings.NotificationEmailBCC };
        }

        private string GetSeparetor()
        {
            var result = NewLine() + "-------------------------" + NewLine() + NewLine();

            return result;
        }

        private string NewLine()
        {
            return Environment.NewLine;
        }
    }
}