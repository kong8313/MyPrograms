using System;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Core.Common.Fakes
{
    public class StubICompanyInfoProvider : ICompanyInfoProvider 
    {
        private ICompanyInfoProvider _inner;

        public StubICompanyInfoProvider()
        {
            _inner = null;
        }

        public ICompanyInfoProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _HasCallCentersAddon;
        public Func<bool> HasCallCentersAddonGet;
        public Action<bool> HasCallCentersAddonSetBoolean;

        bool ICompanyInfoProvider.HasCallCentersAddon
        {
            get
            {
                if (HasCallCentersAddonGet != null)
                {
                    return HasCallCentersAddonGet();
                } else if (_inner != null)
                {
                    return ((ICompanyInfoProvider)_inner).HasCallCentersAddon;
                }

                if (HasCallCentersAddonSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _HasCallCentersAddon;
                }

                return default(bool);
            }

        }

    }
}