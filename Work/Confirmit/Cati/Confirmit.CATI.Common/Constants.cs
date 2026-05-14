namespace Confirmit.CATI.Common
{
    /// <summary>
    /// This class contains global constants.
    /// </summary>
    public class Constants
    {        
        ///<summary>
        ///
        ///</summary>
        public const string InvalidInterviewerCredentialsFaultCode = "InvalidCredentials";

        /// <summary>
        /// The FaultCode name
        /// </summary>
        public const string InternalServerErrorFaultCode = "InternalServerErrorFaultCode";

        public static readonly byte[] HashKey = new byte[] {12, 5, 213, 33, 11, 56, 45, 178};
    }
}
