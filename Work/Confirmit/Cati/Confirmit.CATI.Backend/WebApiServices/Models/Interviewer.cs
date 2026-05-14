using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class representing information about the interviewer
    /// </summary>
    [Table("RestView_Interviewer")]
    public class Interviewer
    {
        /// <summary>
        /// Unique identifier of the interviewer
        /// </summary>
        [Key]
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
        /// Display name of the interviewer
        /// </summary>
        [Column("FullName")]
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Task choice: 0 - Automatic, 1 - Manual, 2 - SurveyAssignment and 3 - Choice.
        /// If [AllowedChoices] is not null the system decides that this field has Choice mode (3)
        /// but in reality it can have any value in this case.
        /// </summary>
        public int ManualSelection { get; set; }

        /// <summary>
        /// Flag to show that there is a new message from supervisor for the interviewer
        /// </summary>
        public bool? HasNewMessage { get; set; }

        /// <summary>
        /// Unique identifier of the survey to be logged into automatically if [ManualSelection] is the survey assignment mode (2)
        /// </summary>
        public string AutomaticSurveyId { get; set; }

        /// <summary>
        /// Available task choices if [ManualSelection] is Choice mode (3).
        /// Can have one or several values from Automatic, Manual or SurveyAssignment.
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
        /// Dial type of the interviewer: 0 - Automatic, 1 - Manual, 2 - Assisted
        /// </summary>
        public byte? DialTypeId { get; set; }
        
        /// <summary>
        /// Additional interviewer's attribute 1 
        /// </summary>
        public string Attribute1 { get; set; }
        
        /// <summary>
        /// Additional interviewer's attribute 2 
        /// </summary>
        public string Attribute2 { get; set; }
        
        /// <summary>
        /// Additional interviewer's attribute 3 
        /// </summary>
        public string Attribute3 { get; set; }
        
        /// <summary>
        /// Additional interviewer's attribute 4 
        /// </summary>
        public string Attribute4 { get; set; }
        
        /// <summary>
        /// Additional interviewer's attribute 5 
        /// </summary>
        public string Attribute5 { get; set; }
    }
}
