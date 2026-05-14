using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIRetryingServiceSettings : IRetryingServiceSettings 
    {
        private IRetryingServiceSettings _inner;

        public StubIRetryingServiceSettings()
        {
            _inner = null;
        }

        public IRetryingServiceSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _DelayBetweenRetriesInMilliseconds;
        public Func<int> DelayBetweenRetriesInMillisecondsGet;
        public Action<int> DelayBetweenRetriesInMillisecondsSetInt32;

        int IRetryingServiceSettings.DelayBetweenRetriesInMilliseconds
        {
            get
            {
                if (DelayBetweenRetriesInMillisecondsGet != null)
                {
                    return DelayBetweenRetriesInMillisecondsGet();
                } else if (_inner != null)
                {
                    return ((IRetryingServiceSettings)_inner).DelayBetweenRetriesInMilliseconds;
                }

                if (DelayBetweenRetriesInMillisecondsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DelayBetweenRetriesInMilliseconds;
                }

                return default(int);
            }

            set
            {
                if (DelayBetweenRetriesInMillisecondsSetInt32 != null)
                {
                    DelayBetweenRetriesInMillisecondsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRetryingServiceSettings)_inner).DelayBetweenRetriesInMilliseconds = value;
                    return;
                }

                if (DelayBetweenRetriesInMillisecondsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DelayBetweenRetriesInMilliseconds = value;
                }

            }
        }

        private int _NumberOfRetryAttempts;
        public Func<int> NumberOfRetryAttemptsGet;
        public Action<int> NumberOfRetryAttemptsSetInt32;

        int IRetryingServiceSettings.NumberOfRetryAttempts
        {
            get
            {
                if (NumberOfRetryAttemptsGet != null)
                {
                    return NumberOfRetryAttemptsGet();
                } else if (_inner != null)
                {
                    return ((IRetryingServiceSettings)_inner).NumberOfRetryAttempts;
                }

                if (NumberOfRetryAttemptsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NumberOfRetryAttempts;
                }

                return default(int);
            }

            set
            {
                if (NumberOfRetryAttemptsSetInt32 != null)
                {
                    NumberOfRetryAttemptsSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRetryingServiceSettings)_inner).NumberOfRetryAttempts = value;
                    return;
                }

                if (NumberOfRetryAttemptsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _NumberOfRetryAttempts = value;
                }

            }
        }

    }
}