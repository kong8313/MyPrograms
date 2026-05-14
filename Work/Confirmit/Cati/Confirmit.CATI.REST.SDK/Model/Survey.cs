namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// The class presents general information about the survey
    /// </summary>
    public class Survey
    {
        /// <summary>
        /// Unique identifier of the survey (pXXXXXXXX)
        /// </summary>
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
