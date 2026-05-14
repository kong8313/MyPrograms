using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Backend.WebApiServices.Models
{
    /// <summary>
    /// Class representing extended information about the interviewer
    /// </summary>
    public class InterviewerProperties
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
        public string DisplayName { get; set; }

        /// <summary>
        /// Encrypted password of the interviewer
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The location field is an optional property that can be used to associate the interviewer with a localized dialer gateway.
        /// Please note that only valid location names (as specified in the dialer gateway configuration settings) should be entered here.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Task choice. This value is ignored if AllowedTaskChoice is specified.
        /// </summary>
        public TaskChoiceMode Mode { get; set; }

        /// <summary>
        /// Identifies which calls are to be commenced
        /// </summary>
        public AssignmentListMode AssignmentsListMode { get; set; }

        /// <summary>
        /// Available task choices if [Mode] is choice mode.
        /// Can have one or several values from Automatic, Manual or SurveyAssignment.
        /// </summary>
        public List<TaskChoicePermissions> AllowedTaskChoice { get; set; }

        /// <summary>
        /// List of unique identifiers of the parent groups of the interviewer
        /// </summary>
        public List<int> ParentGroups { get; set; }

        /// <summary>
        /// Unique identifier of the survey to automatically login to if [Mode] is survey assignment
        /// </summary>
        public string AutomaticSurveyId { get; set; }

        /// <summary>
        /// Unique identifier of the call group
        /// </summary>
        public int CallGroupId { get; set; }

        /// <summary>
        /// Unique identifier of the call center
        /// </summary>
        public int CallCenterId { get; set; }

        /// <summary>
        /// Dial type of the interviewer
        /// </summary>
        public DialType DialType { get; set; } 
        
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
