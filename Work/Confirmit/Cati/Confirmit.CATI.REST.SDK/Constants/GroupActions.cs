namespace Confirmit.CATI.REST.SDK.Constants
{
    /// <summary>
    /// Class with constants for HTTP requests to work with groups via GroupService class
    /// </summary>
    public class GroupActions
    {
        /// <summary>
        /// Default namespace. The value is Controller.
        /// </summary>
        public const string Namespace = "Controller";

        /// <summary>
        /// Part of the url to assign a group to a survey. The value is AssignOnSurvey.
        /// </summary>
        public const string AssignOnSurvey = "AssignOnSurvey";

        /// <summary>
        /// Part of the url to unassign a group from a survey. The value is DeAssignFromSurvey.
        /// </summary>
        public const string DeAssignFromSurvey = "DeAssignFromSurvey";

        /// <summary>
        /// Part of the url to get list of assigned to a group interviewers. The value is GetInterviewers.
        /// </summary>
        public const string GetInterviewers = "GetInterviewers";

        /// <summary>
        /// Part of the url to get assignment information related to a group. The value is GetAssignments.
        /// </summary>
        public const string GetAssignments = "GetAssignments";

        /// <summary>
        /// Part of the url to assign a group to a call. The value is AssignOnCall.
        /// </summary>
        public const string AssignOnCall = "AssignOnCall";

        /// <summary>
        /// Part of the url to unassign a group from a call. The value is DeAssignFromCalls.
        /// </summary>
        public const string DeAssignFromCalls = "DeAssignFromCalls";
    }
}
