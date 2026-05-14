using System;
using Confirmit.CATI.Core.SystemSettings.Console;

namespace Confirmit.CATI.Core.SystemSettings.Console.Fakes
{
    public class StubIBBCCSettings : IBBCCSettings 
    {
        private IBBCCSettings _inner;

        public StubIBBCCSettings()
        {
            _inner = null;
        }

        public IBBCCSettings Inner
        {
            set {_inner = value;} get {return _inner;}
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