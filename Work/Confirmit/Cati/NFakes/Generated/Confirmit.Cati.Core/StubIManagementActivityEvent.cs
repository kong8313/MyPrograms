using System;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.Core.ActivityLogging.Fakes
{
    public class StubIManagementActivityEvent : IManagementActivityEvent 
    {
        private IManagementActivityEvent _inner;

        public StubIManagementActivityEvent()
        {
            _inner = null;
        }

        public IManagementActivityEvent Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void FinishDelegate();
        public FinishDelegate Finish;

        void IActivityEvent.Finish()
        {

            if (Finish != null)
            {
                Finish();
            } else if (_inner != null)
            {
                ((IActivityEvent)_inner).Finish();
            }
        }

        public delegate void SaveDelegate();
        public SaveDelegate Save;

        void IActivityEvent.Save()
        {

            if (Save != null)
            {
                Save();
            } else if (_inner != null)
            {
                ((IActivityEvent)_inner).Save();
            }
        }

        public delegate bool IsRunningDelegate();
        public IsRunningDelegate IsRunning;

        bool IActivityEvent.IsRunning()
        {


            if (IsRunning != null)
            {
                return IsRunning();
            } else if (_inner != null)
            {
                return ((IActivityEvent)_inner).IsRunning();
            }

            return default(bool);
        }

        private ManagementEvent _EventType;
        public Func<ManagementEvent> EventTypeGet;
        public Action<ManagementEvent> EventTypeSetManagementEvent;

        ManagementEvent IManagementActivityEvent.EventType
        {
            get
            {
                if (EventTypeGet != null)
                {
                    return EventTypeGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).EventType;
                }

                if (EventTypeSetManagementEvent == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EventType;
                }

                return default(ManagementEvent);
            }

        }

        private DateTime _StartTime;
        public Func<DateTime> StartTimeGet;
        public Action<DateTime> StartTimeSetDateTime;

        DateTime IManagementActivityEvent.StartTime
        {
            get
            {
                if (StartTimeGet != null)
                {
                    return StartTimeGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).StartTime;
                }

                if (StartTimeSetDateTime == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _StartTime;
                }

                return default(DateTime);
            }

            set
            {
                if (StartTimeSetDateTime != null)
                {
                    StartTimeSetDateTime(value);
                    return;
                } else if (_inner != null)
                {
                    ((IManagementActivityEvent)_inner).StartTime = value;
                    return;
                }

                if (StartTimeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _StartTime = value;
                }

            }
        }

        private int _CompanyId;
        public Func<int> CompanyIdGet;
        public Action<int> CompanyIdSetInt32;

        int IManagementActivityEvent.CompanyId
        {
            get
            {
                if (CompanyIdGet != null)
                {
                    return CompanyIdGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).CompanyId;
                }

                if (CompanyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyId;
                }

                return default(int);
            }

            set
            {
                if (CompanyIdSetInt32 != null)
                {
                    CompanyIdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IManagementActivityEvent)_inner).CompanyId = value;
                    return;
                }

                if (CompanyIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CompanyId = value;
                }

            }
        }

        private string _ServerName;
        public Func<string> ServerNameGet;
        public Action<string> ServerNameSetString;

        string IManagementActivityEvent.ServerName
        {
            get
            {
                if (ServerNameGet != null)
                {
                    return ServerNameGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).ServerName;
                }

                if (ServerNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ServerName;
                }

                return default(string);
            }

            set
            {
                if (ServerNameSetString != null)
                {
                    ServerNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IManagementActivityEvent)_inner).ServerName = value;
                    return;
                }

                if (ServerNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ServerName = value;
                }

            }
        }

        private TimeSpan _Duration;
        public Func<TimeSpan> DurationGet;
        public Action<TimeSpan> DurationSetTimeSpan;

        TimeSpan IManagementActivityEvent.Duration
        {
            get
            {
                if (DurationGet != null)
                {
                    return DurationGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).Duration;
                }

                if (DurationSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Duration;
                }

                return default(TimeSpan);
            }

        }

        private string _Supervisor;
        public Func<string> SupervisorGet;
        public Action<string> SupervisorSetString;

        string IManagementActivityEvent.Supervisor
        {
            get
            {
                if (SupervisorGet != null)
                {
                    return SupervisorGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).Supervisor;
                }

                if (SupervisorSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Supervisor;
                }

                return default(string);
            }

            set
            {
                if (SupervisorSetString != null)
                {
                    SupervisorSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IManagementActivityEvent)_inner).Supervisor = value;
                    return;
                }

                if (SupervisorGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Supervisor = value;
                }

            }
        }

        private int _ObjectId;
        public Func<int> ObjectIdGet;
        public Action<int> ObjectIdSetInt32;

        int IManagementActivityEvent.ObjectId
        {
            get
            {
                if (ObjectIdGet != null)
                {
                    return ObjectIdGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).ObjectId;
                }

                if (ObjectIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ObjectId;
                }

                return default(int);
            }

            set
            {
                if (ObjectIdSetInt32 != null)
                {
                    ObjectIdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IManagementActivityEvent)_inner).ObjectId = value;
                    return;
                }

                if (ObjectIdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ObjectId = value;
                }

            }
        }

        private string _ObjectName;
        public Func<string> ObjectNameGet;
        public Action<string> ObjectNameSetString;

        string IManagementActivityEvent.ObjectName
        {
            get
            {
                if (ObjectNameGet != null)
                {
                    return ObjectNameGet();
                } else if (_inner != null)
                {
                    return ((IManagementActivityEvent)_inner).ObjectName;
                }

                if (ObjectNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ObjectName;
                }

                return default(string);
            }

            set
            {
                if (ObjectNameSetString != null)
                {
                    ObjectNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IManagementActivityEvent)_inner).ObjectName = value;
                    return;
                }

                if (ObjectNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ObjectName = value;
                }

            }
        }

    }
}