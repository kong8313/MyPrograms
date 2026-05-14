using System;
using Confirmit.CATI.Core.Services;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIEmailNotificationService : IEmailNotificationService 
    {
        private IEmailNotificationService _inner;

        public StubIEmailNotificationService()
        {
            _inner = null;
        }

        public IEmailNotificationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendEmailBooleanStringStringStringDelegate(bool sendToAdministrator, string subject, string body, string bodyHtml);
        public SendEmailBooleanStringStringStringDelegate SendEmailBooleanStringStringString;

        void IEmailNotificationService.SendEmail(bool sendToAdministrator, string subject, string body, string bodyHtml)
        {

            if (SendEmailBooleanStringStringString != null)
            {
                SendEmailBooleanStringStringString(sendToAdministrator, subject, body, bodyHtml);
            } else if (_inner != null)
            {
                ((IEmailNotificationService)_inner).SendEmail(sendToAdministrator, subject, body, bodyHtml);
            }
        }

        public delegate void SendEmailStringStringStringStringDelegate(string recipientAddressTo, string subject, string body, string bodyHtml);
        public SendEmailStringStringStringStringDelegate SendEmailStringStringStringString;

        void IEmailNotificationService.SendEmail(string recipientAddressTo, string subject, string body, string bodyHtml)
        {

            if (SendEmailStringStringStringString != null)
            {
                SendEmailStringStringStringString(recipientAddressTo, subject, body, bodyHtml);
            } else if (_inner != null)
            {
                ((IEmailNotificationService)_inner).SendEmail(recipientAddressTo, subject, body, bodyHtml);
            }
        }

        public delegate IEnumerable<string> ParseEmailStringStringDelegate(string inputString);
        public ParseEmailStringStringDelegate ParseEmailStringString;

        IEnumerable<string> IEmailNotificationService.ParseEmailString(string inputString)
        {


            if (ParseEmailStringString != null)
            {
                return ParseEmailStringString(inputString);
            } else if (_inner != null)
            {
                return ((IEmailNotificationService)_inner).ParseEmailString(inputString);
            }

            return default(IEnumerable<string>);
        }

        public delegate string CleanEmailStringStringDelegate(string inputString);
        public CleanEmailStringStringDelegate CleanEmailStringString;

        string IEmailNotificationService.CleanEmailString(string inputString)
        {


            if (CleanEmailStringString != null)
            {
                return CleanEmailStringString(inputString);
            } else if (_inner != null)
            {
                return ((IEmailNotificationService)_inner).CleanEmailString(inputString);
            }

            return default(string);
        }

    }
}