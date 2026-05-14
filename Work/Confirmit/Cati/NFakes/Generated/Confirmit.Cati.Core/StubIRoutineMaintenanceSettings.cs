using System;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIRoutineMaintenanceSettings : IRoutineMaintenanceSettings 
    {
        private IRoutineMaintenanceSettings _inner;

        public StubIRoutineMaintenanceSettings()
        {
            _inner = null;
        }

        public IRoutineMaintenanceSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private IActionsSettings _Actions;
        public Func<IActionsSettings> ActionsGet;
        public Action<IActionsSettings> ActionsSetIActionsSettings;

        IActionsSettings IRoutineMaintenanceSettings.Actions
        {
            get
            {
                if (ActionsGet != null)
                {
                    return ActionsGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceSettings)_inner).Actions;
                }

                if (ActionsSetIActionsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Actions;
                }

                return default(IActionsSettings);
            }

        }

        private TimeSpan _DailyShiftStartTime;
        public Func<TimeSpan> DailyShiftStartTimeGet;
        public Action<TimeSpan> DailyShiftStartTimeSetTimeSpan;

        TimeSpan IRoutineMaintenanceSettings.DailyShiftStartTime
        {
            get
            {
                if (DailyShiftStartTimeGet != null)
                {
                    return DailyShiftStartTimeGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceSettings)_inner).DailyShiftStartTime;
                }

                if (DailyShiftStartTimeSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DailyShiftStartTime;
                }

                return default(TimeSpan);
            }

            set
            {
                if (DailyShiftStartTimeSetTimeSpan != null)
                {
                    DailyShiftStartTimeSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRoutineMaintenanceSettings)_inner).DailyShiftStartTime = value;
                    return;
                }

                if (DailyShiftStartTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DailyShiftStartTime = value;
                }

            }
        }

        private TimeSpan _Duration;
        public Func<TimeSpan> DurationGet;
        public Action<TimeSpan> DurationSetTimeSpan;

        TimeSpan IRoutineMaintenanceSettings.Duration
        {
            get
            {
                if (DurationGet != null)
                {
                    return DurationGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceSettings)_inner).Duration;
                }

                if (DurationSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Duration;
                }

                return default(TimeSpan);
            }

            set
            {
                if (DurationSetTimeSpan != null)
                {
                    DurationSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRoutineMaintenanceSettings)_inner).Duration = value;
                    return;
                }

                if (DurationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Duration = value;
                }

            }
        }

        private TimeSpan _FrequencyExecution;
        public Func<TimeSpan> FrequencyExecutionGet;
        public Action<TimeSpan> FrequencyExecutionSetTimeSpan;

        TimeSpan IRoutineMaintenanceSettings.FrequencyExecution
        {
            get
            {
                if (FrequencyExecutionGet != null)
                {
                    return FrequencyExecutionGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceSettings)_inner).FrequencyExecution;
                }

                if (FrequencyExecutionSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FrequencyExecution;
                }

                return default(TimeSpan);
            }

            set
            {
                if (FrequencyExecutionSetTimeSpan != null)
                {
                    FrequencyExecutionSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRoutineMaintenanceSettings)_inner).FrequencyExecution = value;
                    return;
                }

                if (FrequencyExecutionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _FrequencyExecution = value;
                }

            }
        }

        private int _MonthlyShiftWeekNumber;
        public Func<int> MonthlyShiftWeekNumberGet;
        public Action<int> MonthlyShiftWeekNumberSetInt32;

        int IRoutineMaintenanceSettings.MonthlyShiftWeekNumber
        {
            get
            {
                if (MonthlyShiftWeekNumberGet != null)
                {
                    return MonthlyShiftWeekNumberGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceSettings)_inner).MonthlyShiftWeekNumber;
                }

                if (MonthlyShiftWeekNumberSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MonthlyShiftWeekNumber;
                }

                return default(int);
            }

            set
            {
                if (MonthlyShiftWeekNumberSetInt32 != null)
                {
                    MonthlyShiftWeekNumberSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRoutineMaintenanceSettings)_inner).MonthlyShiftWeekNumber = value;
                    return;
                }

                if (MonthlyShiftWeekNumberGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MonthlyShiftWeekNumber = value;
                }

            }
        }

        private int _WeeklyShiftDayNumber;
        public Func<int> WeeklyShiftDayNumberGet;
        public Action<int> WeeklyShiftDayNumberSetInt32;

        int IRoutineMaintenanceSettings.WeeklyShiftDayNumber
        {
            get
            {
                if (WeeklyShiftDayNumberGet != null)
                {
                    return WeeklyShiftDayNumberGet();
                } else if (_inner != null)
                {
                    return ((IRoutineMaintenanceSettings)_inner).WeeklyShiftDayNumber;
                }

                if (WeeklyShiftDayNumberSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _WeeklyShiftDayNumber;
                }

                return default(int);
            }

            set
            {
                if (WeeklyShiftDayNumberSetInt32 != null)
                {
                    WeeklyShiftDayNumberSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRoutineMaintenanceSettings)_inner).WeeklyShiftDayNumber = value;
                    return;
                }

                if (WeeklyShiftDayNumberGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _WeeklyShiftDayNumber = value;
                }

            }
        }

    }
}