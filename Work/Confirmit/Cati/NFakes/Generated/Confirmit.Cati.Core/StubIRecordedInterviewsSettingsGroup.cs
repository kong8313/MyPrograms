using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIRecordedInterviewsSettingsGroup : IRecordedInterviewsSettingsGroup 
    {
        private IRecordedInterviewsSettingsGroup _inner;

        public StubIRecordedInterviewsSettingsGroup()
        {
            _inner = null;
        }

        public IRecordedInterviewsSettingsGroup Inner
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