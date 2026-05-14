using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Logger;

namespace Confirmit.CATI.Core.Misc
{
    public class CompanyInfo : ICompanyInfo
    {
        public int CompanyId => BackendInstance.Current.CompanyId;

        public string CompanyName => BackendInstance.Current.CompanyName;

        public string CompanyAlias => BackendInstance.Current.CompanyAlias;
        
        public int GetCompanyId(int id, string source)
        {
            int companyId;

            if (Enum.IsDefined(typeof(ClientErrorSource), source))
            {
                // If error came from the client (dialer or old load test utility)
                companyId = id;
            }
            // When WCF logs error it sets id, so, we have to somehow understand is it real company id or not.
            // We use for the source parameter.
            else if (id != 0 && Enum.IsDefined(typeof(BackendEventSource), source))
            {
                companyId = id;
            }
            else
            {
                companyId = BackendInstance.IsInitialized ? BackendInstance.Current.CompanyId : 0;
            }
            
            return companyId;
        }
    }
}