using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Confirmit.Security.Crypto.Web;

namespace Confirmit.CATI.Supervisor.Classes.Auth
{
    public class QueryStringDecryptor
    {
        public QueryString Decrypt(NameValueCollection queryString)
        {
            int companyId = 0;
            var queryStringResult = new QueryString();

            if (!string.IsNullOrEmpty(queryString["sid"]))
            {
                // Note: we do not know company ID and do not have connection string to database untill HttpContext.Current.User is not initialized.
                var decryptedParams = EncryptionUsingMachineKey.Decrypt(DataProtection.All, queryString["sid"]);
                var stringExpicitCompanyId = Regex.Match(decryptedParams, @"(?<=companyid=).*?(?=&|$)", RegexOptions.IgnoreCase).Value;
                int.TryParse(stringExpicitCompanyId, out companyId);

                queryStringResult.ProjectId = Regex.Match(decryptedParams, @"(?<=projectid=).*?(?=&|$)", RegexOptions.IgnoreCase).Value;
            }

            if (!string.IsNullOrEmpty(queryString["companyid"]))
            {
                int.TryParse(queryString["companyid"], out companyId);
            }

            queryStringResult.CompanyId = companyId;

            return queryStringResult;

        }
    }
}