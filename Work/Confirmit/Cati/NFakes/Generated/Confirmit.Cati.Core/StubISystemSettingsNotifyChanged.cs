using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISystemSettingsNotifyChanged : ISystemSettingsNotifyChanged 
    {
        private ISystemSettingsNotifyChanged _inner;

        public StubISystemSettingsNotifyChanged()
        {
            _inner = null;
        }

        public ISystemSettingsNotifyChanged Inner
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

    }
}