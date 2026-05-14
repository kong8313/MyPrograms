using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubITimeZoneBalancingSettingsGroup : ITimeZoneBalancingSettingsGroup 
    {
        private ITimeZoneBalancingSettingsGroup _inner;

        public StubITimeZoneBalancingSettingsGroup()
        {
            _inner = null;
        }

        public ITimeZoneBalancingSettingsGroup Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnChangedDelegate();
        public OnChangedDelegate OnChanged;

        void ISystemSettingsNotifyChanged.OnChanged()
        {

            if (OnChanged != null)
            {
                OnChanged();
            } else if (_inner != null)
            {
                ((ISystemSettingsNotifyChanged)_inner).OnChanged();
            }
        }

        private int _EndOfShiftThreshold;
        public Func<int> EndOfShiftThresholdGet;
        public Action<int> EndOfShiftThresholdSetInt32;

        int ITimeZoneBalancingSettings.EndOfShiftThreshold
        {
            get
            {
                if (EndOfShiftThresholdGet != null)
                {
                    return EndOfShiftThresholdGet();
                } else if (_inner != null)
                {
                    return ((ITimeZoneBalancingSettings)_inner).EndOfShiftThreshold;
                }

                if (EndOfShiftThresholdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EndOfShiftThreshold;
                }

                return default(int);
            }

            set
            {
                if (EndOfShiftThresholdSetInt32 != null)
                {
                    EndOfShiftThresholdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ITimeZoneBalancingSettings)_inner).EndOfShiftThreshold = value;
                    return;
                }

                if (EndOfShiftThresholdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EndOfShiftThreshold = value;
                }

            }
        }

    }
}