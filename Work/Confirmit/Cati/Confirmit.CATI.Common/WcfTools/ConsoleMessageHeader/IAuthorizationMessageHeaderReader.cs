using System;

namespace Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader
{
    public interface IAuthorizationMessageHeaderReader
    {
        /// <summary>
        /// Gets the interviewer login name from the custom header of the current incoming message.
        /// </summary>
        /// <returns>The interviewer login name if found, empty string otherwise.</returns>
        string GetIncomingMessageLogin();

        /// <summary>
        /// Gets the authentication key from the custom header of the current incoming message.
        /// </summary>
        /// <returns>The authentication key if found, Guid.Empty otherwise.</returns>
        Guid GetIncomingMessageKey();

        /// <summary>
        /// Gets the interviewer password from the custom header of the current incoming message.
        /// </summary>
        /// <returns>The interviewer password if found, empty string otherwise.</returns>
        string GetIncomingMessagePassword();
    }
}