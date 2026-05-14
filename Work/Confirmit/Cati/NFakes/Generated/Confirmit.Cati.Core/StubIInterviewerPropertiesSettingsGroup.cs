using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIInterviewerPropertiesSettingsGroup : IInterviewerPropertiesSettingsGroup 
    {
        private IInterviewerPropertiesSettingsGroup _inner;

        public StubIInterviewerPropertiesSettingsGroup()
        {
            _inner = null;
        }

        public IInterviewerPropertiesSettingsGroup Inner
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

        private string _AttributesList;
        public Func<string> AttributesListGet;
        public Action<string> AttributesListSetString;

        string IInterviewerPropertiesSettings.AttributesList
        {
            get
            {
                if (AttributesListGet != null)
                {
                    return AttributesListGet();
                } else if (_inner != null)
                {
                    return ((IInterviewerPropertiesSettings)_inner).AttributesList;
                }

                if (AttributesListSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AttributesList;
                }

                return default(string);
            }

            set
            {
                if (AttributesListSetString != null)
                {
                    AttributesListSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IInterviewerPropertiesSettings)_inner).AttributesList = value;
                    return;
                }

                if (AttributesListGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AttributesList = value;
                }

            }
        }

    }
}