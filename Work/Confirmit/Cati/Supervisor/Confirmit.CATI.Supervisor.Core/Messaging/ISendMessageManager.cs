using Confirmit.CATI.Supervisor.Messaging;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Messaging
{
    public interface ISendMessageManager
    {
        void SendMessage(string userName, string messageText, MessageRecipientType recipientType, List<int> interviewerIds, bool onlineOnly);
    }
}
