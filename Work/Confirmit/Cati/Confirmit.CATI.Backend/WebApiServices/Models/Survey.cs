using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// The class presents general information about the survey
    /// </summary>
    [Table("RestView_Survey")]
    public class Survey
    {
        /// <summary>
        /// Unique identifier of the survey (pXXXXXXXX)
        /// </summary>
        [Key]
        public string SurveyId { get; set; }

        /// <summary>
        /// Name of the survey
        /// </summary>
        public string SurveyName { get; set; }

        /// <summary>
        /// Size of sample for the survey
        /// </summary>
        public int SampleSize { get; set; }

        /// <summary>
        /// Current survey state
        /// </summary>
        public SurveyState State { get; set; }
    }
}
