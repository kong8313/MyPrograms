using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubICacheCallsSettings : ICacheCallsSettings 
    {
        private ICacheCallsSettings _inner;

        public StubICacheCallsSettings()
        {
            _inner = null;
        }

        public ICacheCallsSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _InterviewsCountPerPerson;
        public Func<int> InterviewsCountPerPersonGet;
        public Action<int> InterviewsCountPerPersonSetInt32;

        int ICacheCallsSettings.InterviewsCountPerPerson
        {
            get
            {
                if (InterviewsCountPerPersonGet != null)
                {
                    return InterviewsCountPerPersonGet();
                } else if (_inner != null)
                {
                    return ((ICacheCallsSettings)_inner).InterviewsCountPerPerson;
                }

                if (InterviewsCountPerPersonSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewsCountPerPerson;
                }

                return default(int);
            }

            set
            {
                if (InterviewsCountPerPersonSetInt32 != null)
                {
                    InterviewsCountPerPersonSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICacheCallsSettings)_inner).InterviewsCountPerPerson = value;
                    return;
                }

                if (InterviewsCountPerPersonGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewsCountPerPerson = value;
                }

            }
        }

    }
}