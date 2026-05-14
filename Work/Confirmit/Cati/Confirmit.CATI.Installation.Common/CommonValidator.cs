using System;
using System.Linq;
using System.Text.RegularExpressions;

using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.Installation.Common.Properties;

namespace Confirmit.CATI.Installation.Common
{
    public class CommonValidator : ICommonValidator
    {
        public void ValidateSqlServerConnection(string sqlServerName, string userLogin, string userPassword)
        {
            if (sqlServerName == "localhost" || sqlServerName == string.Empty)
            {
                throw new ValidateException(Resources.WrongSqlServerNameMessage);
            }

            ValidateDatabaseConnection(sqlServerName, null, userLogin, userPassword);
        }

        public void ValidateDatabaseConnection(string sqlServerName, string databaseName, string userLogin, string userPassword)
        {
            if (databaseName == string.Empty)
            {
                throw new ValidateException(Resources.WrongDatabaseNameMessage);
            }

            DatabaseEngine databaseEngine = new DatabaseEngine(sqlServerName, userLogin, userPassword);
            try
            {
                databaseEngine.ValidateConnection();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("The server was not found or was not accessible"))
                {
                    throw new ValidateException(string.Format(Resources.WrongConnectionToSqlServerFormatMessage, sqlServerName), ex);
                }

                throw new ValidateException(string.Format(Resources.WrongSqlCredentialsFormatMessage, sqlServerName, userLogin), ex);
            }

            try
            {
                databaseEngine.ValidateConnection(databaseName);
            }
            catch
            {
                throw new ValidateException(string.Format(Resources.WrongConnectionToDatabaseFormatMessage, databaseName, sqlServerName));
            }
        }

        public void ValidateParametersFilling(string[] parameters)
        {
            if (parameters.Any(string.IsNullOrWhiteSpace))
            {
                throw new ValidateException(Resources.AllParametersFillingMessage);
            }
        }

        public void ValidateOneParameterFilling(string parameterValue, string parameterName)
        {
            if (string.IsNullOrEmpty(parameterValue))
            {
                throw new ValidateException(string.Format(Resources.OneParameterFillingFormatMessage, parameterName));
            }
        }

        public int ValidateIntParameter(string valueStr, string nameOfParameter)
        {
            int value;
            if (!int.TryParse(valueStr, out value))
            {
                throw new ValidateException(string.Format(Resources.WrongValueForIntParameterFormatMessage, nameOfParameter));
            }

            return value;
        }

        public int ValidateNotNegativeIntParameter(string valueStr, string nameOfParameter)
        {
            int value = ValidateIntParameter(valueStr, nameOfParameter);
            if (value < 0)
            {
                throw new ValidateException(string.Format(Resources.ValueCannotBeNegativeFormat, nameOfParameter));
            }

            return value;
        }

        public void ValidateEmailAddresses(string emails)
        {
            foreach (string email in emails.Split(';'))
            {
                // Regular expression pattern for valid email addresses
                const string pattern = @"^(?("")("".+?""@)|(([\w]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[\w])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([\w][-\w]*[\w]\.)+[^\d\-_,;\<\>\~\(\)\\"":@!#\.\$%&'\*\+/=\?\^`\{\}\|]{2,6}))$";

                // Regular expression object
                var check = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);

                // Make sure an email address was provided
                if (string.IsNullOrEmpty(email))
                {
                    throw new ValidateException(Resources.EmptyEmailMessage);
                }

                if (!check.IsMatch(email))
                {
                    throw new ValidateException(string.Format(Resources.WrongEmailFormatMessage, email));
                }
            }
        }

        public void ValidatePossibleFileName(string name)
        {
            var wrongCharacters = new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
            
            if (wrongCharacters.Any(name.Contains))
            {
                throw new ValidateException(string.Format(Resources.WrongFileNameFormat, string.Join(" ", wrongCharacters)));
            }
        }
    }
}
