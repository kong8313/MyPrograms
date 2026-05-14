using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIReviewerSettings : IReviewerSettings 
    {
        private IReviewerSettings _inner;

        public StubIReviewerSettings()
        {
            _inner = null;
        }

        public IReviewerSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _LimitOfAmountOfInterviewsPerSession;
        public Func<string> LimitOfAmountOfInterviewsPerSessionGet;
        public Action<string> LimitOfAmountOfInterviewsPerSessionSetString;

        string IReviewerSettings.LimitOfAmountOfInterviewsPerSession
        {
            get
            {
                if (LimitOfAmountOfInterviewsPerSessionGet != null)
                {
                    return LimitOfAmountOfInterviewsPerSessionGet();
                } else if (_inner != null)
                {
                    return ((IReviewerSettings)_inner).LimitOfAmountOfInterviewsPerSession;
                }

                if (LimitOfAmountOfInterviewsPerSessionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LimitOfAmountOfInterviewsPerSession;
                }

                return default(string);
            }

            set
            {
                if (LimitOfAmountOfInterviewsPerSessionSetString != null)
                {
                    LimitOfAmountOfInterviewsPerSessionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReviewerSettings)_inner).LimitOfAmountOfInterviewsPerSession = value;
                    return;
                }

                if (LimitOfAmountOfInterviewsPerSessionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _LimitOfAmountOfInterviewsPerSession = value;
                }

            }
        }

        private string _SessionUrlTemplate;
        public Func<string> SessionUrlTemplateGet;
        public Action<string> SessionUrlTemplateSetString;

        string IReviewerSettings.SessionUrlTemplate
        {
            get
            {
                if (SessionUrlTemplateGet != null)
                {
                    return SessionUrlTemplateGet();
                } else if (_inner != null)
                {
                    return ((IReviewerSettings)_inner).SessionUrlTemplate;
                }

                if (SessionUrlTemplateSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SessionUrlTemplate;
                }

                return default(string);
            }

            set
            {
                if (SessionUrlTemplateSetString != null)
                {
                    SessionUrlTemplateSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IReviewerSettings)_inner).SessionUrlTemplate = value;
                    return;
                }

                if (SessionUrlTemplateGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SessionUrlTemplate = value;
                }

            }
        }

    }
}