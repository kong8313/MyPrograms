using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubICallHistoryHubSettings : ICallHistoryHubSettings 
    {
        private ICallHistoryHubSettings _inner;

        public StubICallHistoryHubSettings()
        {
            _inner = null;
        }

        public ICallHistoryHubSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _RetentionPeriod;
        public Func<int> RetentionPeriodGet;
        public Action<int> RetentionPeriodSetInt32;

        int ICallHistoryHubSettings.RetentionPeriod
        {
            get
            {
                if (RetentionPeriodGet != null)
                {
                    return RetentionPeriodGet();
                } else if (_inner != null)
                {
                    return ((ICallHistoryHubSettings)_inner).RetentionPeriod;
                }

                if (RetentionPeriodSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RetentionPeriod;
                }

                return default(int);
            }

            set
            {
                if (RetentionPeriodSetInt32 != null)
                {
                    RetentionPeriodSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallHistoryHubSettings)_inner).RetentionPeriod = value;
                    return;
                }

                if (RetentionPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RetentionPeriod = value;
                }

            }
        }

        private bool _SyncEnabled;
        public Func<bool> SyncEnabledGet;
        public Action<bool> SyncEnabledSetBoolean;

        bool ICallHistoryHubSettings.SyncEnabled
        {
            get
            {
                if (SyncEnabledGet != null)
                {
                    return SyncEnabledGet();
                } else if (_inner != null)
                {
                    return ((ICallHistoryHubSettings)_inner).SyncEnabled;
                }

                if (SyncEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SyncEnabled;
                }

                return default(bool);
            }

            set
            {
                if (SyncEnabledSetBoolean != null)
                {
                    SyncEnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallHistoryHubSettings)_inner).SyncEnabled = value;
                    return;
                }

                if (SyncEnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SyncEnabled = value;
                }

            }
        }

        private TimeSpan _SyncSleepPeriod;
        public Func<TimeSpan> SyncSleepPeriodGet;
        public Action<TimeSpan> SyncSleepPeriodSetTimeSpan;

        TimeSpan ICallHistoryHubSettings.SyncSleepPeriod
        {
            get
            {
                if (SyncSleepPeriodGet != null)
                {
                    return SyncSleepPeriodGet();
                } else if (_inner != null)
                {
                    return ((ICallHistoryHubSettings)_inner).SyncSleepPeriod;
                }

                if (SyncSleepPeriodSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SyncSleepPeriod;
                }

                return default(TimeSpan);
            }

            set
            {
                if (SyncSleepPeriodSetTimeSpan != null)
                {
                    SyncSleepPeriodSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallHistoryHubSettings)_inner).SyncSleepPeriod = value;
                    return;
                }

                if (SyncSleepPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SyncSleepPeriod = value;
                }

            }
        }

    }
}