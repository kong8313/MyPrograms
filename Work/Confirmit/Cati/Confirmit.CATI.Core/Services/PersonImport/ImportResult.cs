using System.Collections.Generic;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.Core.Services.PersonImport
{
    /// <summary>
    /// Represents interviewer importing result.
    /// </summary>
    public class ImportResult
    {
        public ImportResult()
        {
            Warnings = new List<string>();
            Interviewers = new List<InterviewerImportDetails>();            
        }

        /// <summary>
        /// Count of processed rows.
        /// </summary>
        public int RowsProcessed{ get; set; }

        /// <summary>
        /// Count of created groups.
        /// </summary>
        public int GroupsCreated { get; set; }

        /// <summary>
        /// Count of created persons.
        /// </summary>
        public int PersonsCreated { get; set; }

        public string Log { get; set; }

        public List<string> Warnings { get; private set; }

        public List<InterviewerImportDetails> Interviewers
        {
            get; 
            private set;
        }

        public int AutomaticSurveySet { get; set; }

        public int AutomaticSurveyReset { get; set; }
    }
}