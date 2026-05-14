using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public interface IPersonAuthorizer
    {
        /// <summary>
        /// Checks person password and increases FailedLoginAttempts count if password is incorrect.(if locking functionality is enabled)
        /// </summary>
        /// <param name="person">person entity</param>
        /// <param name="password">Password to check</param>
        /// <returns>True if password is correct; false if there is no such person or password is incorrect.</returns>
        bool Authorize(BvPersonEntity person, string password);

        /// <summary>
        /// Checks if person password is expired
        /// </summary>
        /// <param name="person"></param>
        /// <param name="interviewerPasswordSettings"></param>
        /// <returns>True, if password is expired, false otherwise</returns>
        bool IsPasswordExpired(BvPersonEntity person, IInterviewerPasswordSettings interviewerPasswordSettings);
    }
}