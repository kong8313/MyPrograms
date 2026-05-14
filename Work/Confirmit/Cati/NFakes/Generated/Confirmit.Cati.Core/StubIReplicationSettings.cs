using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIReplicationSettings : IReplicationSettings 
    {
        private IReplicationSettings _inner;

        public StubIReplicationSettings()
        {
            _inner = null;
        }

        public IReplicationSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _BackgroundReplicationSleepPeriod;
        public Func<int> BackgroundReplicationSleepPeriodGet;
        public Action<int> BackgroundReplicationSleepPeriodSetInt32;

        int IReplicationSettings.BackgroundReplicationSleepPeriod
        {
            get
            {
                if (BackgroundReplicationSleepPeriodGet != null)
                {
                    return BackgroundReplicationSleepPeriodGet();
                } else if (_inner != null)
                {
                    return ((IReplicationSettings)_inner).BackgroundReplicationSleepPeriod;
                }

                if (BackgroundReplicationSleepPeriodSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BackgroundReplicationSleepPeriod;
                }

                return default(int);
            }

            set
            {
                if (BackgroundReplicationSleepPeriodSetInt32 != null)
                {
                    BackgroundReplicationSleepPeriodSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReplicationSettings)_inner).BackgroundReplicationSleepPeriod = value;
                    return;
                }

                if (BackgroundReplicationSleepPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BackgroundReplicationSleepPeriod = value;
                }

            }
        }

        private int _ForceReplicationLockTimeout;
        public Func<int> ForceReplicationLockTimeoutGet;
        public Action<int> ForceReplicationLockTimeoutSetInt32;

        int IReplicationSettings.ForceReplicationLockTimeout
        {
            get
            {
                if (ForceReplicationLockTimeoutGet != null)
                {
                    return ForceReplicationLockTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IReplicationSettings)_inner).ForceReplicationLockTimeout;
                }

                if (ForceReplicationLockTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ForceReplicationLockTimeout;
                }

                return default(int);
            }

            set
            {
                if (ForceReplicationLockTimeoutSetInt32 != null)
                {
                    ForceReplicationLockTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReplicationSettings)_inner).ForceReplicationLockTimeout = value;
                    return;
                }

                if (ForceReplicationLockTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ForceReplicationLockTimeout = value;
                }

            }
        }

    }
}