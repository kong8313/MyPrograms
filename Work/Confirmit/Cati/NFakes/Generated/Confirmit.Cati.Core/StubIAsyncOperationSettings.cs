using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIAsyncOperationSettings : IAsyncOperationSettings 
    {
        private IAsyncOperationSettings _inner;

        public StubIAsyncOperationSettings()
        {
            _inner = null;
        }

        public IAsyncOperationSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _ActivatePortionSize;
        public Func<int> ActivatePortionSizeGet;
        public Action<int> ActivatePortionSizeSetInt32;

        int IAsyncOperationSettings.ActivatePortionSize
        {
            get
            {
                if (ActivatePortionSizeGet != null)
                {
                    return ActivatePortionSizeGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationSettings)_inner).ActivatePortionSize;
                }

                if (ActivatePortionSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ActivatePortionSize;
                }

                return default(int);
            }

            set
            {
                if (ActivatePortionSizeSetInt32 != null)
                {
                    ActivatePortionSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationSettings)_inner).ActivatePortionSize = value;
                    return;
                }

                if (ActivatePortionSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ActivatePortionSize = value;
                }

            }
        }

        private int _AddSamplePortionSize;
        public Func<int> AddSamplePortionSizeGet;
        public Action<int> AddSamplePortionSizeSetInt32;

        int IAsyncOperationSettings.AddSamplePortionSize
        {
            get
            {
                if (AddSamplePortionSizeGet != null)
                {
                    return AddSamplePortionSizeGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationSettings)_inner).AddSamplePortionSize;
                }

                if (AddSamplePortionSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AddSamplePortionSize;
                }

                return default(int);
            }

            set
            {
                if (AddSamplePortionSizeSetInt32 != null)
                {
                    AddSamplePortionSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationSettings)_inner).AddSamplePortionSize = value;
                    return;
                }

                if (AddSamplePortionSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AddSamplePortionSize = value;
                }

            }
        }

        private int _AsyncOperationCleanTimeoutInHours;
        public Func<int> AsyncOperationCleanTimeoutInHoursGet;
        public Action<int> AsyncOperationCleanTimeoutInHoursSetInt32;

        int IAsyncOperationSettings.AsyncOperationCleanTimeoutInHours
        {
            get
            {
                if (AsyncOperationCleanTimeoutInHoursGet != null)
                {
                    return AsyncOperationCleanTimeoutInHoursGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationSettings)_inner).AsyncOperationCleanTimeoutInHours;
                }

                if (AsyncOperationCleanTimeoutInHoursSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AsyncOperationCleanTimeoutInHours;
                }

                return default(int);
            }

            set
            {
                if (AsyncOperationCleanTimeoutInHoursSetInt32 != null)
                {
                    AsyncOperationCleanTimeoutInHoursSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationSettings)_inner).AsyncOperationCleanTimeoutInHours = value;
                    return;
                }

                if (AsyncOperationCleanTimeoutInHoursGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AsyncOperationCleanTimeoutInHours = value;
                }

            }
        }

        private int _MovePortionSize;
        public Func<int> MovePortionSizeGet;
        public Action<int> MovePortionSizeSetInt32;

        int IAsyncOperationSettings.MovePortionSize
        {
            get
            {
                if (MovePortionSizeGet != null)
                {
                    return MovePortionSizeGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationSettings)_inner).MovePortionSize;
                }

                if (MovePortionSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MovePortionSize;
                }

                return default(int);
            }

            set
            {
                if (MovePortionSizeSetInt32 != null)
                {
                    MovePortionSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationSettings)_inner).MovePortionSize = value;
                    return;
                }

                if (MovePortionSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MovePortionSize = value;
                }

            }
        }

        private TimeSpan _RestoreSurveySqlTimeout;
        public Func<TimeSpan> RestoreSurveySqlTimeoutGet;
        public Action<TimeSpan> RestoreSurveySqlTimeoutSetTimeSpan;

        TimeSpan IAsyncOperationSettings.RestoreSurveySqlTimeout
        {
            get
            {
                if (RestoreSurveySqlTimeoutGet != null)
                {
                    return RestoreSurveySqlTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IAsyncOperationSettings)_inner).RestoreSurveySqlTimeout;
                }

                if (RestoreSurveySqlTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RestoreSurveySqlTimeout;
                }

                return default(TimeSpan);
            }

            set
            {
                if (RestoreSurveySqlTimeoutSetTimeSpan != null)
                {
                    RestoreSurveySqlTimeoutSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IAsyncOperationSettings)_inner).RestoreSurveySqlTimeout = value;
                    return;
                }

                if (RestoreSurveySqlTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RestoreSurveySqlTimeout = value;
                }

            }
        }

    }
}