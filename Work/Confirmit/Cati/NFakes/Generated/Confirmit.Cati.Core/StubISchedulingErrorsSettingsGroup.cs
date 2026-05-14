using System;
using Confirmit.CATI.Core.SystemSettings.Alerting;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Alerting.Fakes
{
    public class StubISchedulingErrorsSettingsGroup : ISchedulingErrorsSettingsGroup 
    {
        private ISchedulingErrorsSettingsGroup _inner;

        public StubISchedulingErrorsSettingsGroup()
        {
            _inner = null;
        }

        public ISchedulingErrorsSettingsGroup Inner
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

        private bool _IsAlertEnabled;
        public Func<bool> IsAlertEnabledGet;
        public Action<bool> IsAlertEnabledSetBoolean;

        bool ISchedulingErrorsSettings.IsAlertEnabled
        {
            get
            {
                if (IsAlertEnabledGet != null)
                {
                    return IsAlertEnabledGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingErrorsSettings)_inner).IsAlertEnabled;
                }

                if (IsAlertEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsAlertEnabled;
                }

                return default(bool);
            }

            set
            {
                if (IsAlertEnabledSetBoolean != null)
                {
                    IsAlertEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingErrorsSettings)_inner).IsAlertEnabled = value;
                    return;
                }

                if (IsAlertEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsAlertEnabled = value;
                }

            }
        }

        private TimeSpan _NotificationFrequency;
        public Func<TimeSpan> NotificationFrequencyGet;
        public Action<TimeSpan> NotificationFrequencySetTimeSpan;

        TimeSpan ISchedulingErrorsSettings.NotificationFrequency
        {
            get
            {
                if (NotificationFrequencyGet != null)
                {
                    return NotificationFrequencyGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingErrorsSettings)_inner).NotificationFrequency;
                }

                if (NotificationFrequencySetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NotificationFrequency;
                }

                return default(TimeSpan);
            }

            set
            {
                if (NotificationFrequencySetTimeSpan != null)
                {
                    NotificationFrequencySetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingErrorsSettings)_inner).NotificationFrequency = value;
                    return;
                }

                if (NotificationFrequencyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NotificationFrequency = value;
                }

            }
        }

        private int _NumberOfErrors;
        public Func<int> NumberOfErrorsGet;
        public Action<int> NumberOfErrorsSetInt32;

        int ISchedulingErrorsSettings.NumberOfErrors
        {
            get
            {
                if (NumberOfErrorsGet != null)
                {
                    return NumberOfErrorsGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingErrorsSettings)_inner).NumberOfErrors;
                }

                if (NumberOfErrorsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NumberOfErrors;
                }

                return default(int);
            }

            set
            {
                if (NumberOfErrorsSetInt32 != null)
                {
                    NumberOfErrorsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingErrorsSettings)_inner).NumberOfErrors = value;
                    return;
                }

                if (NumberOfErrorsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NumberOfErrors = value;
                }

            }
        }

        private TimeSpan _TimePeriod;
        public Func<TimeSpan> TimePeriodGet;
        public Action<TimeSpan> TimePeriodSetTimeSpan;

        TimeSpan ISchedulingErrorsSettings.TimePeriod
        {
            get
            {
                if (TimePeriodGet != null)
                {
                    return TimePeriodGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingErrorsSettings)_inner).TimePeriod;
                }

                if (TimePeriodSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TimePeriod;
                }

                return default(TimeSpan);
            }

            set
            {
                if (TimePeriodSetTimeSpan != null)
                {
                    TimePeriodSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingErrorsSettings)_inner).TimePeriod = value;
                    return;
                }

                if (TimePeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TimePeriod = value;
                }

            }
        }

    }
}