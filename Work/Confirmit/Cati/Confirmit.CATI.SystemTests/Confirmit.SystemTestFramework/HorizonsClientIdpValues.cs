using System.Data;
using System.Data.SqlClient;
using Confirmit.Configuration;
using Confirmit.DataServices.RDataAccess;
using Confirmit.Security.Crypto;

namespace Confirmit.SystemTestFramework
{
    public class HorizonsClientIdpValues
    {
        public static string ClientId
        {
            get
            {
                return ConfirmitConfiguration.GetStringValue("IdpInternalResourceOwnerClient", null);

            }
        }

        public static string ClientSecret
        {
            get
            {
                return ConfirmitConfiguration.GetStringValue("IdpInternalResourceOwnerClientSecret", null);

            }
        }

        public const string ClientScopes = "openid profile users offline_access";
    }
}
