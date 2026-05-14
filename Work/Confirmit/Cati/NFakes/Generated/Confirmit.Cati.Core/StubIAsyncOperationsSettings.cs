using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIAsyncOperationsSettings : IAsyncOperationsSettings 
    {
        private IAsyncOperationsSettings _inner;

        public StubIAsyncOperationsSettings()
        {
            _inner = null;
        }

        public IAsyncOperationsSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _DelayBetweenRetriesInSeconds;
        public Func<int> DelayBetweenRetriesInSecondsGet;
        public Action<int> DelayBetweenRetriesInSecondsSetInt32;

        int IAsyncOperationsSettings.DelayBetweenRetriesInSeconds
        {
            get
            {
                if (DelayBetweenRetriesInSecondsGet != null)
                {
                    return DelayBetweenRetriesInSecondsGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationsSettings)_inner).DelayBetweenRetriesInSeconds;
                }

                if (DelayBetweenRetriesInSecondsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DelayBetweenRetriesInSeconds;
                }

                return default(int);
            }

            set
            {
                if (DelayBetweenRetriesInSecondsSetInt32 != null)
                {
                    DelayBetweenRetriesInSecondsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationsSettings)_inner).DelayBetweenRetriesInSeconds = value;
                    return;
                }

                if (DelayBetweenRetriesInSecondsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DelayBetweenRetriesInSeconds = value;
                }

            }
        }

        private int _MaximumRunningAsyncOperations;
        public Func<int> MaximumRunningAsyncOperationsGet;
        public Action<int> MaximumRunningAsyncOperationsSetInt32;

        int IAsyncOperationsSettings.MaximumRunningAsyncOperations
        {
            get
            {
                if (MaximumRunningAsyncOperationsGet != null)
                {
                    return MaximumRunningAsyncOperationsGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationsSettings)_inner).MaximumRunningAsyncOperations;
                }

                if (MaximumRunningAsyncOperationsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaximumRunningAsyncOperations;
                }

                return default(int);
            }

            set
            {
                if (MaximumRunningAsyncOperationsSetInt32 != null)
                {
                    MaximumRunningAsyncOperationsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationsSettings)_inner).MaximumRunningAsyncOperations = value;
                    return;
                }

                if (MaximumRunningAsyncOperationsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaximumRunningAsyncOperations = value;
                }

            }
        }

        private int _NumberOfRetries;
        public Func<int> NumberOfRetriesGet;
        public Action<int> NumberOfRetriesSetInt32;

        int IAsyncOperationsSettings.NumberOfRetries
        {
            get
            {
                if (NumberOfRetriesGet != null)
                {
                    return NumberOfRetriesGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationsSettings)_inner).NumberOfRetries;
                }

                if (NumberOfRetriesSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NumberOfRetries;
                }

                return default(int);
            }

            set
            {
                if (NumberOfRetriesSetInt32 != null)
                {
                    NumberOfRetriesSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationsSettings)_inner).NumberOfRetries = value;
                    return;
                }

                if (NumberOfRetriesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NumberOfRetries = value;
                }

            }
        }

        private int _TimeToTreatOperationHangedInMinutes;
        public Func<int> TimeToTreatOperationHangedInMinutesGet;
        public Action<int> TimeToTreatOperationHangedInMinutesSetInt32;

        int IAsyncOperationsSettings.TimeToTreatOperationHangedInMinutes
        {
            get
            {
                if (TimeToTreatOperationHangedInMinutesGet != null)
                {
                    return TimeToTreatOperationHangedInMinutesGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationsSettings)_inner).TimeToTreatOperationHangedInMinutes;
                }

                if (TimeToTreatOperationHangedInMinutesSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TimeToTreatOperationHangedInMinutes;
                }

                return default(int);
            }

            set
            {
                if (TimeToTreatOperationHangedInMinutesSetInt32 != null)
                {
                    TimeToTreatOperationHangedInMinutesSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationsSettings)_inner).TimeToTreatOperationHangedInMinutes = value;
                    return;
                }

                if (TimeToTreatOperationHangedInMinutesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TimeToTreatOperationHangedInMinutes = value;
                }

            }
        }

    }
}