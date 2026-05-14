using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIAccountLockingSettingsGroup : IAccountLockingSettingsGroup 
    {
        private IAccountLockingSettingsGroup _inner;

        public StubIAccountLockingSettingsGroup()
        {
            _inner = null;
        }

        public IAccountLockingSettingsGroup Inner
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

        private bool _Enabled;
        public Func<bool> EnabledGet;
        public Action<bool> EnabledSetBoolean;

        bool IAccountLockingSettings.Enabled
        {
            get
            {
                if (EnabledGet != null)
                {
                    return EnabledGet();
                } else if (_inner != null)
                {
                    return ((IAccountLockingSettings)_inner).Enabled;
                }

                if (EnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Enabled;
                }

                return default(bool);
            }

            set
            {
                if (EnabledSetBoolean != null)
                {
                    EnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAccountLockingSettings)_inner).Enabled = value;
                    return;
                }

                if (EnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Enabled = value;
                }

            }
        }

        private int _MaxFailedLoginAttempts;
        public Func<int> MaxFailedLoginAttemptsGet;
        public Action<int> MaxFailedLoginAttemptsSetInt32;

        int IAccountLockingSettings.MaxFailedLoginAttempts
        {
            get
            {
                if (MaxFailedLoginAttemptsGet != null)
                {
                    return MaxFailedLoginAttemptsGet();
                } else if (_inner != null)
                {
                    return ((IAccountLockingSettings)_inner).MaxFailedLoginAttempts;
                }

                if (MaxFailedLoginAttemptsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxFailedLoginAttempts;
                }

                return default(int);
            }

            set
            {
                if (MaxFailedLoginAttemptsSetInt32 != null)
                {
                    MaxFailedLoginAttemptsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAccountLockingSettings)_inner).MaxFailedLoginAttempts = value;
                    return;
                }

                if (MaxFailedLoginAttemptsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxFailedLoginAttempts = value;
                }

            }
        }

        private int _MaxFailedLoginAttemptsForced;
        public Func<int> MaxFailedLoginAttemptsForcedGet;
        public Action<int> MaxFailedLoginAttemptsForcedSetInt32;

        int IAccountLockingSettings.MaxFailedLoginAttemptsForced
        {
            get
            {
                if (MaxFailedLoginAttemptsForcedGet != null)
                {
                    return MaxFailedLoginAttemptsForcedGet();
                } else if (_inner != null)
                {
                    return ((IAccountLockingSettings)_inner).MaxFailedLoginAttemptsForced;
                }

                if (MaxFailedLoginAttemptsForcedSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxFailedLoginAttemptsForced;
                }

                return default(int);
            }

            set
            {
                if (MaxFailedLoginAttemptsForcedSetInt32 != null)
                {
                    MaxFailedLoginAttemptsForcedSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAccountLockingSettings)_inner).MaxFailedLoginAttemptsForced = value;
                    return;
                }

                if (MaxFailedLoginAttemptsForcedGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxFailedLoginAttemptsForced = value;
                }

            }
        }

    }
}