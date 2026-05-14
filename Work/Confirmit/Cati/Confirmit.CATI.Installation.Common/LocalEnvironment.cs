using System;

namespace Confirmit.CATI.Installation.Common
{
    public class LocalEnvironment
    {
        public const string CatiBuildDatabaseName = "ConfirmitCATIV15_BUILD";

        public static string GetLocalSqlInstanceName()
        {
            string sqlInstanceName = Environment.MachineName;
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CATI_SQL_INSTANCE_NAME")))
            {
                sqlInstanceName = Environment.GetEnvironmentVariable("CATI_SQL_INSTANCE_NAME");
            }

            return sqlInstanceName;
        }
    }
}