using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubICacheCallsSettingsGroup : ICacheCallsSettingsGroup 
    {
        private ICacheCallsSettingsGroup _inner;

        public StubICacheCallsSettingsGroup()
        {
            _inner = null;
        }

        public ICacheCallsSettingsGroup Inner
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