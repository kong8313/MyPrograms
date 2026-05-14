using System.Collections.Generic;
using System.Linq;
using System.Web;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Supervisor.Core.CallCenters
{
    public class CachedConfirmitSupervisorProvider : ICachedConfirmitSupervisorProvider
    {
        private readonly ICompanyInfo _companyInfo;
        private readonly IAuthoringService _authoringService;

        private const string SessionStorageKey = "ConfirmitCatiSupervisorsListSessionKeyName";

        public CachedConfirmitSupervisorProvider(
            ICompanyInfo companyInfo,
            IAuthoringService authoringService)
        {
            _companyInfo = companyInfo;
            _authoringService = authoringService;
        }

        public IEnumerable<CatiSupervisor> GetConfirmitCatiSupervisors()
        {
            var data = HttpContext.Current.Session[SessionStorageKey];
            IEnumerable<CatiSupervisor> result;
            if (data == null)
            {
                result = _authoringService.GetCompanyCatiSupervisorsNames(_companyInfo.CompanyId);
                HttpContext.Current.Session[SessionStorageKey] = result.ToArray();
            }
            else
            {
                result = (IEnumerable<CatiSupervisor>)data;
            }
            
            return result;
        }

        public void ClearCache()
        {
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Remove(SessionStorageKey);
            }
        }
    }
}
