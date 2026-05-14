using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubILoggingSettings : ILoggingSettings 
    {
        private ILoggingSettings _inner;

        public StubILoggingSettings()
        {
            _inner = null;
        }

        public ILoggingSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _EnableReceivingClientErrors;
        public Func<bool> EnableReceivingClientErrorsGet;
        public Action<bool> EnableReceivingClientErrorsSetBoolean;

        bool ILoggingSettings.EnableReceivingClientErrors
        {
            get
            {
                if (EnableReceivingClientErrorsGet != null)
                {
                    return EnableReceivingClientErrorsGet();
                } else if (_inner != null)
                {
                    return ((ILoggingSettings)_inner).EnableReceivingClientErrors;
                }

                if (EnableReceivingClientErrorsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableReceivingClientErrors;
                }

                return default(bool);
            }

            set
            {
                if (EnableReceivingClientErrorsSetBoolean != null)
                {
                    EnableReceivingClientErrorsSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ILoggingSettings)_inner).EnableReceivingClientErrors = value;
                    return;
                }

                if (EnableReceivingClientErrorsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableReceivingClientErrors = value;
                }

            }
        }

        private bool _TraceVerbose;
        public Func<bool> TraceVerboseGet;
        public Action<bool> TraceVerboseSetBoolean;

        bool ILoggingSettings.TraceVerbose
        {
            get
            {
                if (TraceVerboseGet != null)
                {
                    return TraceVerboseGet();
                } else if (_inner != null)
                {
                    return ((ILoggingSettings)_inner).TraceVerbose;
                }

                if (TraceVerboseSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TraceVerbose;
                }

                return default(bool);
            }

            set
            {
                if (TraceVerboseSetBoolean != null)
                {
                    TraceVerboseSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ILoggingSettings)_inner).TraceVerbose = value;
                    return;
                }

                if (TraceVerboseGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TraceVerbose = value;
                }

            }
        }

    }
}