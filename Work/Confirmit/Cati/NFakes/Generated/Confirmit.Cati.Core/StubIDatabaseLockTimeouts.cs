using System;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;

namespace Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation.Fakes
{
    public class StubIDatabaseLockTimeouts : IDatabaseLockTimeouts 
    {
        private IDatabaseLockTimeouts _inner;

        public StubIDatabaseLockTimeouts()
        {
            _inner = null;
        }

        public IDatabaseLockTimeouts Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _DefaultLockTimeoutInMs;
        public Func<int> DefaultLockTimeoutInMsGet;
        public Action<int> DefaultLockTimeoutInMsSetInt32;

        int IDatabaseLockTimeouts.DefaultLockTimeoutInMs
        {
            get
            {
                if (DefaultLockTimeoutInMsGet != null)
                {
                    return DefaultLockTimeoutInMsGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseLockTimeouts)_inner).DefaultLockTimeoutInMs;
                }

                if (DefaultLockTimeoutInMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultLockTimeoutInMs;
                }

                return default(int);
            }

        }

        private int _MaxLockTimeoutInMs;
        public Func<int> MaxLockTimeoutInMsGet;
        public Action<int> MaxLockTimeoutInMsSetInt32;

        int IDatabaseLockTimeouts.MaxLockTimeoutInMs
        {
            get
            {
                if (MaxLockTimeoutInMsGet != null)
                {
                    return MaxLockTimeoutInMsGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseLockTimeouts)_inner).MaxLockTimeoutInMs;
                }

                if (MaxLockTimeoutInMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxLockTimeoutInMs;
                }

                return default(int);
            }

        }

        private int _SurveyOperationTimioutInMs;
        public Func<int> SurveyOperationTimioutInMsGet;
        public Action<int> SurveyOperationTimioutInMsSetInt32;

        int IDatabaseLockTimeouts.SurveyOperationTimioutInMs
        {
            get
            {
                if (SurveyOperationTimioutInMsGet != null)
                {
                    return SurveyOperationTimioutInMsGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseLockTimeouts)_inner).SurveyOperationTimioutInMs;
                }

                if (SurveyOperationTimioutInMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyOperationTimioutInMs;
                }

                return default(int);
            }

        }

        private int _TaskLockTimeoutInMs;
        public Func<int> TaskLockTimeoutInMsGet;
        public Action<int> TaskLockTimeoutInMsSetInt32;

        int IDatabaseLockTimeouts.TaskLockTimeoutInMs
        {
            get
            {
                if (TaskLockTimeoutInMsGet != null)
                {
                    return TaskLockTimeoutInMsGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseLockTimeouts)_inner).TaskLockTimeoutInMs;
                }

                if (TaskLockTimeoutInMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TaskLockTimeoutInMs;
                }

                return default(int);
            }

        }

        private int _TimezoneUpdateLockTimeoutInMs;
        public Func<int> TimezoneUpdateLockTimeoutInMsGet;
        public Action<int> TimezoneUpdateLockTimeoutInMsSetInt32;

        int IDatabaseLockTimeouts.TimezoneUpdateLockTimeoutInMs
        {
            get
            {
                if (TimezoneUpdateLockTimeoutInMsGet != null)
                {
                    return TimezoneUpdateLockTimeoutInMsGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseLockTimeouts)_inner).TimezoneUpdateLockTimeoutInMs;
                }

                if (TimezoneUpdateLockTimeoutInMsSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TimezoneUpdateLockTimeoutInMs;
                }

                return default(int);
            }

        }

    }
}