using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubITelephoneBlacklistSettings : ITelephoneBlacklistSettings 
    {
        private ITelephoneBlacklistSettings _inner;

        public StubITelephoneBlacklistSettings()
        {
            _inner = null;
        }

        public ITelephoneBlacklistSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _TelephoneBlacklistLimit;
        public Func<int> TelephoneBlacklistLimitGet;
        public Action<int> TelephoneBlacklistLimitSetInt32;

        int ITelephoneBlacklistSettings.TelephoneBlacklistLimit
        {
            get
            {
                if (TelephoneBlacklistLimitGet != null)
                {
                    return TelephoneBlacklistLimitGet();
                } else if (_inner != null)
                {
                    return ((ITelephoneBlacklistSettings)_inner).TelephoneBlacklistLimit;
                }

                if (TelephoneBlacklistLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TelephoneBlacklistLimit;
                }

                return default(int);
            }

            set
            {
                if (TelephoneBlacklistLimitSetInt32 != null)
                {
                    TelephoneBlacklistLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ITelephoneBlacklistSettings)_inner).TelephoneBlacklistLimit = value;
                    return;
                }

                if (TelephoneBlacklistLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TelephoneBlacklistLimit = value;
                }

            }
        }

    }
}