using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Confirmit.CATI.Backend.Resources;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Contracts.ErrorReportingService;
using Confirmit.CATI.Common.Encryption;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Common.WcfTools.ErrorServiceMessageHeader;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Core.Services.CompanyService;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Backend.WcfServices.External.ErrorReportingService
{
    /// <summary>
    /// Logs the error message from the client on the server (for internal HTTP only).
    /// </summary>
    [ErrorContextHandler(WebServiceType.Internal)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple,
        UseSynchronizationContext = false)]
    public class ErrorReportingServiceHttp : ErrorReportingService
    {
    }

    /// <summary>
    /// Logs the error message from the client on the server.
    /// </summary>
    [ErrorContextHandler(WebServiceType.External)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ErrorReportingService : IErrorReportingService
    {
        private readonly ICompanyInformationService _companyInformation;
        private readonly ICatiSecretKeyHasher _catiSecretKeyHasher;
        private readonly IPasswordHash _passwordHash;
        private readonly IConnectionStrings _connectionStrings;
        private readonly ErrorServiceMessageHeaderBehavior _messageHeaderBehavior;

        public ErrorReportingService() : this(
            ServiceLocator.Resolve<ICompanyInformationService>(),
            new CatiSecretKeyHasher(),
            ServiceLocator.Resolve<IPasswordHash>(),
            ServiceLocator.Resolve<IConnectionStrings>(),
            new LoginPasswordAuthenticationDataProvider("", "", 0), 
            ServiceLocator.Resolve<IMessageHeaderAccessor>())
        {
        }

        public ErrorReportingService(
            ICompanyInformationService companyInformation,
            ICatiSecretKeyHasher catiSecretKeyHasher,
            IPasswordHash passwordHash,
            IConnectionStrings connectionStrings,
            ILoginPasswordAuthenticationDataProvider authenticationDataProvider,
            IMessageHeaderAccessor messageHeaderAccessor)
        {
            _companyInformation = companyInformation;
            _catiSecretKeyHasher = catiSecretKeyHasher;
            _passwordHash = passwordHash;
            _connectionStrings = connectionStrings;
            _messageHeaderBehavior = new ErrorServiceMessageHeaderBehavior(authenticationDataProvider, messageHeaderAccessor);
        }

        public void SendConsoleErrorMessage(
            string companyAlias,
            ClientErrorSource source, 
            string errorMessage, 
            byte[] hash)
        {
            if (ServiceLocator.Resolve<ISystemSettings>().Logging.EnableReceivingClientErrors == false)
            {
                return;
            }

            // If error happend even before interviewer clicked login button we will get null in the company alias field.
            // so, this is expected case and should be handled.
            int companyId = 0;
            if (!string.IsNullOrEmpty(companyAlias))
            {
                companyId = _companyInformation.GetCompanyIdFromAlias(companyAlias);
            }

            if (companyId == 0)
            {
                errorMessage = string.Format(
                    "WARNING: This error message has not been authenticated because company alias '{0}' is not found.\r\n\r\n{1}",
                    companyAlias,
                    errorMessage);
            }
            else
            {
                // TODO: Transaction?
                var task = AuthenticateAndGetTask(companyId);

                if (task == null)
                {
                    errorMessage =
                        "WARNING: This error message has not been authenticated because corresponding person or person task does not exist.\r\n\r\n" +
                        errorMessage;
                }
                else
                {
                    errorMessage = CheckMessageHash(hash, task.EncryptionKey, companyAlias, source, errorMessage);
                }
            }


            LogErrorMessage(companyId, source, errorMessage);
        }

        public void SendMonitoringErrorMessage(
            string companyAlias, 
            ClientErrorSource source, 
            string errorMessage,
            byte[] hash)
        {
            if (ServiceLocator.Resolve<ISystemSettings>().Logging.EnableReceivingClientErrors == false)
            {
                return;
            }

            // If error happend even before we read company alias from the file then companyAlias will be null/
            // so, this is expected case and should be handled.
            int companyId = 0;
            if (!string.IsNullOrEmpty(companyAlias))
            {
                companyId = _companyInformation.GetCompanyIdFromAlias(companyAlias);
            }

            if (companyId == 0)
            {
                errorMessage = string.Format(
                    "WARNING: This error message has not been authenticated because company alias '{0}' is not found.\r\n\r\n{1}",
                    companyAlias,
                    errorMessage);
            }
            else
            {
                errorMessage = CheckMessageHash(hash, Constants.HashKey, companyAlias, source, errorMessage);
            }

            LogErrorMessage(companyId, source, errorMessage);
        }

        public void SendDialerErrorMessages(IEnumerable<ErrorMessage> errorMessages)
        {
            foreach (var em in errorMessages)
            {
                LogErrorMessage(em.CompanyId, ClientErrorSource.DialerError, em.Message);    
            }            
        }

        public void SendLoadUtilityErrorMessages(IEnumerable<ErrorMessage> errorMessages)
        {
            foreach (var em in errorMessages)
            {
                LogErrorMessage(em.CompanyId, ClientErrorSource.LoadUtilityError, em.Message);    
            }
        }

        private BvTasksEntity AuthenticateAndGetTask(int companyId)
        {
            var interviewerName = _messageHeaderBehavior.GetIncomingMessageLogin();

            var interviewerPassword = _messageHeaderBehavior.GetIncomingMessagePassword();

            var companyInstanceDbConnectionString = _connectionStrings.GetConnectionStringForSpecificCompany(companyId);

            using (var connectionScope = new ConnectionScope(companyInstanceDbConnectionString))
            {
                BvPersonEntity person = PersonRepository.GetByName(interviewerName);

                if (person == null)
                {
                    return null;
                }

                if (_passwordHash.ValidateHash(interviewerPassword, person.PwdSaltTxt, person.PwdHashTxt) == false)
                {
                    return null;
                }

                return TaskRepository.GetByPerson(person.SID);
            }
        }

        private void LogErrorMessage(int companyId, ClientErrorSource source, string errorMessage)
        {
            TraceHelper.TraceClientError(companyId, errorMessage, source);
        }

        private string CheckMessageHash(byte[] givenHash, byte[] secretKey, string companyAlias, ClientErrorSource source, string errorMessage)
        {
            if (_catiSecretKeyHasher.VerifyComputedHash(secretKey, givenHash, companyAlias, source, errorMessage))
            {
                return errorMessage;
            }

            return string.Format(
                "WARNING: This error message has not been authenticated because the supplied message hash is invalid: '[{0}]'.\r\n\r\n{1}",
                givenHash.Select(x => x.ToString(CultureInfo.InvariantCulture)).JoinInString(","),
                errorMessage);
        }
    }
}
