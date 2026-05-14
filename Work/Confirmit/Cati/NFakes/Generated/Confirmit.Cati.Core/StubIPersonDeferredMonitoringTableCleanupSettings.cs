using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubIPersonDeferredMonitoringTableCleanupSettings : IPersonDeferredMonitoringTableCleanupSettings 
    {
        private IPersonDeferredMonitoringTableCleanupSettings _inner;

        public StubIPersonDeferredMonitoringTableCleanupSettings()
        {
            _inner = null;
        }

        public IPersonDeferredMonitoringTableCleanupSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private TimeSpan _DelayBetweenDeletes;
        public Func<TimeSpan> DelayBetweenDeletesGet;
        public Action<TimeSpan> DelayBetweenDeletesSetTimeSpan;

        TimeSpan IPersonDeferredMonitoringTableCleanupSettings.DelayBetweenDeletes
        {
            get
            {
                if (DelayBetweenDeletesGet != null)
                {
                    return DelayBetweenDeletesGet();
                } else if (_inner != null)
                {
                    return ((IPersonDeferredMonitoringTableCleanupSettings)_inner).DelayBetweenDeletes;
                }

                if (DelayBetweenDeletesSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DelayBetweenDeletes;
                }

                return default(TimeSpan);
            }

            set
            {
                if (DelayBetweenDeletesSetTimeSpan != null)
                {
                    DelayBetweenDeletesSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IPersonDeferredMonitoringTableCleanupSettings)_inner).DelayBetweenDeletes = value;
                    return;
                }

                if (DelayBetweenDeletesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DelayBetweenDeletes = value;
                }

            }
        }

        private int _DeleteTopRows;
        public Func<int> DeleteTopRowsGet;
        public Action<int> DeleteTopRowsSetInt32;

        int IPersonDeferredMonitoringTableCleanupSettings.DeleteTopRows
        {
            get
            {
                if (DeleteTopRowsGet != null)
                {
                    return DeleteTopRowsGet();
                } else if (_inner != null)
                {
                    return ((IPersonDeferredMonitoringTableCleanupSettings)_inner).DeleteTopRows;
                }

                if (DeleteTopRowsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DeleteTopRows;
                }

                return default(int);
            }

            set
            {
                if (DeleteTopRowsSetInt32 != null)
                {
                    DeleteTopRowsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IPersonDeferredMonitoringTableCleanupSettings)_inner).DeleteTopRows = value;
                    return;
                }

                if (DeleteTopRowsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DeleteTopRows = value;
                }

            }
        }

        private TimeSpan _ExpirationPeriod;
        public Func<TimeSpan> ExpirationPeriodGet;
        public Action<TimeSpan> ExpirationPeriodSetTimeSpan;

        TimeSpan IPersonDeferredMonitoringTableCleanupSettings.ExpirationPeriod
        {
            get
            {
                if (ExpirationPeriodGet != null)
                {
                    return ExpirationPeriodGet();
                } else if (_inner != null)
                {
                    return ((IPersonDeferredMonitoringTableCleanupSettings)_inner).ExpirationPeriod;
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
                    ((IPersonDeferredMonitoringTableCleanupSettings)_inner).ExpirationPeriod = value;
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

        int IPersonDeferredMonitoringTableCleanupSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((IPersonDeferredMonitoringTableCleanupSettings)_inner).ShiftType;
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
                    ((IPersonDeferredMonitoringTableCleanupSettings)_inner).ShiftType = value;
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