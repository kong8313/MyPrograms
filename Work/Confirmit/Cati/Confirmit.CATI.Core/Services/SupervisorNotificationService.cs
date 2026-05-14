using Confirmit.CATI.Core.Resources;

namespace Confirmit.CATI.Core.Services
{
    public class SupervisorNotificationService : ISupervisorNotificationService
    {
        private readonly IEmailNotificationService _emailNotificationService;

        public SupervisorNotificationService(IEmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        public void SendAccountLockedEmailNotification(string supervisorAddressTo, string personLogin)
        {
            var subject = Strings.PersonLockOutEmailSubject;
            var body = string.Format(Strings.PersonLockOutEmailBody, personLogin);

            _emailNotificationService.SendEmail(supervisorAddressTo, subject, body);
        }
    }
}
