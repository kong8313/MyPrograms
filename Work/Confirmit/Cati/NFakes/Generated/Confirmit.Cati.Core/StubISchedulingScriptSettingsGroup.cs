using System;
using Confirmit.CATI.Core.SystemSettings;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISchedulingScriptSettingsGroup : ISchedulingScriptSettingsGroup 
    {
        private ISchedulingScriptSettingsGroup _inner;

        public StubISchedulingScriptSettingsGroup()
        {
            _inner = null;
        }

        public ISchedulingScriptSettingsGroup Inner
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

        private bool _EnableRestrictedMode;
        public Func<bool> EnableRestrictedModeGet;
        public Action<bool> EnableRestrictedModeSetBoolean;

        bool ISchedulingScriptSettings.EnableRestrictedMode
        {
            get
            {
                if (EnableRestrictedModeGet != null)
                {
                    return EnableRestrictedModeGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingScriptSettings)_inner).EnableRestrictedMode;
                }

                if (EnableRestrictedModeSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableRestrictedMode;
                }

                return default(bool);
            }

            set
            {
                if (EnableRestrictedModeSetBoolean != null)
                {
                    EnableRestrictedModeSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingScriptSettings)_inner).EnableRestrictedMode = value;
                    return;
                }

                if (EnableRestrictedModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableRestrictedMode = value;
                }

            }
        }

        private int _ErrorLogSize;
        public Func<int> ErrorLogSizeGet;
        public Action<int> ErrorLogSizeSetInt32;

        int ISchedulingScriptSettings.ErrorLogSize
        {
            get
            {
                if (ErrorLogSizeGet != null)
                {
                    return ErrorLogSizeGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingScriptSettings)_inner).ErrorLogSize;
                }

                if (ErrorLogSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ErrorLogSize;
                }

                return default(int);
            }

            set
            {
                if (ErrorLogSizeSetInt32 != null)
                {
                    ErrorLogSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingScriptSettings)_inner).ErrorLogSize = value;
                    return;
                }

                if (ErrorLogSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ErrorLogSize = value;
                }

            }
        }

        private int _MaxActionsToExecute;
        public Func<int> MaxActionsToExecuteGet;
        public Action<int> MaxActionsToExecuteSetInt32;

        int ISchedulingScriptSettings.MaxActionsToExecute
        {
            get
            {
                if (MaxActionsToExecuteGet != null)
                {
                    return MaxActionsToExecuteGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingScriptSettings)_inner).MaxActionsToExecute;
                }

                if (MaxActionsToExecuteSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxActionsToExecute;
                }

                return default(int);
            }

            set
            {
                if (MaxActionsToExecuteSetInt32 != null)
                {
                    MaxActionsToExecuteSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingScriptSettings)_inner).MaxActionsToExecute = value;
                    return;
                }

                if (MaxActionsToExecuteGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxActionsToExecute = value;
                }

            }
        }

        private int _MaxParameters;
        public Func<int> MaxParametersGet;
        public Action<int> MaxParametersSetInt32;

        int ISchedulingScriptSettings.MaxParameters
        {
            get
            {
                if (MaxParametersGet != null)
                {
                    return MaxParametersGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingScriptSettings)_inner).MaxParameters;
                }

                if (MaxParametersSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxParameters;
                }

                return default(int);
            }

            set
            {
                if (MaxParametersSetInt32 != null)
                {
                    MaxParametersSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingScriptSettings)_inner).MaxParameters = value;
                    return;
                }

                if (MaxParametersGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxParameters = value;
                }

            }
        }

        private string _SecureExternalMethods;
        public Func<string> SecureExternalMethodsGet;
        public Action<string> SecureExternalMethodsSetString;

        string ISchedulingScriptSettings.SecureExternalMethods
        {
            get
            {
                if (SecureExternalMethodsGet != null)
                {
                    return SecureExternalMethodsGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingScriptSettings)_inner).SecureExternalMethods;
                }

                if (SecureExternalMethodsSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SecureExternalMethods;
                }

                return default(string);
            }

            set
            {
                if (SecureExternalMethodsSetString != null)
                {
                    SecureExternalMethodsSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingScriptSettings)_inner).SecureExternalMethods = value;
                    return;
                }

                if (SecureExternalMethodsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SecureExternalMethods = value;
                }

            }
        }

        private bool _UseDirectDbAccess;
        public Func<bool> UseDirectDbAccessGet;
        public Action<bool> UseDirectDbAccessSetBoolean;

        bool ISchedulingScriptSettings.UseDirectDbAccess
        {
            get
            {
                if (UseDirectDbAccessGet != null)
                {
                    return UseDirectDbAccessGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingScriptSettings)_inner).UseDirectDbAccess;
                }

                if (UseDirectDbAccessSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UseDirectDbAccess;
                }

                return default(bool);
            }

            set
            {
                if (UseDirectDbAccessSetBoolean != null)
                {
                    UseDirectDbAccessSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISchedulingScriptSettings)_inner).UseDirectDbAccess = value;
                    return;
                }

                if (UseDirectDbAccessGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _UseDirectDbAccess = value;
                }

            }
        }

        private List<string> _SecureExternalMethodList;
        public Func<List<string>> SecureExternalMethodListGet;
        public Action<List<string>> SecureExternalMethodListSetListOfString;

        List<string> ISchedulingScriptSettings.SecureExternalMethodList
        {
            get
            {
                if (SecureExternalMethodListGet != null)
                {
                    return SecureExternalMethodListGet();
                } else if (_inner != null)
                {
                    return ((ISchedulingScriptSettings)_inner).SecureExternalMethodList;
                }

                if (SecureExternalMethodListSetListOfString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SecureExternalMethodList;
                }

                return default(List<string>);
            }

        }

    }
}