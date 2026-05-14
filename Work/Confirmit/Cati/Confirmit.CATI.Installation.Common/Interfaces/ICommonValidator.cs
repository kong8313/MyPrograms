namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface ICommonValidator
    {
        void ValidateSqlServerConnection(string sqlServerName, string userLogin, string userPassword);

        void ValidateDatabaseConnection(string sqlServerName, string databaseName, string userLogin, string userPassword);

        void ValidateParametersFilling(string[] parameters);

        void ValidateOneParameterFilling(string parameterValue, string parameterName);

        int ValidateIntParameter(string valueStr, string nameOfParameter);

        int ValidateNotNegativeIntParameter(string valueStr, string nameOfParameter);

        void ValidateEmailAddresses(string emails);

        void ValidatePossibleFileName(string name);
    }
}
