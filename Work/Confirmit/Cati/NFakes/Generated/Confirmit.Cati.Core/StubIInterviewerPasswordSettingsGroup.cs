using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIInterviewerPasswordSettingsGroup : IInterviewerPasswordSettingsGroup 
    {
        private IInterviewerPasswordSettingsGroup _inner;

        public StubIInterviewerPasswordSettingsGroup()
        {
            _inner = null;
        }

        public IInterviewerPasswordSettingsGroup Inner
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

        private int _ExpirationPeriodInDays;
        public Func<int> ExpirationPeriodInDaysGet;
        public Action<int> ExpirationPeriodInDaysSetInt32;

        int IInterviewerPasswordSettings.ExpirationPeriodInDays
        {
            get
            {
                if (ExpirationPeriodInDaysGet != null)
                {
                    return ExpirationPeriodInDaysGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPasswordSettings)_inner).ExpirationPeriodInDays;
                }

                if (ExpirationPeriodInDaysSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExpirationPeriodInDays;
                }

                return default(int);
            }

            set
            {
                if (ExpirationPeriodInDaysSetInt32 != null)
                {
                    ExpirationPeriodInDaysSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPasswordSettings)_inner).ExpirationPeriodInDays = value;
                    return;
                }

                if (ExpirationPeriodInDaysGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ExpirationPeriodInDays = value;
                }

            }
        }

        private bool _IsChangeAfterFirstLoginRequired;
        public Func<bool> IsChangeAfterFirstLoginRequiredGet;
        public Action<bool> IsChangeAfterFirstLoginRequiredSetBoolean;

        bool IInterviewerPasswordSettings.IsChangeAfterFirstLoginRequired
        {
            get
            {
                if (IsChangeAfterFirstLoginRequiredGet != null)
                {
                    return IsChangeAfterFirstLoginRequiredGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPasswordSettings)_inner).IsChangeAfterFirstLoginRequired;
                }

                if (IsChangeAfterFirstLoginRequiredSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsChangeAfterFirstLoginRequired;
                }

                return default(bool);
            }

            set
            {
                if (IsChangeAfterFirstLoginRequiredSetBoolean != null)
                {
                    IsChangeAfterFirstLoginRequiredSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPasswordSettings)_inner).IsChangeAfterFirstLoginRequired = value;
                    return;
                }

                if (IsChangeAfterFirstLoginRequiredGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsChangeAfterFirstLoginRequired = value;
                }

            }
        }

        private bool _IsComplexPasswordEnforced;
        public Func<bool> IsComplexPasswordEnforcedGet;
        public Action<bool> IsComplexPasswordEnforcedSetBoolean;

        bool IInterviewerPasswordSettings.IsComplexPasswordEnforced
        {
            get
            {
                if (IsComplexPasswordEnforcedGet != null)
                {
                    return IsComplexPasswordEnforcedGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPasswordSettings)_inner).IsComplexPasswordEnforced;
                }

                if (IsComplexPasswordEnforcedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsComplexPasswordEnforced;
                }

                return default(bool);
            }

            set
            {
                if (IsComplexPasswordEnforcedSetBoolean != null)
                {
                    IsComplexPasswordEnforcedSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPasswordSettings)_inner).IsComplexPasswordEnforced = value;
                    return;
                }

                if (IsComplexPasswordEnforcedGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsComplexPasswordEnforced = value;
                }

            }
        }

        private bool _IsExpirationEnabled;
        public Func<bool> IsExpirationEnabledGet;
        public Action<bool> IsExpirationEnabledSetBoolean;

        bool IInterviewerPasswordSettings.IsExpirationEnabled
        {
            get
            {
                if (IsExpirationEnabledGet != null)
                {
                    return IsExpirationEnabledGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPasswordSettings)_inner).IsExpirationEnabled;
                }

                if (IsExpirationEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsExpirationEnabled;
                }

                return default(bool);
            }

            set
            {
                if (IsExpirationEnabledSetBoolean != null)
                {
                    IsExpirationEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPasswordSettings)_inner).IsExpirationEnabled = value;
                    return;
                }

                if (IsExpirationEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsExpirationEnabled = value;
                }

            }
        }

        private bool _IsMinimumPasswordLengthEnforced;
        public Func<bool> IsMinimumPasswordLengthEnforcedGet;
        public Action<bool> IsMinimumPasswordLengthEnforcedSetBoolean;

        bool IInterviewerPasswordSettings.IsMinimumPasswordLengthEnforced
        {
            get
            {
                if (IsMinimumPasswordLengthEnforcedGet != null)
                {
                    return IsMinimumPasswordLengthEnforcedGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPasswordSettings)_inner).IsMinimumPasswordLengthEnforced;
                }

                if (IsMinimumPasswordLengthEnforcedSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsMinimumPasswordLengthEnforced;
                }

                return default(bool);
            }

            set
            {
                if (IsMinimumPasswordLengthEnforcedSetBoolean != null)
                {
                    IsMinimumPasswordLengthEnforcedSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPasswordSettings)_inner).IsMinimumPasswordLengthEnforced = value;
                    return;
                }

                if (IsMinimumPasswordLengthEnforcedGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsMinimumPasswordLengthEnforced = value;
                }

            }
        }

        private bool _IsResetToSamePasswordEnabled;
        public Func<bool> IsResetToSamePasswordEnabledGet;
        public Action<bool> IsResetToSamePasswordEnabledSetBoolean;

        bool IInterviewerPasswordSettings.IsResetToSamePasswordEnabled
        {
            get
            {
                if (IsResetToSamePasswordEnabledGet != null)
                {
                    return IsResetToSamePasswordEnabledGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPasswordSettings)_inner).IsResetToSamePasswordEnabled;
                }

                if (IsResetToSamePasswordEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsResetToSamePasswordEnabled;
                }

                return default(bool);
            }

            set
            {
                if (IsResetToSamePasswordEnabledSetBoolean != null)
                {
                    IsResetToSamePasswordEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPasswordSettings)_inner).IsResetToSamePasswordEnabled = value;
                    return;
                }

                if (IsResetToSamePasswordEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsResetToSamePasswordEnabled = value;
                }

            }
        }

        private int _MinimumPasswordLength;
        public Func<int> MinimumPasswordLengthGet;
        public Action<int> MinimumPasswordLengthSetInt32;

        int IInterviewerPasswordSettings.MinimumPasswordLength
        {
            get
            {
                if (MinimumPasswordLengthGet != null)
                {
                    return MinimumPasswordLengthGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPasswordSettings)_inner).MinimumPasswordLength;
                }

                if (MinimumPasswordLengthSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MinimumPasswordLength;
                }

                return default(int);
            }

            set
            {
                if (MinimumPasswordLengthSetInt32 != null)
                {
                    MinimumPasswordLengthSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPasswordSettings)_inner).MinimumPasswordLength = value;
                    return;
                }

                if (MinimumPasswordLengthGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MinimumPasswordLength = value;
                }

            }
        }

    }
}