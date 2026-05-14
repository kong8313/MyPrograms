using System;
using Confirmit.CATI.Supervisor.Messaging;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Messaging;

namespace Confirmit.CATI.Supervisor.Core.Messaging.Fakes
{
    public class StubISendMessageManager : ISendMessageManager 
    {
        private ISendMessageManager _inner;

        public StubISendMessageManager()
        {
            _inner = null;
        }

        public ISendMessageManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendMessageStringStringMessageRecipientTypeListOfInt32BooleanDelegate(string userName, string messageText, MessageRecipientType recipientType, List<int> interviewerIds, bool onlineOnly);
        public SendMessageStringStringMessageRecipientTypeListOfInt32BooleanDelegate SendMessageStringStringMessageRecipientTypeListOfInt32Boolean;

        void ISendMessageManager.SendMessage(string userName, string messageText, MessageRecipientType recipientType, List<int> interviewerIds, bool onlineOnly)
        {

            if (SendMessageStringStringMessageRecipientTypeListOfInt32Boolean != null)
            {
                SendMessageStringStringMessageRecipientTypeListOfInt32Boolean(userName, messageText, recipientType, interviewerIds, onlineOnly);
            } else if (_inner != null)
            {
                ((ISendMessageManager)_inner).SendMessage(userName, messageText, recipientType, interviewerIds, onlineOnly);
            }
        }

    }
}