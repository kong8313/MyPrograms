using System;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Class representing information about the interviewer
    /// </summary>
    public class Interviewer
    {
        /// <summary>
        /// Unique identifier of the interviewer
        /// </summary>
        public int InterviewerId { get; set; }

        /// <summary>
        /// Name of the interviewer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the interviewer
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Task choice: manual, automatic, survey assignment and choice
        /// </summary>
        public int ManualSelection { get; set; }

        /// <summary>
        /// Flag to show that there is a new message from supervisor for the interviewer
        /// </summary>
        public bool? HasNewMessage { get; set; }

        /// <summary>
        /// Unique identifier of the survey to be logged into automatically if [ManualSelection] is the survey assignment mode
        /// </summary>
        public string AutomaticSurveyId { get; set; }

        /// <summary>
        /// Available task choices if [ManualSelection] is choice mode
        /// </summary>
        public TaskChoicePermissions? AllowedChoices { get; set; }

        /// <summary>
        /// Flag to mark the locked interviewer
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Date and time when the user was locked
        /// </summary>
        public DateTimeOffset? LockedDate { get; set; }

        /// <summary>
        /// Identifies which calls are to be commenced
        /// </summary>
        public AssignmentListMode AssignmentsListMode { get; set; }

        /// <summary>
        /// Unique identifier of the call group.
        /// For an interviewer to be delivered calls automatically for a survey where Call Groups are enabled, the interviewer must be a member of a Call Group.
        /// If they are not a member of a Call Group, no calls will be delivered automatically.
        /// An interviewer can be a member of one Call Group only.This can be changed at any time, and any changes will take effect on the next call delivered.
        /// Interviewers who are working as a member of a call group must have the "Survey Selection" task choice, otherwise Call Groups cannot be used.
        /// When working as a member of a Call Group, calls are delivered based on the Extended Statuses in that Call Group.
        /// </summary>
        public int? CallGroupId { get; set; }

        /// <summary>
        /// Unique identifier of the call center
        /// </summary>
        public int CallCenterId { get; set; }

        /// <summary>
        /// The location field is an optional property that can be used to associate the interviewer with a localized dialer gateway.
        /// Please note that only valid location names (as specified in the dialer gateway configuration settings) should be entered here.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Date of the last password change
        /// </summary>
        public DateTimeOffset PwdSetDate { get; set; }

        /// <summary>
        /// Dial type of the interviewer
        /// </summary>
        public byte? DialTypeId { get; set; }
    }
}
