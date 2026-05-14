using System;
using Confirmit.CATI.Core.SystemSettings.Toggle;

namespace Confirmit.CATI.Core.SystemSettings.Toggle.Fakes
{
    public class StubICatiAgentSettings : ICatiAgentSettings 
    {
        private ICatiAgentSettings _inner;

        public StubICatiAgentSettings()
        {
            _inner = null;
        }

        public ICatiAgentSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _AsyncOperationSchedulerThread;
        public Func<bool> AsyncOperationSchedulerThreadGet;
        public Action<bool> AsyncOperationSchedulerThreadSetBoolean;

        bool ICatiAgentSettings.AsyncOperationSchedulerThread
        {
            get
            {
                if (AsyncOperationSchedulerThreadGet != null)
                {
                    return AsyncOperationSchedulerThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).AsyncOperationSchedulerThread;
                }

                if (AsyncOperationSchedulerThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AsyncOperationSchedulerThread;
                }

                return default(bool);
            }

            set
            {
                if (AsyncOperationSchedulerThreadSetBoolean != null)
                {
                    AsyncOperationSchedulerThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).AsyncOperationSchedulerThread = value;
                    return;
                }

                if (AsyncOperationSchedulerThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AsyncOperationSchedulerThread = value;
                }

            }
        }

        private bool _AsyncOperationsHeartBeatUpdaterThread;
        public Func<bool> AsyncOperationsHeartBeatUpdaterThreadGet;
        public Action<bool> AsyncOperationsHeartBeatUpdaterThreadSetBoolean;

        bool ICatiAgentSettings.AsyncOperationsHeartBeatUpdaterThread
        {
            get
            {
                if (AsyncOperationsHeartBeatUpdaterThreadGet != null)
                {
                    return AsyncOperationsHeartBeatUpdaterThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).AsyncOperationsHeartBeatUpdaterThread;
                }

                if (AsyncOperationsHeartBeatUpdaterThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AsyncOperationsHeartBeatUpdaterThread;
                }

                return default(bool);
            }

            set
            {
                if (AsyncOperationsHeartBeatUpdaterThreadSetBoolean != null)
                {
                    AsyncOperationsHeartBeatUpdaterThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).AsyncOperationsHeartBeatUpdaterThread = value;
                    return;
                }

                if (AsyncOperationsHeartBeatUpdaterThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AsyncOperationsHeartBeatUpdaterThread = value;
                }

            }
        }

        private bool _AutoLogoutThread;
        public Func<bool> AutoLogoutThreadGet;
        public Action<bool> AutoLogoutThreadSetBoolean;

        bool ICatiAgentSettings.AutoLogoutThread
        {
            get
            {
                if (AutoLogoutThreadGet != null)
                {
                    return AutoLogoutThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).AutoLogoutThread;
                }

                if (AutoLogoutThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AutoLogoutThread;
                }

                return default(bool);
            }

            set
            {
                if (AutoLogoutThreadSetBoolean != null)
                {
                    AutoLogoutThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).AutoLogoutThread = value;
                    return;
                }

                if (AutoLogoutThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AutoLogoutThread = value;
                }

            }
        }

        private bool _AutoLogoutWebConsoleThread;
        public Func<bool> AutoLogoutWebConsoleThreadGet;
        public Action<bool> AutoLogoutWebConsoleThreadSetBoolean;

        bool ICatiAgentSettings.AutoLogoutWebConsoleThread
        {
            get
            {
                if (AutoLogoutWebConsoleThreadGet != null)
                {
                    return AutoLogoutWebConsoleThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).AutoLogoutWebConsoleThread;
                }

                if (AutoLogoutWebConsoleThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AutoLogoutWebConsoleThread;
                }

                return default(bool);
            }

            set
            {
                if (AutoLogoutWebConsoleThreadSetBoolean != null)
                {
                    AutoLogoutWebConsoleThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).AutoLogoutWebConsoleThread = value;
                    return;
                }

                if (AutoLogoutWebConsoleThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AutoLogoutWebConsoleThread = value;
                }

            }
        }

        private bool _DialerHealthControlThread;
        public Func<bool> DialerHealthControlThreadGet;
        public Action<bool> DialerHealthControlThreadSetBoolean;

        bool ICatiAgentSettings.DialerHealthControlThread
        {
            get
            {
                if (DialerHealthControlThreadGet != null)
                {
                    return DialerHealthControlThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).DialerHealthControlThread;
                }

                if (DialerHealthControlThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DialerHealthControlThread;
                }

                return default(bool);
            }

            set
            {
                if (DialerHealthControlThreadSetBoolean != null)
                {
                    DialerHealthControlThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).DialerHealthControlThread = value;
                    return;
                }

                if (DialerHealthControlThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DialerHealthControlThread = value;
                }

            }
        }

        private bool _EmailReportsThread;
        public Func<bool> EmailReportsThreadGet;
        public Action<bool> EmailReportsThreadSetBoolean;

        bool ICatiAgentSettings.EmailReportsThread
        {
            get
            {
                if (EmailReportsThreadGet != null)
                {
                    return EmailReportsThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).EmailReportsThread;
                }

                if (EmailReportsThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EmailReportsThread;
                }

                return default(bool);
            }

            set
            {
                if (EmailReportsThreadSetBoolean != null)
                {
                    EmailReportsThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).EmailReportsThread = value;
                    return;
                }

                if (EmailReportsThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EmailReportsThread = value;
                }

            }
        }

        private bool _ExpiredCallsThread;
        public Func<bool> ExpiredCallsThreadGet;
        public Action<bool> ExpiredCallsThreadSetBoolean;

        bool ICatiAgentSettings.ExpiredCallsThread
        {
            get
            {
                if (ExpiredCallsThreadGet != null)
                {
                    return ExpiredCallsThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).ExpiredCallsThread;
                }

                if (ExpiredCallsThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExpiredCallsThread;
                }

                return default(bool);
            }

            set
            {
                if (ExpiredCallsThreadSetBoolean != null)
                {
                    ExpiredCallsThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).ExpiredCallsThread = value;
                    return;
                }

                if (ExpiredCallsThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ExpiredCallsThread = value;
                }

            }
        }

        private bool _IvrThread;
        public Func<bool> IvrThreadGet;
        public Action<bool> IvrThreadSetBoolean;

        bool ICatiAgentSettings.IvrThread
        {
            get
            {
                if (IvrThreadGet != null)
                {
                    return IvrThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).IvrThread;
                }

                if (IvrThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IvrThread;
                }

                return default(bool);
            }

            set
            {
                if (IvrThreadSetBoolean != null)
                {
                    IvrThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).IvrThread = value;
                    return;
                }

                if (IvrThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IvrThread = value;
                }

            }
        }

        private bool _ReplicationThread;
        public Func<bool> ReplicationThreadGet;
        public Action<bool> ReplicationThreadSetBoolean;

        bool ICatiAgentSettings.ReplicationThread
        {
            get
            {
                if (ReplicationThreadGet != null)
                {
                    return ReplicationThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).ReplicationThread;
                }

                if (ReplicationThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReplicationThread;
                }

                return default(bool);
            }

            set
            {
                if (ReplicationThreadSetBoolean != null)
                {
                    ReplicationThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).ReplicationThread = value;
                    return;
                }

                if (ReplicationThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ReplicationThread = value;
                }

            }
        }

        private bool _ReviewerUpdateReviewStatusThread;
        public Func<bool> ReviewerUpdateReviewStatusThreadGet;
        public Action<bool> ReviewerUpdateReviewStatusThreadSetBoolean;

        bool ICatiAgentSettings.ReviewerUpdateReviewStatusThread
        {
            get
            {
                if (ReviewerUpdateReviewStatusThreadGet != null)
                {
                    return ReviewerUpdateReviewStatusThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).ReviewerUpdateReviewStatusThread;
                }

                if (ReviewerUpdateReviewStatusThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReviewerUpdateReviewStatusThread;
                }

                return default(bool);
            }

            set
            {
                if (ReviewerUpdateReviewStatusThreadSetBoolean != null)
                {
                    ReviewerUpdateReviewStatusThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).ReviewerUpdateReviewStatusThread = value;
                    return;
                }

                if (ReviewerUpdateReviewStatusThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ReviewerUpdateReviewStatusThread = value;
                }

            }
        }

        private bool _RoutineMaintenanceThread;
        public Func<bool> RoutineMaintenanceThreadGet;
        public Action<bool> RoutineMaintenanceThreadSetBoolean;

        bool ICatiAgentSettings.RoutineMaintenanceThread
        {
            get
            {
                if (RoutineMaintenanceThreadGet != null)
                {
                    return RoutineMaintenanceThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).RoutineMaintenanceThread;
                }

                if (RoutineMaintenanceThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RoutineMaintenanceThread;
                }

                return default(bool);
            }

            set
            {
                if (RoutineMaintenanceThreadSetBoolean != null)
                {
                    RoutineMaintenanceThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).RoutineMaintenanceThread = value;
                    return;
                }

                if (RoutineMaintenanceThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RoutineMaintenanceThread = value;
                }

            }
        }

        private bool _ScheduleErrorsNotificationThread;
        public Func<bool> ScheduleErrorsNotificationThreadGet;
        public Action<bool> ScheduleErrorsNotificationThreadSetBoolean;

        bool ICatiAgentSettings.ScheduleErrorsNotificationThread
        {
            get
            {
                if (ScheduleErrorsNotificationThreadGet != null)
                {
                    return ScheduleErrorsNotificationThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).ScheduleErrorsNotificationThread;
                }

                if (ScheduleErrorsNotificationThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ScheduleErrorsNotificationThread;
                }

                return default(bool);
            }

            set
            {
                if (ScheduleErrorsNotificationThreadSetBoolean != null)
                {
                    ScheduleErrorsNotificationThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).ScheduleErrorsNotificationThread = value;
                    return;
                }

                if (ScheduleErrorsNotificationThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ScheduleErrorsNotificationThread = value;
                }

            }
        }

        private bool _ScheduleThread;
        public Func<bool> ScheduleThreadGet;
        public Action<bool> ScheduleThreadSetBoolean;

        bool ICatiAgentSettings.ScheduleThread
        {
            get
            {
                if (ScheduleThreadGet != null)
                {
                    return ScheduleThreadGet();
                } else if (_inner != null)
                {
                    return ((ICatiAgentSettings)_inner).ScheduleThread;
                }

                if (ScheduleThreadSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ScheduleThread;
                }

                return default(bool);
            }

            set
            {
                if (ScheduleThreadSetBoolean != null)
                {
                    ScheduleThreadSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiAgentSettings)_inner).ScheduleThread = value;
                    return;
                }

                if (ScheduleThreadGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ScheduleThread = value;
                }

            }
        }

    }
}