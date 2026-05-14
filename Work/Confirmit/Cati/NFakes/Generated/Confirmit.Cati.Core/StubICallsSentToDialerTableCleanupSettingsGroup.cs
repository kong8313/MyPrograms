using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubICallsSentToDialerTableCleanupSettingsGroup : ICallsSentToDialerTableCleanupSettingsGroup 
    {
        private ICallsSentToDialerTableCleanupSettingsGroup _inner;

        public StubICallsSentToDialerTableCleanupSettingsGroup()
        {
            _inner = null;
        }

        public ICallsSentToDialerTableCleanupSettingsGroup Inner
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

        private TimeSpan _ExpirationPeriod;
        public Func<TimeSpan> ExpirationPeriodGet;
        public Action<TimeSpan> ExpirationPeriodSetTimeSpan;

        TimeSpan ICallsSentToDialerTableCleanupSettings.ExpirationPeriod
        {
            get
            {
                if (ExpirationPeriodGet != null)
                {
                    return ExpirationPeriodGet();
                } else if (_inner != null)
                {
                    return ((ICallsSentToDialerTableCleanupSettings)_inner).ExpirationPeriod;
                }

                if (ExpirationPeriodSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExpirationPeriod;
                }

                return default(TimeSpan);
            }

            set
            {
                if (ExpirationPeriodSetTimeSpan != null)
                {
                    ExpirationPeriodSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallsSentToDialerTableCleanupSettings)_inner).ExpirationPeriod = value;
                    return;
                }

                if (ExpirationPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ExpirationPeriod = value;
                }

            }
        }

        private int _ShiftType;
        public Func<int> ShiftTypeGet;
        public Action<int> ShiftTypeSetInt32;

        int ICallsSentToDialerTableCleanupSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((ICallsSentToDialerTableCleanupSettings)_inner).ShiftType;
                }

                if (ShiftTypeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ShiftType;
                }

                return default(int);
            }

            set
            {
                if (ShiftTypeSetInt32 != null)
                {
                    ShiftTypeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallsSentToDialerTableCleanupSettings)_inner).ShiftType = value;
                    return;
                }

                if (ShiftTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ShiftType = value;
                }

            }
        }

    }
}