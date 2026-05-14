using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubITelephoneBlacklistSettingsGroup : ITelephoneBlacklistSettingsGroup 
    {
        private ITelephoneBlacklistSettingsGroup _inner;

        public StubITelephoneBlacklistSettingsGroup()
        {
            _inner = null;
        }

        public ITelephoneBlacklistSettingsGroup Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnChangedDelegate();
        public OnChangedDelegate OnChanged;

        void ISystemSettingsNotifyChanged.OnChanged()
        {

            if (OnChanged != null)
            {
                OnChanged();
            } else if (_inner != null)
            {
                ((ISystemSettingsNotifyChanged)_inner).OnChanged();
            }
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