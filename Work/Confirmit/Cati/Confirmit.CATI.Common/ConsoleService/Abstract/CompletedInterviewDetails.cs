namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Describes completed interview information
    /// </summary>
    public class CompletedInterviewDetails
    {
        /// <summary>
        /// Interview status returned from the Confirmit
        /// </summary>
        public string Status;

        /// <summary>
        /// Interview its returned from the Confirmit
        /// </summary>
        public string Its;

        /// <summary>
        /// Interview duration returned from the Confirmit
        /// </summary>
        public int InterviewDuration;
    }
}