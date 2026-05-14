using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.WcfServices.Clients;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.PersonLogin
{
    public class LicenseService : ILicenseService
    {
        private readonly IAuthoringService _authoringService;
        private readonly ICompanyInfo _companyInfo;

        public LicenseService(
            IAuthoringService authoringService,
            ICompanyInfo companyInfo)
        {
            _authoringService = authoringService;
            _companyInfo = companyInfo;
        }

        /// <summary>
        /// Checks License limit of simultaneously logged in interviewers
        /// </summary>
        /// <param name="agentType"></param>
        public void CheckLicense(AgentType agentType)
        {
            if (agentType == AgentType.IvrAgent)
            {
                return;
            }

            var maximumCatiInterviewers = _authoringService.GetMaximumCatiInterviewers(_companyInfo.CompanyId);

            EventDetailsScope.Current.AddTiming("CheckLicense.GetMaximumCatiInterviewers");

            var loggedInInterviewersCount = BvSpGetLoggedInPersonsCountAdapter.ExecuteScalar<int>();

            EventDetailsScope.Current.AddTiming("CheckLicense.BvSpGetLoggedInPersonsCountAdapter");

            if (loggedInInterviewersCount >= maximumCatiInterviewers)
            {
                throw new UserMessageException("License limit of logged in CATI interviewers is exceeded.", "Error_LicenseLimitExceeded");
            }
        }
    }
}