namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Describes an CATI interview control data.
    /// Interview control data is an entity binding an interview and respondent.
    /// </summary>
    public class InterviewControlData
    {
        /// <summary>
        /// The Interview owner survey id.
        /// </summary>
        /// <remarks>
        /// Interview is uniquely identified by the pair (surveyId, interviewId)
        /// Confirmit survey ID like pNNNNNNN is used.
        /// </remarks>
        public string surveyId;

        /// <summary>
        /// The interview id.
        /// </summary>
        /// <remarks>
        /// Interview is uniquely identified by the pair (surveyId, interviewId)
        /// </remarks>
        public int interviewId;

        /// <summary>
        /// The respondent name.
        /// </summary>
        public string respondentName;

        /// <summary>
        /// The respondent phone number.
        /// </summary>
        public string respondentPhone;

        /// <summary>
        /// Interview status.
        /// See Fusion documentation for the full list of possible statuses
        /// </summary>
        /// <example>
        /// status = 13 means that the interview is finished.
        /// </example>
        /// <remarks>
        /// Status is the same as ITS (in terms of Fusion).
        /// </remarks>
        public int status;

        /// <summary>
        /// Interview status name according state group assigned to survey.
        /// </summary>
        public string statusName;
    }
}