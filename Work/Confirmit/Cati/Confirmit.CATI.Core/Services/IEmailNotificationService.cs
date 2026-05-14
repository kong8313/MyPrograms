using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services
{
    public interface IEmailNotificationService
    {
        void SendEmail(bool sendToAdministrator, string subject, string body, string bodyHtml = null);
        void SendEmail(string recipientAddressTo, string subject, string body, string bodyHtml = null);
        IEnumerable<string> ParseEmailString(string inputString);
        string CleanEmailString(string inputString);
    }
}