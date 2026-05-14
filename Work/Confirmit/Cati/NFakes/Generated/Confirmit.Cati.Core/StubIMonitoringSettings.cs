using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIMonitoringSettings : IMonitoringSettings 
    {
        private IMonitoringSettings _inner;

        public StubIMonitoringSettings()
        {
            _inner = null;
        }

        public IMonitoringSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _AllowBargingMode;
        public Func<bool> AllowBargingModeGet;
        public Action<bool> AllowBargingModeSetBoolean;

        bool IMonitoringSettings.AllowBargingMode
        {
            get
            {
                if (AllowBargingModeGet != null)
                {
                    return AllowBargingModeGet();
                } else if (_inner != null)
                {
                    return ((IMonitoringSettings)_inner).AllowBargingMode;
                }

                if (AllowBargingModeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllowBargingMode;
                }

                return default(bool);
            }

            set
            {
                if (AllowBargingModeSetBoolean != null)
                {
                    AllowBargingModeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMonitoringSettings)_inner).AllowBargingMode = value;
                    return;
                }

                if (AllowBargingModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllowBargingMode = value;
                }

            }
        }

        private bool _AllowCoachingMode;
        public Func<bool> AllowCoachingModeGet;
        public Action<bool> AllowCoachingModeSetBoolean;

        bool IMonitoringSettings.AllowCoachingMode
        {
            get
            {
                if (AllowCoachingModeGet != null)
                {
                    return AllowCoachingModeGet();
                } else if (_inner != null)
                {
                    return ((IMonitoringSettings)_inner).AllowCoachingMode;
                }

                if (AllowCoachingModeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllowCoachingMode;
                }

                return default(bool);
            }

            set
            {
                if (AllowCoachingModeSetBoolean != null)
                {
                    AllowCoachingModeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMonitoringSettings)_inner).AllowCoachingMode = value;
                    return;
                }

                if (AllowCoachingModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllowCoachingMode = value;
                }

            }
        }

        private int _LaunchFileAllowedTimeLifeInHours;
        public Func<int> LaunchFileAllowedTimeLifeInHoursGet;
        public Action<int> LaunchFileAllowedTimeLifeInHoursSetInt32;

        int IMonitoringSettings.LaunchFileAllowedTimeLifeInHours
        {
            get
            {
                if (LaunchFileAllowedTimeLifeInHoursGet != null)
                {
                    return LaunchFileAllowedTimeLifeInHoursGet();
                } else if (_inner != null)
                {
                    return ((IMonitoringSettings)_inner).LaunchFileAllowedTimeLifeInHours;
                }

                if (LaunchFileAllowedTimeLifeInHoursSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LaunchFileAllowedTimeLifeInHours;
                }

                return default(int);
            }

            set
            {
                if (LaunchFileAllowedTimeLifeInHoursSetInt32 != null)
                {
                    LaunchFileAllowedTimeLifeInHoursSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMonitoringSettings)_inner).LaunchFileAllowedTimeLifeInHours = value;
                    return;
                }

                if (LaunchFileAllowedTimeLifeInHoursGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _LaunchFileAllowedTimeLifeInHours = value;
                }

            }
        }

    }
}