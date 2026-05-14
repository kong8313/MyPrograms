using System;

namespace Confirmit.CATI.Supervisor.Script.Classes
{
    [Serializable]
    public class SchedulingRulesOperationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Specifies row key that should be highlighted after operation is complete.
        /// For example if new row has been created after copy/paste operation, new pasted row should be highlighted.
        /// </summary>
        public string HighlightRowKey { get; set; }
    }
}
