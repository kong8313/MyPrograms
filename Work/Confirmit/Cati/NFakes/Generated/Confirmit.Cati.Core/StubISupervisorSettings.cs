using System;
using Confirmit.CATI.Core.SystemSettings.Toggle;

namespace Confirmit.CATI.Core.SystemSettings.Toggle.Fakes
{
    public class StubISupervisorSettings : ISupervisorSettings 
    {
        private ISupervisorSettings _inner;

        public StubISupervisorSettings()
        {
            _inner = null;
        }

        public ISupervisorSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _EnableScriptErrorsLogging;
        public Func<bool> EnableScriptErrorsLoggingGet;
        public Action<bool> EnableScriptErrorsLoggingSetBoolean;

        bool ISupervisorSettings.EnableScriptErrorsLogging
        {
            get
            {
                if (EnableScriptErrorsLoggingGet != null)
                {
                    return EnableScriptErrorsLoggingGet();
                } else if (_inner != null)
                {
                    return ((ISupervisorSettings)_inner).EnableScriptErrorsLogging;
                }

                if (EnableScriptErrorsLoggingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableScriptErrorsLogging;
                }

                return default(bool);
            }

            set
            {
                if (EnableScriptErrorsLoggingSetBoolean != null)
                {
                    EnableScriptErrorsLoggingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISupervisorSettings)_inner).EnableScriptErrorsLogging = value;
                    return;
                }

                if (EnableScriptErrorsLoggingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableScriptErrorsLogging = value;
                }

            }
        }

    }
}