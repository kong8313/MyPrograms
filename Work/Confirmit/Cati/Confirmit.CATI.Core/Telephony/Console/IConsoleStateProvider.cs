using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Console
{
    public interface IConsoleStateProvider
    {
        /// <summary>
        /// It is the request for the current interview and interviewer state.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="person"></param>
        /// <param name="evt"></param>
        /// <param name="activityEvent"></param>
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
        State GetState(BvTasksEntity task, BvPersonEntity person, GetStateEvent evt,
            UrlGeneratedInGetStateEvent activityEvent);
    }
}