using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIRecordedInterviewsSettings : IRecordedInterviewsSettings 
    {
        private IRecordedInterviewsSettings _inner;

        public StubIRecordedInterviewsSettings()
        {
            _inner = null;
        }

        public IRecordedInterviewsSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _MaxSaved;
        public Func<int> MaxSavedGet;
        public Action<int> MaxSavedSetInt32;

        int IRecordedInterviewsSettings.MaxSaved
        {
            get
            {
                if (MaxSavedGet != null)
                {
                    return MaxSavedGet();
                } else if (_inner != null)
                {
                    return ((IRecordedInterviewsSettings)_inner).MaxSaved;
                }

                if (MaxSavedSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxSaved;
                }

                return default(int);
            }

            set
            {
                if (MaxSavedSetInt32 != null)
                {
                    MaxSavedSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IRecordedInterviewsSettings)_inner).MaxSaved = value;
                    return;
                }

                if (MaxSavedGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxSaved = value;
                }

            }
        }

    }
}