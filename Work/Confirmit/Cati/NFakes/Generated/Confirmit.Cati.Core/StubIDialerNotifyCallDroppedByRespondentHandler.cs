using System;
using Confirmit.CATI.Core.Telephony.NotificationHandlers;

namespace Confirmit.CATI.Core.Telephony.NotificationHandlers.Fakes
{
    public class StubIDialerNotifyCallDroppedByRespondentHandler : IDialerNotifyCallDroppedByRespondentHandler 
    {
        private IDialerNotifyCallDroppedByRespondentHandler _inner;

        public StubIDialerNotifyCallDroppedByRespondentHandler()
        {
            _inner = null;
        }

        public IDialerNotifyCallDroppedByRespondentHandler Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteInt32StringInt64Int64Int64Delegate(int dialerId, string companyId, long campaignId, long agentId, long callId);
        public ExecuteInt32StringInt64Int64Int64Delegate ExecuteInt32StringInt64Int64Int64;

        void IDialerNotifyCallDroppedByRespondentHandler.Execute(int dialerId, string companyId, long campaignId, long agentId, long callId)
        {

            if (ExecuteInt32StringInt64Int64Int64 != null)
            {
                ExecuteInt32StringInt64Int64Int64(dialerId, companyId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                ((IDialerNotifyCallDroppedByRespondentHandler)_inner).Execute(dialerId, companyId, campaignId, agentId, callId);
            }
        }

    }
}