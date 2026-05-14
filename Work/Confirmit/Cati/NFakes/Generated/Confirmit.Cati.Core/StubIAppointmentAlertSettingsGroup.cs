using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIAppointmentAlertSettingsGroup : IAppointmentAlertSettingsGroup 
    {
        private IAppointmentAlertSettingsGroup _inner;

        public StubIAppointmentAlertSettingsGroup()
        {
            _inner = null;
        }

        public IAppointmentAlertSettingsGroup Inner
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

        private int _LongInterval;
        public Func<int> LongIntervalGet;
        public Action<int> LongIntervalSetInt32;

        int IAppointmentAlertSettings.LongInterval
        {
            get
            {
                if (LongIntervalGet != null)
                {
                    return LongIntervalGet();
                } else if (_inner != null)
                {
                    return ((IAppointmentAlertSettings)_inner).LongInterval;
                }

                if (LongIntervalSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LongInterval;
                }

                return default(int);
            }

            set
            {
                if (LongIntervalSetInt32 != null)
                {
                    LongIntervalSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAppointmentAlertSettings)_inner).LongInterval = value;
                    return;
                }

                if (LongIntervalGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _LongInterval = value;
                }

            }
        }

        private int _ShortInterval;
        public Func<int> ShortIntervalGet;
        public Action<int> ShortIntervalSetInt32;

        int IAppointmentAlertSettings.ShortInterval
        {
            get
            {
                if (ShortIntervalGet != null)
                {
                    return ShortIntervalGet();
                } else if (_inner != null)
                {
                    return ((IAppointmentAlertSettings)_inner).ShortInterval;
                }

                if (ShortIntervalSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShortInterval;
                }

                return default(int);
            }

            set
            {
                if (ShortIntervalSetInt32 != null)
                {
                    ShortIntervalSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAppointmentAlertSettings)_inner).ShortInterval = value;
                    return;
                }

                if (ShortIntervalGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ShortInterval = value;
                }

            }
        }

    }
}