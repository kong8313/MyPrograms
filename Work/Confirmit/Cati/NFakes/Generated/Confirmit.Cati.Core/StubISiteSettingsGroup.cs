using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISiteSettingsGroup : ISiteSettingsGroup 
    {
        private ISiteSettingsGroup _inner;

        public StubISiteSettingsGroup()
        {
            _inner = null;
        }

        public ISiteSettingsGroup Inner
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

        private string _StartSurveyURL;
        public Func<string> StartSurveyURLGet;
        public Action<string> StartSurveyURLSetString;

        string ISiteSettings.StartSurveyURL
        {
            get
            {
                if (StartSurveyURLGet != null)
                {
                    return StartSurveyURLGet();
                } else if (_inner != null)
                {
                    return ((ISiteSettings)_inner).StartSurveyURL;
                }

                if (StartSurveyURLSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _StartSurveyURL;
                }

                return default(string);
            }

            set
            {
                if (StartSurveyURLSetString != null)
                {
                    StartSurveyURLSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISiteSettings)_inner).StartSurveyURL = value;
                    return;
                }

                if (StartSurveyURLGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _StartSurveyURL = value;
                }

            }
        }

    }
}