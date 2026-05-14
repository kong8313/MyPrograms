using System;
using Confirmit.CATI.Core.Telephony.Inbound;

namespace Confirmit.CATI.Core.Telephony.Inbound.Fakes
{
    public class StubIDialerNotifyInboundCallHandler : IDialerNotifyInboundCallHandler 
    {
        private IDialerNotifyInboundCallHandler _inner;

        public StubIDialerNotifyInboundCallHandler()
        {
            _inner = null;
        }

        public IDialerNotifyInboundCallHandler Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteInt32Int32StringStringStringDelegate(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId);
        public ExecuteInt32Int32StringStringStringDelegate ExecuteInt32Int32StringStringString;

        void IDialerNotifyInboundCallHandler.Execute(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId)
        {

            if (ExecuteInt32Int32StringStringString != null)
            {
                ExecuteInt32Int32StringStringString(dialerId, companyId, ddiNumber, cliNumber, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerNotifyInboundCallHandler)_inner).Execute(dialerId, companyId, ddiNumber, cliNumber, inboundCallId);
            }
        }

    }
}