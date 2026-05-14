using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class representing information about interviewer break
    /// </summary>
    [Table("RestView_BreakHistory")]
    public class BreakHistory
    {
        /// <summary>
        /// Unique identifier of the break
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Time when the break was started
        /// </summary>
        public DateTimeOffset Time { get; set; }

        /// <summary>
        /// Unique identifier of a survey
        /// </summary>
        public string SurveyId { get; set; }

        /// <summary>
        /// Duration of the break in seconds
        /// </summary>
        public int? Duration { get; set; }

        /// <summary>
        /// Unique identifier of an interviewer
        /// </summary>
        public int InterviewerId { get; set; }

        /// <summary>
        /// Unique identifier of a call center
        /// </summary>
        public int CallCenterId { get; set; }

        /// <summary>
        /// Unique identifier of a break type
        /// </summary>
        public int BreakTypeId { get; set; }

        /// <summary>
        /// A flag indicating if the break is paid
        /// </summary>
        public bool? IsPaid { get; set; }

        /// <summary>
        /// Name of the break type
        /// </summary>
        public string BreakTypeName { get; set; }
    }
}
