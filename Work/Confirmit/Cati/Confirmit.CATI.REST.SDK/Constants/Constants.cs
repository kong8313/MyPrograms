namespace Confirmit.CATI.REST.SDK.Constants
{
    /// <summary>
    /// Class contains constants that are used in CATI REST SDK
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Identifier of the default call center. The value is 1.
        /// </summary>
        public const int DefaultCallCenterId = 1;

        /// <summary>
        /// Identifier of the 'CATI Interviewers' group. The value is 14.
        /// </summary>
        public const int CatiInterviewersRootGroupId = 14;

        /// <summary>
        /// Name of the "CATI Interviewers" group. The value is 'CATI Interviewers'.
        /// </summary>
        public const string CatiInterviewersRootGroupName = "CATI Interviewers";

        /// <summary>
        /// Custom HTTP header name that is used in HTTP requests to CATI REST API. The value is X-Confirmit-ApiKey.
        /// </summary>
        public const string XConfirmitApiKeyHeader = "X-Confirmit-ApiKey";
    }
}

