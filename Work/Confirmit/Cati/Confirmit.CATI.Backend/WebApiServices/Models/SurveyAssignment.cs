using System.ComponentModel.DataAnnotations;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// The class presents information about assignments to a survey
    /// </summary>
    public class SurveyAssignment
    {
        /// <summary>
        /// Unique identifier of the survey (pXXXXXXXX)
        /// </summary>
        [Key]
        public string SurveyId { get; set; }

        /// <summary>
        /// Count of the calls assigned to the survey
        /// </summary>
        public int AssignedCallsCount { get; set; }

        /// <summary>
        /// Type of the assignment
        /// </summary>
        public AssignmentType AssignmentType { get; set; }
    }
}
