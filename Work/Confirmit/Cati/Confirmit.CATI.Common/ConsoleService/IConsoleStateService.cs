using System.ServiceModel;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Common.ConsoleService
{
    /// <summary>
    /// WCF service contract for CATI console. Contains methods that does not require SSL encryption.
    /// </summary>
    [ServiceContract(Name = "ConsoleStateService", Namespace = "http://www.confirmit.com/ConsoleStateService/05/27/2010")]
    public interface IConsoleStateService
    {
        /// <summary>
        /// CATI console must call this function each "KeepAliveTimeout" seconds.
        /// (CATI console obtains "KeepAliveTimeout" from CATIConsoleWebServ at login).
        /// It allows Fusion realize that CATI console user is alive.
        /// </summary>
        /// <example>
        /// User didn't log out via "Logout" but closed the CATI console or turned off the computer.
        /// So in "KeepAliveTimeout" seconds CATIConsoleWebServ will find out that user does not keep alive and CATIConsoleWebServ
        /// will logout the user automatically.
        /// </example>
        [OperationContract]
        [FaultContract(typeof(InterviewerNotLoggedInExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        [FaultContract(typeof(StateServiceSessionExpiredExceptionDetails))]
        KeepAliveResult KeepAlive();

        /// <summary>
        /// It is the request for the current interview and interviewer state.
        /// </summary>
        /// <remarks>
        /// CATI console calls this method to find out:
        /// - the login state of the interviewer
        /// - the login to dialer state of the interviewer
        /// - the previously started by <code>StartInterview</code> method interview and obtain 
        /// the interview URL in order to start it on Confirmit
        /// - that another a new interview was started
        /// - that there are no calls for the interviewer 
        /// - ...
        /// </remarks>
        /// <returns>
        /// The state parameters are returned. 
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(InterviewerNotLoggedInExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        [FaultContract(typeof(StateServiceSessionExpiredExceptionDetails))]
        State GetState();
    }
}