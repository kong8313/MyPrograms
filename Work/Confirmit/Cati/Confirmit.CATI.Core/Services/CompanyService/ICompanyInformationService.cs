using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CompanyService
{
    public interface ICompanyInformationService
    {
        string GetCompanyNameFromCompanyId(int companyId);

        string GetCompanyAliasFromCompanyId(int companyId);

        int GetCompanyIdFromAlias(string companyAlias);

        int GetMaxIvrAgentsForCurrentCompany();

        bool HasCompanyCallCentersAddon(int companyId);
        
        void SetCatiSqlServerId(int companyId, int? sqlServerId);
        
        List<int> GetChildCompanyIds(int parentCompanyId);
    }
}
