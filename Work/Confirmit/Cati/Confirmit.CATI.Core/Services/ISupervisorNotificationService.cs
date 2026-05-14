namespace Confirmit.CATI.Core.Services
{
    public interface ISupervisorNotificationService
    {
        void SendAccountLockedEmailNotification(string supervisorAddressTo, string personLogin);
    }
}