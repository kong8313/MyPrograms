using System;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;

namespace Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions.Fakes
{
    public class StubISurveyCleanupSettings : ISurveyCleanupSettings 
    {
        private ISurveyCleanupSettings _inner;

        public StubISurveyCleanupSettings()
        {
            _inner = null;
        }

        public ISurveyCleanupSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private TimeSpan _CleanupTimeout;
        public Func<TimeSpan> CleanupTimeoutGet;
        public Action<TimeSpan> CleanupTimeoutSetTimeSpan;

        TimeSpan ISurveyCleanupSettings.CleanupTimeout
        {
            get
            {
                if (CleanupTimeoutGet != null)
                {
                    return CleanupTimeoutGet();
                } else if (_inner != null)
                {
                    return ((ISurveyCleanupSettings)_inner).CleanupTimeout;
                }

                if (CleanupTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CleanupTimeout;
                }

                return default(TimeSpan);
            }

            set
            {
                if (CleanupTimeoutSetTimeSpan != null)
                {
                    CleanupTimeoutSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISurveyCleanupSettings)_inner).CleanupTimeout = value;
                    return;
                }

                if (CleanupTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CleanupTimeout = value;
                }

            }
        }

        private TimeSpan _NotificationTimeout;
        public Func<TimeSpan> NotificationTimeoutGet;
        public Action<TimeSpan> NotificationTimeoutSetTimeSpan;

        TimeSpan ISurveyCleanupSettings.NotificationTimeout
        {
            get
            {
                if (NotificationTimeoutGet != null)
                {
                    return NotificationTimeoutGet();
                } else if (_inner != null)
                {
                    return ((ISurveyCleanupSettings)_inner).NotificationTimeout;
                }

                if (NotificationTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NotificationTimeout;
                }

                return default(TimeSpan);
            }

            set
            {
                if (NotificationTimeoutSetTimeSpan != null)
                {
                    NotificationTimeoutSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISurveyCleanupSettings)_inner).NotificationTimeout = value;
                    return;
                }

                if (NotificationTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NotificationTimeout = value;
                }

            }
        }

        private int _ShiftType;
        public Func<int> ShiftTypeGet;
        public Action<int> ShiftTypeSetInt32;

        int ISurveyCleanupSettings.ShiftType
        {
            get
            {
                if (ShiftTypeGet != null)
                {
                    return ShiftTypeGet();
                } else if (_inner != null)
                {
                    return ((ISurveyCleanupSettings)_inner).ShiftType;
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
                    ((ISurveyCleanupSettings)_inner).ShiftType = value;
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