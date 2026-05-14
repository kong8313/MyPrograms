using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISiteSettings : ISiteSettings 
    {
        private ISiteSettings _inner;

        public StubISiteSettings()
        {
            _inner = null;
        }

        public ISiteSettings Inner
        {
            set {_inner = value;} get {return _inner;}
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