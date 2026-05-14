using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    public interface IConsoleServiceHelper
    {
        Timezone GetTimeZone(int timezoneId);

        /// <summary>
        /// The person a logout process.
        /// This internal method runs in a separate thread.
        /// </summary>
        void LogoutProcess(
            int personId,
            string company,
            LoginState loggedInToDialerState,
            bool isLoginRcToDialer,
            string projectId,
            int dialerId);

        bool SetPendingBreakStatus(
            BvTasksEntity task,
            BvPersonEntity person,
            PendingBreakStatus status,
            int? breakTypeId);

        /// <summary>
        /// This method is called from console when person is returning from break;
        /// Console retries this method if communication error has occured.
        /// It can be in several cases: 
        ///     1) this method was not called
        ///     2) this method threw exception but this exception didn't reach the console 'cause communication problem (console got communication error)
        ///     3) this method was successfully completed but console got communication error
        /// </summary>
        void ContinueWorkAfterBreak(BvTasksEntity task, int attemptNumber);

        void SwitchSurveyIfNeeded(BvTasksEntity task);
    }
}