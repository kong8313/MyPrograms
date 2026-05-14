using System;
using Confirmit.CATI.Supervisor.Core.Metadata;

namespace Confirmit.CATI.Supervisor.Core.Metadata.Fakes
{
    public class StubIMetadataApiService : IMetadataApiService 
    {
        private IMetadataApiService _inner;

        public StubIMetadataApiService()
        {
            _inner = null;
        }

        public IMetadataApiService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CompanyModel GetCompanyInfoDelegate();
        public GetCompanyInfoDelegate GetCompanyInfo;

        CompanyModel IMetadataApiService.GetCompanyInfo()
        {


            if (GetCompanyInfo != null)
            {
                return GetCompanyInfo();
            } else if (_inner != null)
            {
                return ((IMetadataApiService)_inner).GetCompanyInfo();
            }

            return default(CompanyModel);
        }

    }
}