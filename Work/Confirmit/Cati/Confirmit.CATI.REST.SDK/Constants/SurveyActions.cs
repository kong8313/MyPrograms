namespace Confirmit.CATI.REST.SDK.Constants
{
    /// <summary>
    /// Class with constants for http requests to work with surveys via SurveyService class
    /// </summary>
    public class SurveyActions
    {
        /// <summary>
        /// Default namespace. The value is Controller.
        /// </summary>
        public const string Namespace = "Controller";

        /// <summary>
        /// Part of the url to open a survey. The value is Open.
        /// </summary>
        public const string Open = "Open";

        /// <summary>
        /// Part of the url to close a survey. The value is Close.
        /// </summary>
        public const string Close = "Close";

        /// <summary>
        /// Part of the url to shutdown a survey. The value is Shutdown.
        /// </summary>
        public const string Shutdown = "Shutdown";

        /// <summary>
        /// Part of the url to remove all assignments on a survey. The value is CleanAssignments.
        /// </summary>
        public const string CleanAssignments = "CleanAssignments";

        /// <summary>
        /// Part of the url to get information about an assignments on a survey. The value is GetAssignments.
        /// </summary>
        public const string GetAssignments = "GetAssignments";

        /// <summary>
        /// Part of the url to get basic properties of a surveys. The value is GetBasicProperties.
        /// </summary>
        public const string GetBasicProperties = "GetBasicProperties";

        /// <summary>
        /// Part of the url to update basic properties of a surveys. The value is PutBasicProperties.
        /// </summary>
        public const string PutBasicProperties = "PutBasicProperties";
    }
}