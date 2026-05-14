using System.ServiceModel;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Common.Contracts.ConsoleAuthorizationService
{
    [ServiceContract(Name = "ConsoleAuthorizationService", Namespace = "http://www.confirmit.com/ConsoleAuthorizationService/24/03/2011")]
    public interface IConsoleAuthorizationService
    {
        /// <summary>
        /// Gets CATI company id for authorized user.
        /// Throws InvalidInterviewerCredentialsException if no such CATI company found or user not found or user is not authorized.
        /// Throws PasswordExpiredException if user password is expired.
        /// Throws UserAlreadyLoggedInException if user is already logged in from another computer.
        /// </summary>
        /// <param name="interviewerName">User name</param>
        /// <param name="interviewerPassword">Password</param>
        /// <param name="catiCompanyAlias">CATI company alias</param>
        /// <param name="stationId"></param>
        /// <returns>CATI company id</returns>
        [OperationContract]
        [FaultContract(typeof(InvalidInterviewerCredentialsExceptionDetails))]
        [FaultContract(typeof(PasswordExpiredExceptionDetails))]
        [FaultContract(typeof(UserAlreadyLoggedInException))]
        int AuthorizeAndReturnCompanyId(
            string interviewerName, string interviewerPassword, string catiCompanyAlias, string stationId);

        /// <summary>
        /// Changes CATI interviewer password. 
        /// Returns nothing.
        /// Throws InvalidInterviewerCredentialsException if no such CATI company found or user not found or user old password is incorrect.
        /// Throws UserAlreadyLoggedInException if user is already logged in from another computer.
        /// Throws TheSamePasswordException if newPassword equals to oldPassword.
        /// Throws TooShortPasswordException if newPassword is shorter then it is allowed by CATI system settings.
        /// Throws PasswordDoesNotSatisfyRulesException if newPassword does not satisfy password rules.
        /// The method can be called both when interviewer is logged in to CATI console and when not.
        /// </summary>
        /// <param name="interviewerName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="catiCompanyAlias"></param>
        /// <param name="stationId"></param>
        [OperationContract]
        [FaultContract(typeof(InvalidInterviewerCredentialsExceptionDetails))]
        [FaultContract(typeof(TheSamePasswordExceptionDetails))]
        [FaultContract(typeof(TooShortPasswordExceptionDetails))]
        [FaultContract(typeof(PasswordDoesNotSatisfyRulesExceptionDetails))]
        void ChangePersonPassword(string interviewerName, string oldPassword, string newPassword, string catiCompanyAlias, string stationId);

        [OperationContract]
        bool IsLatestVersion(string version);
    }
}