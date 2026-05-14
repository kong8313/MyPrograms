using System.ComponentModel.DataAnnotations;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// The class presents some basic properties of the survey
    /// </summary>
    public class SurveyBasicProperties
    {
        /// <summary>
        /// Unique identifier of the survey (pXXXXXXXX)
        /// </summary>
        [Key]
        public string SurveyId { get; set; }

        /// <summary>
        /// Name of the extended status codes group assigned to the survey
        /// </summary>
        public string ExtendedStatusGroup { get; set; }

        /// <summary>
        /// Name of the scheduling script assigned to the survey
        /// </summary>
        public string Scheduling { get; set; }

        /// <summary>
        /// Call delivery mode of the survey
        /// </summary>
        public CallDeliveryMode CallDeliveryMode { get; set; }

        /// <summary>
        /// A flag indicating that survey uses call groups
        /// </summary>
        public bool CallGroups { get; set; }
    }
}