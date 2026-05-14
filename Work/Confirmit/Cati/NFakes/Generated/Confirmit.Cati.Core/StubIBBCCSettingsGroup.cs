using System;
using Confirmit.CATI.Core.SystemSettings.Console;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Console.Fakes
{
    public class StubIBBCCSettingsGroup : IBBCCSettingsGroup 
    {
        private IBBCCSettingsGroup _inner;

        public StubIBBCCSettingsGroup()
        {
            _inner = null;
        }

        public IBBCCSettingsGroup Inner
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

        private bool _OptimisticConcurrency;
        public Func<bool> OptimisticConcurrencyGet;
        public Action<bool> OptimisticConcurrencySetBoolean;

        bool IBBCCSettings.OptimisticConcurrency
        {
            get
            {
                if (OptimisticConcurrencyGet != null)
                {
                    return OptimisticConcurrencyGet();
                } else if (_inner != null)
                {
                    return ((IBBCCSettings)_inner).OptimisticConcurrency;
                }

                if (OptimisticConcurrencySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OptimisticConcurrency;
                }

                return default(bool);
            }

            set
            {
                if (OptimisticConcurrencySetBoolean != null)
                {
                    OptimisticConcurrencySetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IBBCCSettings)_inner).OptimisticConcurrency = value;
                    return;
                }

                if (OptimisticConcurrencyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _OptimisticConcurrency = value;
                }

            }
        }

    }
}