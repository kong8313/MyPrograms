using System;
using System.Data.SqlClient;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Setup.UnitTests.FakeClasses
{
    public class FakeConfirmitCATIValidator : IConfirmitCATIValidator
    {
        private readonly IConfirmitCATIValidator _realConfirmitCatiValidator;

        public string TypeOfActionWithDatabase { get; set; }
        public bool IsConfirmitLinkedServerOk { get; set; }
        public bool IsDataAndLogPathParametersOk { get; set; }
        public bool IsDatabaseSettingsOk { get; set; }
        public bool IsHasSQLLoginAdministratorPermissionsOk { get; set; }
        public bool IsIsAlivePageUrlOk { get; set; }
        public bool IsLoggingParametersOk { get; set; }
        public bool IsSQLServerVersionOk { get; set; }
        public bool IsAbilityOfEventSourceCreationOk { get; set; }
        public bool IsDatabaseConnectionOk { get; set; }
        public bool IsEmailAddressesOk { get; set; }
        public bool IsFileNameOk { get; set; }
        public bool IsIpAddressesOk { get; set; }
        public bool IsOneParameterFillingOk { get; set; }
        public bool IsParametersFillingOk { get; set; }
        public bool IsSqlServerConnectionOk { get; set; }
        public bool IsIntParameterOk { get; set; }
        public bool IsNotNegativeIntParameterOk { get; set; }
        public bool IsDtcEnabled { get; set; }

        public bool UseRealEmailAddressValidation { get; set; }
        public bool UseRealFileNamePossibilityValidation { get; set; }

        public FakeConfirmitCATIValidator()
        {
            TypeOfActionWithDatabase = "UseExictingDB"; // CreateNewDB
            IsConfirmitLinkedServerOk = IsDataAndLogPathParametersOk = IsDatabaseSettingsOk = IsHasSQLLoginAdministratorPermissionsOk = IsIsAlivePageUrlOk = IsFileNameOk =
            IsLoggingParametersOk = IsSQLServerVersionOk = IsAbilityOfEventSourceCreationOk = IsDatabaseConnectionOk = IsEmailAddressesOk = 
            IsIpAddressesOk = IsOneParameterFillingOk = IsParametersFillingOk = IsSqlServerConnectionOk = IsIntParameterOk = IsNotNegativeIntParameterOk = IsDtcEnabled = true;

            UseRealEmailAddressValidation = UseRealFileNamePossibilityValidation = false;

            _realConfirmitCatiValidator = new ConfirmitCATIValidator();
        }

        public string GetTypeOfActionWithDatabase(string sqlServerName, string databaseName, string userLogin, string userPassword)
        {
            return TypeOfActionWithDatabase;
        }

        public void ValidateConfirmitLinkedServer(string catiSqlServerName, string userName, string password, string confirmitLinkedServerName)
        {
            if (!IsConfirmitLinkedServerOk)
            {
                throw new ValidateException("Wrong ConfirmitLinkedServer");
            }
        }

        public void ValidateIsAlivePageUrl(string isAlivePageUrl, IIsAliveHtmEngine isAliveHtmEngine)
        {
            if (!IsIsAlivePageUrlOk)
            {
                throw new ValidateException("Wrong IsAlivePageUrl");
            }
        }

        public void VerifyDtc(SqlConnectionStringBuilder catiConnectionStringBuilder, string confirmitLinkedServer, string databaseNameOnConfirmitSide)
        {
            if (!IsDtcEnabled)
            {
                throw new ValidateException("DTC is not enabled");
            }
        }

        public void ValidateDataAndLogPathParameters(string catiDatabasesDataFilePath, string catiDatabasesLogsFilePath)
        {
            if (!IsDataAndLogPathParametersOk)
            {
                throw new ValidateException("Wrong DataAndLogPathParameters");
            }
        }

        public void ValidateDatabaseSettings(string sqlServerName, string databaseName, string userLogin, string userPassword, string typeOfActionWithDatabase)
        {
            if (!IsDatabaseSettingsOk)
            {
                throw new ValidateException("Wrong DatabaseSettings");
            }
        }

        public void ValidateHasSQLLoginAdministratorPermissions(string sqlServerName, string databaseLogin, string databasePassword)
        {
            if (!IsHasSQLLoginAdministratorPermissionsOk)
            {
                throw new ValidateException("Wrong HasSQLLoginAdministratorPermissions");
            }
        }

        public void ValidateSQLServerVersion(string sqlServerName, string databaseLogin, string databasePassword)
        {
            if (!IsSQLServerVersionOk)
            {
                throw new ValidateException("Wrong SQLServerVersion");
            }
        }
        
        public void ValidateDatabaseConnection(string sqlServerName, string databaseName, string userLogin, string userPassword)
        {
            if (!IsDatabaseConnectionOk)
            {
                throw new ValidateException("Wrong DatabaseConnection");
            }
        }

        public void ValidateEmailAddresses(string emails)
        {
            if (!IsEmailAddressesOk)
            {
                throw new ValidateException("Wrong EmailAddresses");
            }

            if (UseRealEmailAddressValidation)
            {
                _realConfirmitCatiValidator.ValidateEmailAddresses(emails);
            }
        }

        public void ValidatePossibleFileName(string name)
        {
            if (!IsFileNameOk)
            {
                throw new ValidateException("Wrong EmailAddresses");
            }

            if (UseRealFileNamePossibilityValidation)
            {
                _realConfirmitCatiValidator.ValidatePossibleFileName(name);
            }
        }

        public void ValidateOneParameterFilling(string parameterValue, string parameterName)
        {
            if (!IsOneParameterFillingOk)
            {
                throw new ValidateException("Wrong OneParameterFilling");
            }
        }

        public void ValidateParametersFilling(string[] parameters)
        {
            if (!IsParametersFillingOk)
            {
                throw new ValidateException("Wrong ParametersFilling");
            }
        }

        public void ValidateSqlServerConnection(string sqlServerName, string userLogin, string userPassword)
        {
            if (!IsSqlServerConnectionOk)
            {
                throw new ValidateException("Wrong SqlServerConnection");
            }
        }

        public int ValidateIntParameter(string valueStr, string nameOfParameter)
        {
            if (!IsIntParameterOk)
            {
                throw new ValidateException("Wrong IntParameter");
            }

            return Convert.ToInt32(valueStr);
        }

        public int ValidateNotNegativeIntParameter(string valueStr, string nameOfParameter)
        {
            if (!IsNotNegativeIntParameterOk)
            {
                throw new ValidateException("Wrong NotNegativeIntParameter");
            }

            return Convert.ToInt32(valueStr);
        }
    }
}