using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIQuotaBalancingSettings : IQuotaBalancingSettings 
    {
        private IQuotaBalancingSettings _inner;

        public StubIQuotaBalancingSettings()
        {
            _inner = null;
        }

        public IQuotaBalancingSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _MaxCellsCount;
        public Func<int> MaxCellsCountGet;
        public Action<int> MaxCellsCountSetInt32;

        int IQuotaBalancingSettings.MaxCellsCount
        {
            get
            {
                if (MaxCellsCountGet != null)
                {
                    return MaxCellsCountGet();
                } else if (_inner != null)
                {
                    return ((IQuotaBalancingSettings)_inner).MaxCellsCount;
                }

                if (MaxCellsCountSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxCellsCount;
                }

                return default(int);
            }

            set
            {
                if (MaxCellsCountSetInt32 != null)
                {
                    MaxCellsCountSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IQuotaBalancingSettings)_inner).MaxCellsCount = value;
                    return;
                }

                if (MaxCellsCountGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxCellsCount = value;
                }

            }
        }

        private TimeSpan _MinDelay;
        public Func<TimeSpan> MinDelayGet;
        public Action<TimeSpan> MinDelaySetTimeSpan;

        TimeSpan IQuotaBalancingSettings.MinDelay
        {
            get
            {
                if (MinDelayGet != null)
                {
                    return MinDelayGet();
                } else if (_inner != null)
                {
                    return ((IQuotaBalancingSettings)_inner).MinDelay;
                }

                if (MinDelaySetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MinDelay;
                }

                return default(TimeSpan);
            }

            set
            {
                if (MinDelaySetTimeSpan != null)
                {
                    MinDelaySetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IQuotaBalancingSettings)_inner).MinDelay = value;
                    return;
                }

                if (MinDelayGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MinDelay = value;
                }

            }
        }

        private TimeSpan _TotalPeriod;
        public Func<TimeSpan> TotalPeriodGet;
        public Action<TimeSpan> TotalPeriodSetTimeSpan;

        TimeSpan IQuotaBalancingSettings.TotalPeriod
        {
            get
            {
                if (TotalPeriodGet != null)
                {
                    return TotalPeriodGet();
                } else if (_inner != null)
                {
                    return ((IQuotaBalancingSettings)_inner).TotalPeriod;
                }

                if (TotalPeriodSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TotalPeriod;
                }

                return default(TimeSpan);
            }

            set
            {
                if (TotalPeriodSetTimeSpan != null)
                {
                    TotalPeriodSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IQuotaBalancingSettings)_inner).TotalPeriod = value;
                    return;
                }

                if (TotalPeriodGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TotalPeriod = value;
                }

            }
        }

    }
}