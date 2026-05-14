using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Classes.Fakes
{
    public class StubIDialerStatusProvider : IDialerStatusProvider 
    {
        private IDialerStatusProvider _inner;

        public StubIDialerStatusProvider()
        {
            _inner = null;
        }

        public IDialerStatusProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DialerStatus GetDialerStatusInt32BooleanDelegate(int dialerId, bool isActivated);
        public GetDialerStatusInt32BooleanDelegate GetDialerStatusInt32Boolean;

        DialerStatus IDialerStatusProvider.GetDialerStatus(int dialerId, bool isActivated)
        {


            if (GetDialerStatusInt32Boolean != null)
            {
                return GetDialerStatusInt32Boolean(dialerId, isActivated);
            } else if (_inner != null)
            {
                return ((IDialerStatusProvider)_inner).GetDialerStatus(dialerId, isActivated);
            }

            return default(DialerStatus);
        }

        public delegate DialerStatus GetDialerActualStatusInt32BooleanBooleanInt32Delegate(int dialerId, bool isActivated, bool withReconnection, int expectedStatus);
        public GetDialerActualStatusInt32BooleanBooleanInt32Delegate GetDialerActualStatusInt32BooleanBooleanInt32;

        DialerStatus IDialerStatusProvider.GetDialerActualStatus(int dialerId, bool isActivated, bool withReconnection, int expectedStatus)
        {


            if (GetDialerActualStatusInt32BooleanBooleanInt32 != null)
            {
                return GetDialerActualStatusInt32BooleanBooleanInt32(dialerId, isActivated, withReconnection, expectedStatus);
            } else if (_inner != null)
            {
                return ((IDialerStatusProvider)_inner).GetDialerActualStatus(dialerId, isActivated, withReconnection, expectedStatus);
            }

            return default(DialerStatus);
        }

    }
}