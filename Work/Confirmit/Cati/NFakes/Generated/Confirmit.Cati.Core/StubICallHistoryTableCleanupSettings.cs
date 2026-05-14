using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubICallHistoryTableCleanupSettings : ICallHistoryTableCleanupSettings 
    {
        private ICallHistoryTableCleanupSettings _inner;

        public StubICallHistoryTableCleanupSettings()
        {
            _inner = null;
        }

        public ICallHistoryTableCleanupSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private TimeSpan _ExpirationPeriod;
        public Func<TimeSpan> ExpirationPeriodGet;
        public Action<TimeSpan> ExpirationPeriodSetTimeSpan;

        TimeSpan ICallHistoryTableCleanupSettings.ExpirationPeriod
        {
            get
            {
                if (ExpirationPeriodGet != null)
                {
                    return ExpirationPeriodGet();
                } else if (_inner != null)
                {
                    return ((ICallHistoryTableCleanupSettings)_inner).ExpirationPeriod;
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
                    ((ICallHistoryTableCleanupSettings)_inner).ExpirationPeriod = value;
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

        int ICallHistoryTableCleanupSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((ICallHistoryTableCleanupSettings)_inner).ShiftType;
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
                    ((ICallHistoryTableCleanupSettings)_inner).ShiftType = value;
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