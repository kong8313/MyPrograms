namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Enum with possible task choice modes of interviewer
    /// </summary>
    public enum TaskChoiceMode
    {
        /// <summary>
        /// Automatic task choice mode.
        /// The interviewer cannot select an interview; the system allocates them.
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Manual task choice mode.
        /// The interviewer can select the interview that is to be started from all the available surveys.
        /// </summary>
        Manual = 1,

        /// <summary>
        /// Survey assignment task choice mode.
        /// After logging in, the interviewer can select the survey they wish to work with.
        /// The system then allocates interviews to the interviewer from the selected survey only.
        /// </summary>
        SurveyAssignment = 2,

        /// <summary>
        /// Choice task choice mode.
        /// After logging in, the interviewer can select the mode in which they wish to work.
        /// The interviewer can select the mode from all available task choice permissions.
        /// </summary>
        Choice = 3
    }
}