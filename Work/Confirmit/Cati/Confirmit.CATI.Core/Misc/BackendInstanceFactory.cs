using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Services.CompanyService;
using Confirmit.Databases;

namespace Confirmit.CATI.Core.Misc
{
    public enum HostType
    {
        BackendDefaultInstance,
        BackendNamedInstance,
        Supervisor
    }

    public class BackendInstanceFactory : IBackendInstanceFactory
    {
        private readonly IConnectionStrings _connectionStrings;
        private readonly ICompanyInformationService _companyInformationService;
        
        public BackendInstanceFactory()
            : this(ServiceLocator.Resolve<IConnectionStrings>(), ServiceLocator.Resolve<ICompanyInformationService>())
        {

        }

        public BackendInstanceFactory(IConnectionStrings connectionStrings, ICompanyInformationService companyInformationService)
        {
            _connectionStrings = connectionStrings;
            _companyInformationService = companyInformationService;
        }

        public BackendInstance Create(
            int companyId,
            HostType hostType)
        {
            var backendInstance = new BackendInstance();

            // Settings below common for all "hosts"
            backendInstance.MasterConnectionString = _connectionStrings.MasterConnectionString;
            backendInstance.DefaultInstanceConnectionString = _connectionStrings.DefaultInstanceConnectionString;
            backendInstance.ConfirmConnectionString = _connectionStrings.ConfirmConnectionString;
            backendInstance.ConfirmlogConnectionString = _connectionStrings.ConfirmlogConnectionString;

            switch (hostType)
            {
                case HostType.BackendDefaultInstance:
                    backendInstance.CompanyId = 0;
                    backendInstance.CompanyName = "Default instance";
                    backendInstance.CompanyAlias = "Default instance alias";
                    backendInstance.ConnectionString = _connectionStrings.DefaultInstanceConnectionString;
                    backendInstance.IsDefaultInstance = true;
                    backendInstance.IsExecutedInBackendInstance = true;
                    backendInstance.IsCacheEnabled = false;
                    break;
                case HostType.BackendNamedInstance:
                    backendInstance.CompanyId = companyId;
                    backendInstance.CompanyName = _companyInformationService.GetCompanyNameFromCompanyId(companyId);
                    backendInstance.CompanyAlias = _companyInformationService.GetCompanyAliasFromCompanyId(companyId);
                    backendInstance.ConnectionString = _connectionStrings.GetConnectionStringForSpecificCompany(companyId);
                    backendInstance.IsDefaultInstance = false;
                    backendInstance.IsExecutedInBackendInstance = true;
                    backendInstance.IsCacheEnabled = true;
                    backendInstance.HasCallCentersAddon = _companyInformationService.HasCompanyCallCentersAddon(companyId);
                    break;
                case HostType.Supervisor:
                    if (companyId > 0 && SupervisorPrincipal.Current.IsCatiParentAdministrator)
                    {
                        companyId = DbLib.GetParentCatiCompanyId(companyId);
                    }

                    backendInstance.CompanyId = companyId;
                    backendInstance.CompanyName = companyId == 0 ? "Unknown instance" : _companyInformationService.GetCompanyNameFromCompanyId(companyId);
                    backendInstance.CompanyAlias = companyId == 0 ? "Unknown instance alias" : _companyInformationService.GetCompanyAliasFromCompanyId(companyId);
                    backendInstance.ConnectionString = companyId == 0 ? string.Empty : _connectionStrings.GetConnectionStringForSpecificCompany(companyId);
                    backendInstance.IsDefaultInstance = false;
                    backendInstance.IsExecutedInBackendInstance = false;
                    backendInstance.IsCacheEnabled = false;
                    backendInstance.HasCallCentersAddon = companyId != 0 && _companyInformationService.HasCompanyCallCentersAddon(companyId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("hostType");
            }

            return backendInstance;
        }
    }
}
