using System.Data.SqlClient;

namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IConfirmitCATIValidator : ICommonValidator
    {
        string GetTypeOfActionWithDatabase(string sqlServerName, string databaseName, string userLogin, string userPassword);

        void ValidateDatabaseSettings(string sqlServerName, string databaseName, string userLogin, string userPassword, string typeOfActionWithDatabase);

        void VerifyDtc(SqlConnectionStringBuilder catiConnectionStringBuilder, string confirmitLinkedServer, string databaseNameOnConfirmitSide);

        void ValidateDataAndLogPathParameters(string catiDatabasesDataFilePath, string catiDatabasesLogsFilePath);

        void ValidateHasSQLLoginAdministratorPermissions(string sqlServerName, string databaseLogin, string databasePassword);

        void ValidateConfirmitLinkedServer(string catiSqlServerName,  string userName, string password, string confirmitLinkedServerName);

        void ValidateIsAlivePageUrl(string isAlivePageUrl, IIsAliveHtmEngine isAliveHtmEngine);
    }
}
