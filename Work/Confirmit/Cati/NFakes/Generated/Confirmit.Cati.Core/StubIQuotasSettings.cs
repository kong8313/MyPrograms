using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIQuotasSettings : IQuotasSettings 
    {
        private IQuotasSettings _inner;

        public StubIQuotasSettings()
        {
            _inner = null;
        }

        public IQuotasSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _MaxQuestionsPerQuota;
        public Func<int> MaxQuestionsPerQuotaGet;
        public Action<int> MaxQuestionsPerQuotaSetInt32;

        int IQuotasSettings.MaxQuestionsPerQuota
        {
            get
            {
                if (MaxQuestionsPerQuotaGet != null)
                {
                    return MaxQuestionsPerQuotaGet();
                } else if (_inner != null)
                {
                    return ((IQuotasSettings)_inner).MaxQuestionsPerQuota;
                }

                if (MaxQuestionsPerQuotaSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MaxQuestionsPerQuota;
                }

                return default(int);
            }

            set
            {
                if (MaxQuestionsPerQuotaSetInt32 != null)
                {
                    MaxQuestionsPerQuotaSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IQuotasSettings)_inner).MaxQuestionsPerQuota = value;
                    return;
                }

                if (MaxQuestionsPerQuotaGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MaxQuestionsPerQuota = value;
                }

            }
        }

    }
}