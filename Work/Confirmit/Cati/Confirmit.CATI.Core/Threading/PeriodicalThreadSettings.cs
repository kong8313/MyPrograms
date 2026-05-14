using System.Linq;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Threading
{
    public class PeriodicalThreadSettings
    {
        private ICompanyInfo _companyInfo;
        public PeriodicalThreadSettings(ICompanyInfo companyInfo)
        {
            _companyInfo = companyInfo;
        }
        
        private bool SuspendStartingAsyncOperations => Configuration.ConfirmitConfiguration.GetBoolValue("SuspendStartingAsyncOperations", false);
        private string SuspendStartingAsyncOperationsAllowCompanies => Configuration.ConfirmitConfiguration.GetStringValue("SuspendStartingAsyncOperationsAllowCompanies", "");

        public bool IsCurrentCompanySuspended()
        {
            if (!SuspendStartingAsyncOperations)
                return false;

            var companies = SuspendStartingAsyncOperationsAllowCompanies.Split(',').Where(x => int.TryParse(x, out _)).Select(int.Parse);

            return !companies.Contains(_companyInfo.CompanyId);
        }
    }
}