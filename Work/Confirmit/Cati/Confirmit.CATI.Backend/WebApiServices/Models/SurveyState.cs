namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Enum with possible survey states
    /// </summary>
    public enum SurveyState
    {
        /// <summary>
        /// Closed state
        /// </summary>
        Closed = 0,

        /// <summary>
        /// Open state
        /// </summary>
        Open = 1,

        /// <summary>
        /// Survey has been marked for deletion and is not available for users
        /// </summary>
        SoftDeleted = 2
    }
}