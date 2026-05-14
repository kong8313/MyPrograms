using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubICallGroupSettingsGroup : ICallGroupSettingsGroup 
    {
        private ICallGroupSettingsGroup _inner;

        public StubICallGroupSettingsGroup()
        {
            _inner = null;
        }

        public ICallGroupSettingsGroup Inner
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

        bool ICallGroupSettings.Enabled
        {
            get
            {
                if (EnabledGet != null)
                {
                    return EnabledGet();
                } else if (_inner != null)
                {
                    return ((ICallGroupSettings)_inner).Enabled;
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
                    ((ICallGroupSettings)_inner).Enabled = value;
                    return;
                }

                if (EnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Enabled = value;
                }

            }
        }

        private bool _EnabledForNewSurveys;
        public Func<bool> EnabledForNewSurveysGet;
        public Action<bool> EnabledForNewSurveysSetBoolean;

        bool ICallGroupSettings.EnabledForNewSurveys
        {
            get
            {
                if (EnabledForNewSurveysGet != null)
                {
                    return EnabledForNewSurveysGet();
                } else if (_inner != null)
                {
                    return ((ICallGroupSettings)_inner).EnabledForNewSurveys;
                }

                if (EnabledForNewSurveysSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnabledForNewSurveys;
                }

                return default(bool);
            }

            set
            {
                if (EnabledForNewSurveysSetBoolean != null)
                {
                    EnabledForNewSurveysSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICallGroupSettings)_inner).EnabledForNewSurveys = value;
                    return;
                }

                if (EnabledForNewSurveysGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnabledForNewSurveys = value;
                }

            }
        }

    }
}