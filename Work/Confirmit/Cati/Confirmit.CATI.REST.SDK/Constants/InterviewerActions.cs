namespace Confirmit.CATI.REST.SDK.Constants
{
    /// <summary>
    /// Class with constants for http requests to work with interviewers via InterviewerService class
    /// </summary>
    public class InterviewerActions
    {
        /// <summary>
        /// Default namespace. The value is Controller.
        /// </summary>
        public const string Namespace = "Controller";

        /// <summary>
        /// Part of the url to assign an interviewer to a survey. The value is AssignOnSurvey.
        /// </summary>
        public const string AssignOnSurvey = "AssignOnSurvey";

        /// <summary>
        /// Part of the url to unassign an interviewer from a survey. The value is DeAssignFromSurvey.
        /// </summary>
        public const string DeAssignFromSurvey = "DeAssignFromSurvey";

        /// <summary>
        /// Part of the url to assign an interviewer to a call. The value is AssignOnCall.
        /// </summary>
        public const string AssignOnCall = "AssignOnCall";

        /// <summary>
        /// Part of the url to unassign an interviewer from a call. The value is DeAssignFromCalls.
        /// </summary>
        public const string DeAssignFromCalls = "DeAssignFromCalls";

        /// <summary>
        /// Part of the url to remove all assignments from an interviewer. The value is CleanAssignments.
        /// </summary>
        public const string CleanAssignments = "CleanAssignments";

        /// <summary>
        /// Part of the url to lock an interviewer. The value is Lock.
        /// </summary>
        public const string Lock = "Lock";

        /// <summary>
        /// Part of the url to unlock an interviewer. The value is Unlock.
        /// </summary>
        public const string Unlock = "Unlock";

        /// <summary>
        /// Part of the url to get parent groups of an interviewer. The value is GetGroups.
        /// </summary>
        public const string GetGroups = "GetGroups";

        /// <summary>
        /// Part of the url to get assignment information related to an interviewer. The value is GetAssignments.
        /// </summary>
        public const string GetAssignments = "GetAssignments";
    }
}