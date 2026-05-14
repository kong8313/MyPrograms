using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class representing information about the call attempt
    /// </summary>
    [Table("RestView_CallHistory")]
    public class CallHistory
    {
        /// <summary>
        /// Unique identifier of the call
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Time of the call attempt
        /// </summary>
        public DateTimeOffset Time { get; set; }

        /// <summary>
        /// Unique identifier of the survey (pXXXXXXXX)
        /// </summary>
        public string SurveyId { get; set; }

        /// <summary>
        /// Unique identifier of the interview
        /// </summary>
        public int? InterviewId { get; set; }

        /// <summary>
        /// Unique identifier of the interviewer
        /// </summary>
        public int? InterviewerId { get; set; }

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
        public int CallCenterId { get; set; }
    }
}
