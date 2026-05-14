using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIMultipleAssignmentsSettingsGroup : IMultipleAssignmentsSettingsGroup 
    {
        private IMultipleAssignmentsSettingsGroup _inner;

        public StubIMultipleAssignmentsSettingsGroup()
        {
            _inner = null;
        }

        public IMultipleAssignmentsSettingsGroup Inner
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

        private bool _Enabled;
        public Func<bool> EnabledGet;
        public Action<bool> EnabledSetBoolean;

        bool IMultipleAssignmentsSettings.Enabled
        {
            get
            {
                if (EnabledGet != null)
                {
                    return EnabledGet();
                } else if (_inner != null)
                {
                    return ((IMultipleAssignmentsSettings)_inner).Enabled;
                }

                if (EnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Enabled;
                }

                return default(bool);
            }

            set
            {
                if (EnabledSetBoolean != null)
                {
                    EnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMultipleAssignmentsSettings)_inner).Enabled = value;
                    return;
                }

                if (EnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Enabled = value;
                }

            }
        }

    }
}