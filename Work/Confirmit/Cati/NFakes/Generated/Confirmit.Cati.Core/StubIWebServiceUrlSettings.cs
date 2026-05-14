using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIWebServiceUrlSettings : IWebServiceUrlSettings 
    {
        private IWebServiceUrlSettings _inner;

        public StubIWebServiceUrlSettings()
        {
            _inner = null;
        }

        public IWebServiceUrlSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _Authoring;
        public Func<string> AuthoringGet;
        public Action<string> AuthoringSetString;

        string IWebServiceUrlSettings.Authoring
        {
            get
            {
                if (AuthoringGet != null)
                {
                    return AuthoringGet();
                } else if (_inner != null)
                {
                    return ((IWebServiceUrlSettings)_inner).Authoring;
                }

                if (AuthoringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Authoring;
                }

                return default(string);
            }

            set
            {
                if (AuthoringSetString != null)
                {
                    AuthoringSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWebServiceUrlSettings)_inner).Authoring = value;
                    return;
                }

                if (AuthoringGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Authoring = value;
                }

            }
        }

        private string _SurveyData;
        public Func<string> SurveyDataGet;
        public Action<string> SurveyDataSetString;

        string IWebServiceUrlSettings.SurveyData
        {
            get
            {
                if (SurveyDataGet != null)
                {
                    return SurveyDataGet();
                } else if (_inner != null)
                {
                    return ((IWebServiceUrlSettings)_inner).SurveyData;
                }

                if (SurveyDataSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SurveyData;
                }

                return default(string);
            }

            set
            {
                if (SurveyDataSetString != null)
                {
                    SurveyDataSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWebServiceUrlSettings)_inner).SurveyData = value;
                    return;
                }

                if (SurveyDataGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SurveyData = value;
                }

            }
        }

    }
}