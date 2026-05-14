using System;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerLoginLogoutManager : IDialerLoginLogoutManager 
    {
        private IDialerLoginLogoutManager _inner;

        public StubIDialerLoginLogoutManager()
        {
            _inner = null;
        }

        public IDialerLoginLogoutManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DialerErrorCode LogoutInt32Int64BooleanInt32Delegate(int dialerId, long campaignId, bool isPredictive, int agentId);
        public LogoutInt32Int64BooleanInt32Delegate LogoutInt32Int64BooleanInt32;

        DialerErrorCode IDialerLoginLogoutManager.Logout(int dialerId, long campaignId, bool isPredictive, int agentId)
        {


            if (LogoutInt32Int64BooleanInt32 != null)
            {
                return LogoutInt32Int64BooleanInt32(dialerId, campaignId, isPredictive, agentId);
            } else if (_inner != null)
            {
                return ((IDialerLoginLogoutManager)_inner).Logout(dialerId, campaignId, isPredictive, agentId);
            }

            return default(DialerErrorCode);
        }

    }
}