using Confirmit.CATI.Backend.WebApiServices.Authorization;
using Confirmit.CATI.Backend.WebApiServices.Fakes;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Misc.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Backend.UnitTests.WebApiServices
{
    [TestClass]
    public class AuthorizerTest
    {
        private StubICompanyInfo CompanyInfo = new StubICompanyInfo {CompanyIdGet = () => 123};

        private Authorizer GetAuthorizer(int companyId, CatiSupervisorRoles roles)
        {
            var catiSupervisorInfo = new CatiSupervisorInfo
            {
                CompanyId = companyId,
                Roles = roles
            };

            var supervisorInfoProvider = new StubISupervisorInfoProvider
            {
                GetInfo = () => catiSupervisorInfo
            };

            var authorizer = new Authorizer(supervisorInfoProvider, CompanyInfo);
            return authorizer;
        }

        [ExpectedException(typeof(AuthenticateException))]
        [TestMethod]
        public void SameCompany_NoSystemApiAccess_NoAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = true,
                CompanyAdministrate = true,
                SystemApiAccess = false,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(123, roles);

            authorizer.Authorize();
        }

        [ExpectedException(typeof(AuthenticateException))]
        [TestMethod]
        public void AnotherCompany_NoSystemApiAccess_NoAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = true,
                CompanyAdministrate = true,
                SystemApiAccess = false,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(456, roles);

            authorizer.Authorize();
        }

        [ExpectedException(typeof(AuthenticateException))]
        [TestMethod]
        public void SameCompany_NoSystemCatiAdministrate_NoAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = true,
                CompanyAdministrate = true,
                SystemApiAccess = true,
                SystemCatiAdministrate = false
            };

            var authorizer = GetAuthorizer(123, roles);

            authorizer.Authorize();
        }

        [ExpectedException(typeof(AuthenticateException))]
        [TestMethod]
        public void AnotherCompany_NoSystemCatiAdministrate_NoAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = true,
                CompanyAdministrate = true,
                SystemApiAccess = true,
                SystemCatiAdministrate = false
            };

            var authorizer = GetAuthorizer(456, roles);

            authorizer.Authorize();
        }

        [TestMethod]
        public void SameCompany_NoSystemProjectAdministrateNoCompanyAdministrate_HasAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = false,
                CompanyAdministrate = false,
                SystemApiAccess = true,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(123, roles);

            authorizer.Authorize();
        }

        [ExpectedException(typeof(AuthenticateException))]
        [TestMethod]
        public void AnotherCompany_NoSystemProjectAdministrateNoCompanyAdministrate_NoAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = false,
                CompanyAdministrate = false,
                SystemApiAccess = true,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(456, roles);

            authorizer.Authorize();
        }

        [TestMethod]
        public void SameCompany_SystemProjectAdministrate_HasAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = true,
                CompanyAdministrate = false,
                SystemApiAccess = true,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(123, roles);

            authorizer.Authorize();
        }

        [TestMethod]
        public void AnotherCompany_SystemProjectAdministrate_HasAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = true,
                CompanyAdministrate = false,
                SystemApiAccess = true,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(456, roles);

            authorizer.Authorize();
        }

        [TestMethod]
        public void SameCompany_CompanyAdministrate_HasAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = false,
                CompanyAdministrate = true,
                SystemApiAccess = true,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(123, roles);

            authorizer.Authorize();
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticateException))]
        public void AnotherCompany_CompanyAdministrate_NoAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemProjectAdministrate = false,
                CompanyAdministrate = true,
                SystemApiAccess = true,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(456, roles);

            authorizer.Authorize();
        }

        [TestMethod]
        public void SameCompany_SystemAdministrate_HasAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemAdministrate = true,
                SystemProjectAdministrate = false,
                CompanyAdministrate = false,
                SystemApiAccess = true,
                SystemCatiAdministrate = true
            };

            var authorizer = GetAuthorizer(456, roles);

            authorizer.Authorize();
        }

        [TestMethod]
        public void AnotherCompany_SystemAdministrate_HasAccess()
        {
            var roles = new CatiSupervisorRoles
            {
                SystemAdministrate = true,
                SystemProjectAdministrate = false,
                CompanyAdministrate = false,
                SystemApiAccess = true,
                SystemCatiAdministrate = true,
            };

            var authorizer = GetAuthorizer(456, roles);

            authorizer.Authorize();
        }
    }
}