using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIAutoLogoutSettings : IAutoLogoutSettings 
    {
        private IAutoLogoutSettings _inner;

        public StubIAutoLogoutSettings()
        {
            _inner = null;
        }

        public IAutoLogoutSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _AutoLogoutThreadSleepPeriod;
        public Func<int> AutoLogoutThreadSleepPeriodGet;
        public Action<int> AutoLogoutThreadSleepPeriodSetInt32;

        int IAutoLogoutSettings.AutoLogoutThreadSleepPeriod
        {
            get
            {
                if (AutoLogoutThreadSleepPeriodGet != null)
                {
                    return AutoLogoutThreadSleepPeriodGet();
                } else if (_inner != null)
                {
                    return ((IAutoLogoutSettings)_inner).AutoLogoutThreadSleepPeriod;
                }

                if (AutoLogoutThreadSleepPeriodSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AutoLogoutThreadSleepPeriod;
                }

                return default(int);
            }

            set
            {
                if (AutoLogoutThreadSleepPeriodSetInt32 != null)
                {
                    AutoLogoutThreadSleepPeriodSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAutoLogoutSettings)_inner).AutoLogoutThreadSleepPeriod = value;
                    return;
                }

                if (AutoLogoutThreadSleepPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AutoLogoutThreadSleepPeriod = value;
                }

            }
        }

        private int _AutoLogoutTimeout;
        public Func<int> AutoLogoutTimeoutGet;
        public Action<int> AutoLogoutTimeoutSetInt32;

        int IAutoLogoutSettings.AutoLogoutTimeout
        {
            get
            {
                if (AutoLogoutTimeoutGet != null)
                {
                    return AutoLogoutTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IAutoLogoutSettings)_inner).AutoLogoutTimeout;
                }

                if (AutoLogoutTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AutoLogoutTimeout;
                }

                return default(int);
            }

            set
            {
                if (AutoLogoutTimeoutSetInt32 != null)
                {
                    AutoLogoutTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAutoLogoutSettings)_inner).AutoLogoutTimeout = value;
                    return;
                }

                if (AutoLogoutTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AutoLogoutTimeout = value;
                }

            }
        }

        private TimeSpan _AutoLogoutWebConsoleThreadSleepPeriod;
        public Func<TimeSpan> AutoLogoutWebConsoleThreadSleepPeriodGet;
        public Action<TimeSpan> AutoLogoutWebConsoleThreadSleepPeriodSetTimeSpan;

        TimeSpan IAutoLogoutSettings.AutoLogoutWebConsoleThreadSleepPeriod
        {
            get
            {
                if (AutoLogoutWebConsoleThreadSleepPeriodGet != null)
                {
                    return AutoLogoutWebConsoleThreadSleepPeriodGet();
                } else if (_inner != null)
                {
                    return ((IAutoLogoutSettings)_inner).AutoLogoutWebConsoleThreadSleepPeriod;
                }

                if (AutoLogoutWebConsoleThreadSleepPeriodSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AutoLogoutWebConsoleThreadSleepPeriod;
                }

                return default(TimeSpan);
            }

            set
            {
                if (AutoLogoutWebConsoleThreadSleepPeriodSetTimeSpan != null)
                {
                    AutoLogoutWebConsoleThreadSleepPeriodSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAutoLogoutSettings)_inner).AutoLogoutWebConsoleThreadSleepPeriod = value;
                    return;
                }

                if (AutoLogoutWebConsoleThreadSleepPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AutoLogoutWebConsoleThreadSleepPeriod = value;
                }

            }
        }

        private TimeSpan _AutoLogoutWebConsoleTimeout;
        public Func<TimeSpan> AutoLogoutWebConsoleTimeoutGet;
        public Action<TimeSpan> AutoLogoutWebConsoleTimeoutSetTimeSpan;

        TimeSpan IAutoLogoutSettings.AutoLogoutWebConsoleTimeout
        {
            get
            {
                if (AutoLogoutWebConsoleTimeoutGet != null)
                {
                    return AutoLogoutWebConsoleTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IAutoLogoutSettings)_inner).AutoLogoutWebConsoleTimeout;
                }

                if (AutoLogoutWebConsoleTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AutoLogoutWebConsoleTimeout;
                }

                return default(TimeSpan);
            }

            set
            {
                if (AutoLogoutWebConsoleTimeoutSetTimeSpan != null)
                {
                    AutoLogoutWebConsoleTimeoutSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAutoLogoutSettings)_inner).AutoLogoutWebConsoleTimeout = value;
                    return;
                }

                if (AutoLogoutWebConsoleTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AutoLogoutWebConsoleTimeout = value;
                }

            }
        }

    }
}