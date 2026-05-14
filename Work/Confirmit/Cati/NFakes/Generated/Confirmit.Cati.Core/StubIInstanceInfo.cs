using System;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIInstanceInfo : IInstanceInfo 
    {
        private IInstanceInfo _inner;

        public StubIInstanceInfo()
        {
            _inner = null;
        }

        public IInstanceInfo Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _IsExecutedInBackendInstance;
        public Func<bool> IsExecutedInBackendInstanceGet;
        public Action<bool> IsExecutedInBackendInstanceSetBoolean;

        bool IInstanceInfo.IsExecutedInBackendInstance
        {
            get
            {
                if (IsExecutedInBackendInstanceGet != null)
                {
                    return IsExecutedInBackendInstanceGet();
                } else if (_inner != null)
                {
                    return ((IInstanceInfo)_inner).IsExecutedInBackendInstance;
                }

                if (IsExecutedInBackendInstanceSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsExecutedInBackendInstance;
                }

                return default(bool);
            }

        }

        private bool _IsDefaultInstance;
        public Func<bool> IsDefaultInstanceGet;
        public Action<bool> IsDefaultInstanceSetBoolean;

        bool IInstanceInfo.IsDefaultInstance
        {
            get
            {
                if (IsDefaultInstanceGet != null)
                {
                    return IsDefaultInstanceGet();
                } else if (_inner != null)
                {
                    return ((IInstanceInfo)_inner).IsDefaultInstance;
                }

                if (IsDefaultInstanceSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsDefaultInstance;
                }

                return default(bool);
            }

        }

    }
}