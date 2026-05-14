using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIActivityLoggingSettingsGroup : IActivityLoggingSettingsGroup 
    {
        private IActivityLoggingSettingsGroup _inner;

        public StubIActivityLoggingSettingsGroup()
        {
            _inner = null;
        }

        public IActivityLoggingSettingsGroup Inner
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

        private TimeSpan _InterviewerActivityEventTimingsThreshold;
        public Func<TimeSpan> InterviewerActivityEventTimingsThresholdGet;
        public Action<TimeSpan> InterviewerActivityEventTimingsThresholdSetTimeSpan;

        TimeSpan IActivityLoggingSettings.InterviewerActivityEventTimingsThreshold
        {
            get
            {
                if (InterviewerActivityEventTimingsThresholdGet != null)
                {
                    return InterviewerActivityEventTimingsThresholdGet();
                } else if (_inner != null)
                {
                    return ((IActivityLoggingSettings)_inner).InterviewerActivityEventTimingsThreshold;
                }

                if (InterviewerActivityEventTimingsThresholdSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerActivityEventTimingsThreshold;
                }

                return default(TimeSpan);
            }

            set
            {
                if (InterviewerActivityEventTimingsThresholdSetTimeSpan != null)
                {
                    InterviewerActivityEventTimingsThresholdSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IActivityLoggingSettings)_inner).InterviewerActivityEventTimingsThreshold = value;
                    return;
                }

                if (InterviewerActivityEventTimingsThresholdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerActivityEventTimingsThreshold = value;
                }

            }
        }

        private TimeSpan _ManagementActivityEventTimingsThreshold;
        public Func<TimeSpan> ManagementActivityEventTimingsThresholdGet;
        public Action<TimeSpan> ManagementActivityEventTimingsThresholdSetTimeSpan;

        TimeSpan IActivityLoggingSettings.ManagementActivityEventTimingsThreshold
        {
            get
            {
                if (ManagementActivityEventTimingsThresholdGet != null)
                {
                    return ManagementActivityEventTimingsThresholdGet();
                } else if (_inner != null)
                {
                    return ((IActivityLoggingSettings)_inner).ManagementActivityEventTimingsThreshold;
                }

                if (ManagementActivityEventTimingsThresholdSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ManagementActivityEventTimingsThreshold;
                }

                return default(TimeSpan);
            }

            set
            {
                if (ManagementActivityEventTimingsThresholdSetTimeSpan != null)
                {
                    ManagementActivityEventTimingsThresholdSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IActivityLoggingSettings)_inner).ManagementActivityEventTimingsThreshold = value;
                    return;
                }

                if (ManagementActivityEventTimingsThresholdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ManagementActivityEventTimingsThreshold = value;
                }

            }
        }

    }
}