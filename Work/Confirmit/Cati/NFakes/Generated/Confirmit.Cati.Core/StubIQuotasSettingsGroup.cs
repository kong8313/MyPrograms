using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIQuotasSettingsGroup : IQuotasSettingsGroup 
    {
        private IQuotasSettingsGroup _inner;

        public StubIQuotasSettingsGroup()
        {
            _inner = null;
        }

        public IQuotasSettingsGroup Inner
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