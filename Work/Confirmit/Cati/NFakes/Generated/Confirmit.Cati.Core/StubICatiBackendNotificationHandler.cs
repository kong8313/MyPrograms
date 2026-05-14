using System;
using Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification.Fakes
{
    public class StubICatiBackendNotificationHandler : ICatiBackendNotificationHandler 
    {
        private ICatiBackendNotificationHandler _inner;

        public StubICatiBackendNotificationHandler()
        {
            _inner = null;
        }

        public ICatiBackendNotificationHandler Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task HandleMessageCatiBackendNotificationDelegate(CatiBackendNotification message);
        public HandleMessageCatiBackendNotificationDelegate HandleMessageCatiBackendNotification;

        Task ICatiBackendNotificationHandler.HandleMessage(CatiBackendNotification message)
        {


            if (HandleMessageCatiBackendNotification != null)
            {
                return HandleMessageCatiBackendNotification(message);
            } else if (_inner != null)
            {
                return ((ICatiBackendNotificationHandler)_inner).HandleMessage(message);
            }

            return default(Task);
        }

        private string _NotificationTypeName;
        public Func<string> NotificationTypeNameGet;
        public Action<string> NotificationTypeNameSetString;

        string ICatiBackendNotificationHandler.NotificationTypeName
        {
            get
            {
                if (NotificationTypeNameGet != null)
                {
                    return NotificationTypeNameGet();
                } else if (_inner != null)
                {
                    return ((ICatiBackendNotificationHandler)_inner).NotificationTypeName;
                }

                if (NotificationTypeNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NotificationTypeName;
                }

                return default(string);
            }

        }

    }
}