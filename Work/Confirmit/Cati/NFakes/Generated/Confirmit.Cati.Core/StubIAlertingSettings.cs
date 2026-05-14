using System;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Alerting;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIAlertingSettings : IAlertingSettings 
    {
        private IAlertingSettings _inner;

        public StubIAlertingSettings()
        {
            _inner = null;
        }

        public IAlertingSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private INoCallsSettings _NoCalls;
        public Func<INoCallsSettings> NoCallsGet;
        public Action<INoCallsSettings> NoCallsSetINoCallsSettings;

        INoCallsSettings IAlertingSettings.NoCalls
        {
            get
            {
                if (NoCallsGet != null)
                {
                    return NoCallsGet();
                } else if (_inner != null)
                {
                    return ((IAlertingSettings)_inner).NoCalls;
                }

                if (NoCallsSetINoCallsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _NoCalls;
                }

                return default(INoCallsSettings);
            }

        }

        private ISchedulingErrorsSettings _SchedulingErrors;
        public Func<ISchedulingErrorsSettings> SchedulingErrorsGet;
        public Action<ISchedulingErrorsSettings> SchedulingErrorsSetISchedulingErrorsSettings;

        ISchedulingErrorsSettings IAlertingSettings.SchedulingErrors
        {
            get
            {
                if (SchedulingErrorsGet != null)
                {
                    return SchedulingErrorsGet();
                } else if (_inner != null)
                {
                    return ((IAlertingSettings)_inner).SchedulingErrors;
                }

                if (SchedulingErrorsSetISchedulingErrorsSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SchedulingErrors;
                }

                return default(ISchedulingErrorsSettings);
            }

        }

        private ISelectingStateSettings _SelectingState;
        public Func<ISelectingStateSettings> SelectingStateGet;
        public Action<ISelectingStateSettings> SelectingStateSetISelectingStateSettings;

        ISelectingStateSettings IAlertingSettings.SelectingState
        {
            get
            {
                if (SelectingStateGet != null)
                {
                    return SelectingStateGet();
                } else if (_inner != null)
                {
                    return ((IAlertingSettings)_inner).SelectingState;
                }

                if (SelectingStateSetISelectingStateSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SelectingState;
                }

                return default(ISelectingStateSettings);
            }

        }

        private IWaitingStateSettings _WaitingState;
        public Func<IWaitingStateSettings> WaitingStateGet;
        public Action<IWaitingStateSettings> WaitingStateSetIWaitingStateSettings;

        IWaitingStateSettings IAlertingSettings.WaitingState
        {
            get
            {
                if (WaitingStateGet != null)
                {
                    return WaitingStateGet();
                } else if (_inner != null)
                {
                    return ((IAlertingSettings)_inner).WaitingState;
                }

                if (WaitingStateSetIWaitingStateSettings == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _WaitingState;
                }

                return default(IWaitingStateSettings);
            }

        }

    }
}