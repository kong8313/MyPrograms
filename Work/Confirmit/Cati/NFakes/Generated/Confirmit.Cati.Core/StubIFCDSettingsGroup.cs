using System;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIFCDSettingsGroup : IFCDSettingsGroup 
    {
        private IFCDSettingsGroup _inner;

        public StubIFCDSettingsGroup()
        {
            _inner = null;
        }

        public IFCDSettingsGroup Inner
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

        private int _BehaviorType;
        public Func<int> BehaviorTypeGet;
        public Action<int> BehaviorTypeSetInt32;

        int IFCDSettings.BehaviorType
        {
            get
            {
                if (BehaviorTypeGet != null)
                {
                    return BehaviorTypeGet();
                } else if (_inner != null)
                {
                    return ((IFCDSettings)_inner).BehaviorType;
                }

                if (BehaviorTypeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BehaviorType;
                }

                return default(int);
            }

            set
            {
                if (BehaviorTypeSetInt32 != null)
                {
                    BehaviorTypeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IFCDSettings)_inner).BehaviorType = value;
                    return;
                }

                if (BehaviorTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BehaviorType = value;
                }

            }
        }

        private int _InterviewQuotaCellsTransactionThreshold;
        public Func<int> InterviewQuotaCellsTransactionThresholdGet;
        public Action<int> InterviewQuotaCellsTransactionThresholdSetInt32;

        int IFCDSettings.InterviewQuotaCellsTransactionThreshold
        {
            get
            {
                if (InterviewQuotaCellsTransactionThresholdGet != null)
                {
                    return InterviewQuotaCellsTransactionThresholdGet();
                } else if (_inner != null)
                {
                    return ((IFCDSettings)_inner).InterviewQuotaCellsTransactionThreshold;
                }

                if (InterviewQuotaCellsTransactionThresholdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewQuotaCellsTransactionThreshold;
                }

                return default(int);
            }

            set
            {
                if (InterviewQuotaCellsTransactionThresholdSetInt32 != null)
                {
                    InterviewQuotaCellsTransactionThresholdSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IFCDSettings)_inner).InterviewQuotaCellsTransactionThreshold = value;
                    return;
                }

                if (InterviewQuotaCellsTransactionThresholdGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewQuotaCellsTransactionThreshold = value;
                }

            }
        }

        private FcdAlgorithmType _AlgorithmType;
        public Func<FcdAlgorithmType> AlgorithmTypeGet;
        public Action<FcdAlgorithmType> AlgorithmTypeSetFcdAlgorithmType;

        FcdAlgorithmType IFCDSettings.AlgorithmType
        {
            get
            {
                if (AlgorithmTypeGet != null)
                {
                    return AlgorithmTypeGet();
                } else if (_inner != null)
                {
                    return ((IFCDSettings)_inner).AlgorithmType;
                }

                if (AlgorithmTypeSetFcdAlgorithmType == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AlgorithmType;
                }

                return default(FcdAlgorithmType);
            }

            set
            {
                if (AlgorithmTypeSetFcdAlgorithmType != null)
                {
                    AlgorithmTypeSetFcdAlgorithmType(value);
                    return;
                } else if (_inner != null)
                {
                    ((IFCDSettings)_inner).AlgorithmType = value;
                    return;
                }

                if (AlgorithmTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AlgorithmType = value;
                }

            }
        }

    }
}