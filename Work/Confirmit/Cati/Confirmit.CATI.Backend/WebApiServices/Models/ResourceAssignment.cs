using System.ComponentModel.DataAnnotations;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// The class presents information about the assignment of interviewers and interviewer groups
    /// </summary>
    public class ResourceAssignment
    {
        /// <summary>
        /// Unique identifier of the interviewer or the interviewer group
        /// </summary>
        [Key]
        public int ResourceId { get; set; }

        /// <summary>
        /// Name of the interviewer or the interviewer group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag to show that this resource is a group
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// The number of explicitly assigned calls
        /// </summary>
        public int AssignedCallsCount { get; set; }
    }
}