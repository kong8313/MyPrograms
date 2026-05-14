using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Cookies
{
    public class ConfirmitNetCookieNameProvider
    {
        private const string ConfirmitNetCookieName = "confirmitnet";
        private const string ConfirmitNetIdpCookieName = "confirmitidp";

        public static string GetName()
        {
            return GetCookieName(ConfirmitNetCookieName);
        }

        public static string GetIdpName()
        {
            return GetCookieName(ConfirmitNetIdpCookieName);
        }

        private static string GetCookieName(string cookie)
        {
            var suffix = GetConfirmitNetCookieSuffix();

            if (!string.IsNullOrEmpty(suffix))
            {
                suffix = "_" + suffix;
            }

            return cookie + suffix;
        }

        private static string GetConfirmitNetCookieSuffix()
        {
            var result = String.Empty;

            try
            {
                using (var connection = new SqlConnection(BackendInstance.Current.ConfirmConnectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"SELECT ISNULL(ConfigValue, '') FROM [dbo].[CfgConfig] where ConfigName = 'ConfirmitCookieSuffix'";
                    command.CommandType = CommandType.Text;
                    result = (string)command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error during getting confirmit cookie suffix:" + Environment.NewLine + ex);
            }

            return result;
        }
    }
}