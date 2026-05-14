using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubICallManagementSettingsGroup : ICallManagementSettingsGroup 
    {
        private ICallManagementSettingsGroup _inner;

        public StubICallManagementSettingsGroup()
        {
            _inner = null;
        }

        public ICallManagementSettingsGroup Inner
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

        private int _ExportCallsLimit;
        public Func<int> ExportCallsLimitGet;
        public Action<int> ExportCallsLimitSetInt32;

        int ICallManagementSettings.ExportCallsLimit
        {
            get
            {
                if (ExportCallsLimitGet != null)
                {
                    return ExportCallsLimitGet();
                } else if (_inner != null)
                {
                    return ((ICallManagementSettings)_inner).ExportCallsLimit;
                }

                if (ExportCallsLimitSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ExportCallsLimit;
                }

                return default(int);
            }

            set
            {
                if (ExportCallsLimitSetInt32 != null)
                {
                    ExportCallsLimitSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallManagementSettings)_inner).ExportCallsLimit = value;
                    return;
                }

                if (ExportCallsLimitGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ExportCallsLimit = value;
                }

            }
        }

        private int _MaximumConfirmitVariables;
        public Func<int> MaximumConfirmitVariablesGet;
        public Action<int> MaximumConfirmitVariablesSetInt32;

        int ICallManagementSettings.MaximumConfirmitVariables
        {
            get
            {
                if (MaximumConfirmitVariablesGet != null)
                {
                    return MaximumConfirmitVariablesGet();
                } else if (_inner != null)
                {
                    return ((ICallManagementSettings)_inner).MaximumConfirmitVariables;
                }

                if (MaximumConfirmitVariablesSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaximumConfirmitVariables;
                }

                return default(int);
            }

            set
            {
                if (MaximumConfirmitVariablesSetInt32 != null)
                {
                    MaximumConfirmitVariablesSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallManagementSettings)_inner).MaximumConfirmitVariables = value;
                    return;
                }

                if (MaximumConfirmitVariablesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaximumConfirmitVariables = value;
                }

            }
        }

        private int _PageSize;
        public Func<int> PageSizeGet;
        public Action<int> PageSizeSetInt32;

        int ICallManagementSettings.PageSize
        {
            get
            {
                if (PageSizeGet != null)
                {
                    return PageSizeGet();
                } else if (_inner != null)
                {
                    return ((ICallManagementSettings)_inner).PageSize;
                }

                if (PageSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _PageSize;
                }

                return default(int);
            }

            set
            {
                if (PageSizeSetInt32 != null)
                {
                    PageSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallManagementSettings)_inner).PageSize = value;
                    return;
                }

                if (PageSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _PageSize = value;
                }

            }
        }

    }
}