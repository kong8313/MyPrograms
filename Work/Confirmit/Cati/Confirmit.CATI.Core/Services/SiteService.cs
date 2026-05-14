using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services
{
    public static class SiteService
    {
        public static DiallerType GetDiallerType()
        {
            return ServiceLocator.Resolve<IDialerSettings>().Dialer;
        }

        public static int GetNewSid()
        {
            int newSid;

            //
            // we should not run BvSpGetNewSID using adapter because it should be executed
            // under its own connection to avoid deadlocks
            using (var connection = new SqlConnection(BackendInstance.Current.ConnectionString))
            {
                connection.Open();

                var command = new SqlCommand("BvSpGetNewSID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var returnValue = new SqlParameter("@ReturnValue", 0)
                {
                    Direction = ParameterDirection.ReturnValue
                };

                command.Parameters.Add(returnValue);

                command.ExecuteNonQuery();

                newSid = (int)command.Parameters["@ReturnValue"].Value;
            }

            return newSid;
        }
    }
}