namespace Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader
{
    public class AuthorizationMessageHeaderConstants
    {
        /// <summary>
        /// The namespace to create message headers with.
        /// </summary>
        public const string Namespace = "http://confirmit.com/2010/05/25/AuthorizationMessageHeaderBehavior";

        /// <summary>
        /// The name of the custom message header for interviewer login name.
        /// </summary>
        public const string LoginHeaderName = "Login";

        /// <summary>
        /// The name of the custom message header for authentication key.
        /// </summary>
        public const string KeyHeaderName = "Key";

        /// <summary>
        /// The name of the custom message header for interviewer password.
        /// </summary>
        public const string PasswordHeaderName = "Password";
    }
}