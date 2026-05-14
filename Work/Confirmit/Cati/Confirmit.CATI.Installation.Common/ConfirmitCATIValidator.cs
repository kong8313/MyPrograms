using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.Installation.Common.Properties;

namespace Confirmit.CATI.Installation.Common
{
    public class ConfirmitCATIValidator : CommonValidator, IConfirmitCATIValidator
    {
        /// <summary>
        /// Get type of action with databse. Return "UseExistingDB" if defauolt CATI database already exists, "CreateNewDB" otherwise
        /// </summary>
        /// <param name="sqlServerName">Sql server name</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="userLogin">Sql user login</param>
        /// <param name="userPassword">Sql user password</param>
        /// <returns></returns>
        public string GetTypeOfActionWithDatabase(string sqlServerName, string databaseName, string userLogin, string userPassword)
        {
            ValidateSqlServerConnection(sqlServerName, userLogin, userPassword);

            bool isDatabaseExist;
            try
            {
                ValidateDatabaseConnection(sqlServerName, databaseName, userLogin, userPassword);
                isDatabaseExist = true;
            }
            catch
            {
                isDatabaseExist = false;
            }

            if (isDatabaseExist)
            {
                return "UseExistingDB";
            }

            return "CreateNewDB";
        }

        public void ValidateDatabaseSettings(string sqlServerName, string databaseName, string userLogin, string userPassword, string typeOfActionWithDatabase)
        {
            ValidateSqlServerConnection(sqlServerName, userLogin, userPassword);

            if (typeOfActionWithDatabase == "UseExistingDB")
            {
                ValidateDatabaseConnection(sqlServerName, databaseName, userLogin, userPassword);
            }
        }

        public void VerifyDtc(SqlConnectionStringBuilder catiConnectionStringBuilder, string confirmitLinkedServer, string databaseNameOnConfirmitSide)
        {
            string testQuery = string.Format(@"
                    DECLARE @TestSubQuery NVARCHAR(MAX) = '.sys.sp_executesql N''select 1'''

                    DECLARE @TestQueryForMasterDatabase NVARCHAR(MAX) = '[master]' + @TestSubQuery
                    DECLARE @TestQueryForConfirmlogDatabase NVARCHAR(MAX) = '[{0}].[{1}]' + @TestSubQuery

                BEGIN TRAN
                    EXEC (@TestQueryForMasterDatabase)
                    EXEC (@TestQueryForConfirmlogDatabase)
                COMMIT",
                confirmitLinkedServer,
                databaseNameOnConfirmitSide);

            var catiDatabaseEngine = new DatabaseEngine(catiConnectionStringBuilder.DataSource, catiConnectionStringBuilder.UserID, catiConnectionStringBuilder.Password);

            try
            {
                catiDatabaseEngine.ExecuteNonQuery("tempdb", testQuery);
            }
            catch (SqlException ex)
            {
                throw new ValidateException(string.Format(Resources.DtcIsNotEnabledFormat, ex.Message), ex);
            }
            catch (Exception ex)
            {
                throw new ValidateException(string.Format(Resources.DtcUnexpectedErrorFormat, ex.Message), ex);
            }
        }

        public void ValidateDataAndLogPathParameters(string catiDatabasesDataFilePath, string catiDatabasesLogsFilePath)
        {
            if (string.IsNullOrEmpty(catiDatabasesDataFilePath) ^ string.IsNullOrEmpty(catiDatabasesLogsFilePath))
            {
                throw new ValidateException(Resources.PathToDataAndLogFileMessage);
            }
        }

        /// <summary>
        /// Valudate, that SQL login has administration permissions (is a member of sysadmin group)
        /// </summary>
        /// <param name="sqlServerName">SQL server name</param>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        public void ValidateHasSQLLoginAdministratorPermissions(string sqlServerName, string userName, string password)
        {
            try
            {
                var dbEngine = new DatabaseEngine(sqlServerName, userName, password);
                dbEngine.ValidateConnection();

                if (dbEngine.ExecuteScalar<int>("select IS_SRVROLEMEMBER ('sysadmin')") == 0)
                {
                    throw new ValidateException(string.Format(Resources.UserMustBeSysAdminFormat, userName));
                }
            }
            catch (ValidateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ValidateException(string.Format(Resources.WrongSqlCredentialsFormatMessage, sqlServerName, userName), ex);
            }
        }

        /// <summary>
        /// Validate, thst linked server to connect to 'confirmlog' database exists and works fine
        /// </summary>
        /// <param name="sqlServerName">SQL server name with default CATI database</param>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <param name="confirmitLinkedServerName">Linked server name</param>
        public void ValidateConfirmitLinkedServer(string sqlServerName, string userName, string password, string confirmitLinkedServerName)
        {
            try
            {
                var dbEngine = new DatabaseEngine(sqlServerName, userName, password);
                dbEngine.ValidateConnection();

                if (dbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM master.dbo.sysservers WHERE srvname='{0}'", confirmitLinkedServerName)) == 0)
                {
                    throw new ValidateException(Resources.LinkedServerDoesNotExist);
                }

                 if (dbEngine.ExecuteScalar<int>(string.Format("SELECT COUNT(*) FROM [{0}].master.dbo.sysdatabases WHERE name='confirm' or name='confirmlog'", confirmitLinkedServerName)) != 2)
                 {
                     throw new ValidateException(string.Format(Resources.LinkedServerDoesNotContainConfirmitDatabasesFormat, confirmitLinkedServerName));
                 }
            }
            catch (ValidateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ValidateException(string.Format(Resources.LinkedServerDoesNotWorkFormat, ex.Message));
            }
        }

        public void ValidateIsAlivePageUrl(string isAlivePageUrl, IIsAliveHtmEngine isAliveHtmEngine)
        {
            try
            {
                if (string.IsNullOrEmpty(isAlivePageUrl))
                {
                    throw new ValidateException(Resources.NotDefinedIsAlivePageUrl);
                }

                isAliveHtmEngine.VerifyAccesToPageByUrl("http://localhost/" + isAlivePageUrl);
            }
            catch (ValidateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ValidateException(string.Format("Unexpected error: {0}", ex.Message));
            }
        }
    }
}
