using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Backend.WebApiServices.Authorization
{
    public class Authorizer : IAuthorizer
    {
        private readonly ISupervisorInfoProvider _supervisorInfoProvider;
        private readonly ICompanyInfo _companyInfo;

        public Authorizer(ISupervisorInfoProvider supervisorInfoProvider, ICompanyInfo companyInfo)
        {
            _supervisorInfoProvider = supervisorInfoProvider;
            _companyInfo = companyInfo;
        }

        public void Authorize()
        {
            var supervisorInfo = _supervisorInfoProvider.GetInfo();
            var roles = supervisorInfo.Roles;

            if (roles == null)
            {
                throw new AuthenticateException("The client key is invalid");
            }

            if (!roles.SystemApiAccess || !roles.SystemCatiAdministrate)
            {
                throw new AuthenticateException("You dont have required permissions");
            }

            var canAccessAnotherCompanies =
                roles.SystemProjectAdministrate ||
                roles.SystemAdministrate;

            if (_companyInfo.CompanyId != supervisorInfo.CompanyId && !canAccessAnotherCompanies)
            {
                throw new AuthenticateException("Not enough permissions to access API for this company");
            }
        }
    }
}