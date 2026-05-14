using System.Threading.Tasks;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public interface ICatiBackendNotificationHandler
    {
        string NotificationTypeName { get; }
        Task HandleMessage(CatiBackendNotification message);
    }
}