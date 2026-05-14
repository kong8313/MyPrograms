using System;
using Confirmit.CATI.Core.Services.CompanyService;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CompanyService.Fakes
{
    public class StubICompanyInformationService : ICompanyInformationService 
    {
        private ICompanyInformationService _inner;

        public StubICompanyInformationService()
        {
            _inner = null;
        }

        public ICompanyInformationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetCompanyNameFromCompanyIdInt32Delegate(int companyId);
        public GetCompanyNameFromCompanyIdInt32Delegate GetCompanyNameFromCompanyIdInt32;

        string ICompanyInformationService.GetCompanyNameFromCompanyId(int companyId)
        {


            if (GetCompanyNameFromCompanyIdInt32 != null)
            {
                return GetCompanyNameFromCompanyIdInt32(companyId);
            } else if (_inner != null)
            {
                return ((ICompanyInformationService)_inner).GetCompanyNameFromCompanyId(companyId);
            }

            return default(string);
        }

        public delegate string GetCompanyAliasFromCompanyIdInt32Delegate(int companyId);
        public GetCompanyAliasFromCompanyIdInt32Delegate GetCompanyAliasFromCompanyIdInt32;

        string ICompanyInformationService.GetCompanyAliasFromCompanyId(int companyId)
        {


            if (GetCompanyAliasFromCompanyIdInt32 != null)
            {
                return GetCompanyAliasFromCompanyIdInt32(companyId);
            } else if (_inner != null)
            {
                return ((ICompanyInformationService)_inner).GetCompanyAliasFromCompanyId(companyId);
            }

            return default(string);
        }

        public delegate int GetCompanyIdFromAliasStringDelegate(string companyAlias);
        public GetCompanyIdFromAliasStringDelegate GetCompanyIdFromAliasString;

        int ICompanyInformationService.GetCompanyIdFromAlias(string companyAlias)
        {


            if (GetCompanyIdFromAliasString != null)
            {
                return GetCompanyIdFromAliasString(companyAlias);
            } else if (_inner != null)
            {
                return ((ICompanyInformationService)_inner).GetCompanyIdFromAlias(companyAlias);
            }

            return default(int);
        }

        public delegate int GetMaxIvrAgentsForCurrentCompanyDelegate();
        public GetMaxIvrAgentsForCurrentCompanyDelegate GetMaxIvrAgentsForCurrentCompany;

        int ICompanyInformationService.GetMaxIvrAgentsForCurrentCompany()
        {


            if (GetMaxIvrAgentsForCurrentCompany != null)
            {
                return GetMaxIvrAgentsForCurrentCompany();
            } else if (_inner != null)
            {
                return ((ICompanyInformationService)_inner).GetMaxIvrAgentsForCurrentCompany();
            }

            return default(int);
        }

        public delegate bool HasCompanyCallCentersAddonInt32Delegate(int companyId);
        public HasCompanyCallCentersAddonInt32Delegate HasCompanyCallCentersAddonInt32;

        bool ICompanyInformationService.HasCompanyCallCentersAddon(int companyId)
        {


            if (HasCompanyCallCentersAddonInt32 != null)
            {
                return HasCompanyCallCentersAddonInt32(companyId);
            } else if (_inner != null)
            {
                return ((ICompanyInformationService)_inner).HasCompanyCallCentersAddon(companyId);
            }

            return default(bool);
        }

        public delegate void SetCatiSqlServerIdInt32NullableOfInt32Delegate(int companyId, int? sqlServerId);
        public SetCatiSqlServerIdInt32NullableOfInt32Delegate SetCatiSqlServerIdInt32NullableOfInt32;

        void ICompanyInformationService.SetCatiSqlServerId(int companyId, int? sqlServerId)
        {

            if (SetCatiSqlServerIdInt32NullableOfInt32 != null)
            {
                SetCatiSqlServerIdInt32NullableOfInt32(companyId, sqlServerId);
            } else if (_inner != null)
            {
                ((ICompanyInformationService)_inner).SetCatiSqlServerId(companyId, sqlServerId);
            }
        }

        public delegate List<int> GetChildCompanyIdsInt32Delegate(int parentCompanyId);
        public GetChildCompanyIdsInt32Delegate GetChildCompanyIdsInt32;

        List<int> ICompanyInformationService.GetChildCompanyIds(int parentCompanyId)
        {


            if (GetChildCompanyIdsInt32 != null)
            {
                return GetChildCompanyIdsInt32(parentCompanyId);
            } else if (_inner != null)
            {
                return ((ICompanyInformationService)_inner).GetChildCompanyIds(parentCompanyId);
            }

            return default(List<int>);
        }

    }
}