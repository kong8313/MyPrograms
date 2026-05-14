using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubIMessageTableCleanupSettingsGroup : IMessageTableCleanupSettingsGroup 
    {
        private IMessageTableCleanupSettingsGroup _inner;

        public StubIMessageTableCleanupSettingsGroup()
        {
            _inner = null;
        }

        public IMessageTableCleanupSettingsGroup Inner
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

        TimeSpan IMessageTableCleanupSettings.ExpirationPeriod
        {
            get
            {
                if (ExpirationPeriodGet != null)
                {
                    return ExpirationPeriodGet();
                } else if (_inner != null)
                {
                    return ((IMessageTableCleanupSettings)_inner).ExpirationPeriod;
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
                    ((IMessageTableCleanupSettings)_inner).ExpirationPeriod = value;
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

        int IMessageTableCleanupSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((IMessageTableCleanupSettings)_inner).ShiftType;
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
                    ((IMessageTableCleanupSettings)_inner).ShiftType = value;
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