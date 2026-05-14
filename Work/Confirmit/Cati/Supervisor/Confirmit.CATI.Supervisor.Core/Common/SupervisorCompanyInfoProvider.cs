using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Supervisor.Core.Common
{
    class SupervisorCompanyInfoProvider : ICompanyInfoProvider
    {
        public bool HasCallCentersAddon
        {
            get { return BackendInstance.Current.HasCallCentersAddon; }
        }
    }
}
