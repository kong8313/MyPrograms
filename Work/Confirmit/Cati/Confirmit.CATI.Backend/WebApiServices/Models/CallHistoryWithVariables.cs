using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class representing information about the call attempt including survey fields
    /// </summary>
    public class CallHistoryWithVariables
    {
        /// <summary>
        /// Unique number of entity
        /// </summary>
        [Key]
        public int NumberInOrder { get; set; }

        /// <summary>
        /// Unique identifier of the call
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Time of the call
        /// </summary>
        public DateTimeOffset? Time { get; set; }

        /// <summary>
        /// Unique identifier of the survey (pXXXXXXXX)
        /// </summary>
        public string SurveyId { get; set; }

        /// <summary>
        /// Survey name
        /// </summary>
        public string SurveyName { get; set; }

        /// <summary>
        /// Unique identifier of the interview
        /// </summary>
        public int? InterviewId { get; set; }

        /// <summary>
        /// Unique identifier of the interviewer
        /// </summary>
        public int? InterviewerId { get; set; }

        /// <summary>
        /// Interviewer name
        /// </summary>
        public string InterviewerName { get; set; }

        /// <summary>
        /// Telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Extended status that has been assigned to the interview after the call attempt
        /// </summary>
        public short? ExtendedStatus { get; set; }

        /// <summary>
        /// Duration of the call attempt in seconds
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Waiting time (in seconds) before the call has started
        /// </summary>
        public int? WaitingTime { get; set; }

        /// <summary>
        /// Unique identifier of the call center
        /// </summary>
        public int? CallCenterId { get; set; }

        /// <summary>
        /// List of survey fields
        /// </summary>
        public List<Variable> Variables { get; set; }
    }
}
