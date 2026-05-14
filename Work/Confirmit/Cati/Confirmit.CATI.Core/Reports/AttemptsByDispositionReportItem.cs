using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports
{
    /// <summary>
    /// Single row for attempts by disposition report.
    /// </summary>
    [Serializable]
    public class AttemptsByDispositionReportItem
    {
        /// <summary>
        /// ITS (extended status) ID.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// ITS (extended status) title.
        /// </summary>
        public string Disposition { get; set; }

        /// <summary>
        /// Number of interviews with 1 attempt.
        /// </summary>
        public int Attempts1 { get; set; }

        /// <summary>
        /// Number of interviews with 2 attempts.
        /// </summary>
        
        public int Attempts2 { get; set; }
        
        /// <summary>
        /// Number of interviews with 3 attempts.
        /// </summary>
        public int Attempts3 { get; set; }
        
        /// <summary>
        /// Number of interviews with 4 attempts.
        /// </summary>
        public int Attempts4 { get; set; }
        
        /// <summary>
        /// Number of interviews with 5 attempts.
        /// </summary>
        public int Attempts5 { get; set; }
        
        /// <summary>
        /// Number of interviews with 6 attempts.
        /// </summary>
        public int Attempts6 { get; set; }
        
        /// <summary>
        /// Number of interviews with 7 attempts.
        /// </summary>
        public int Attempts7 { get; set; }
        
        /// <summary>
        /// Number of interviews with 8 attempts.
        /// </summary>
        public int Attempts8 { get; set; }
        
        /// <summary>
        /// Number of interviews with 9 attempts.
        /// </summary>
        public int Attempts9 { get; set; }
        
        /// <summary>
        /// Number of interviews with 10 attempts.
        /// </summary>
        public int Attempts10 { get; set; }

        /// <summary>
        /// Number of interviews with 11 attempts and more.
        /// </summary>
        public int Attempts11AndMore { get; set; }
    }

    [Serializable]
    public class AttemptsByDispositionReportItemList : List<AttemptsByDispositionReportItem>
    {
        public AttemptsByDispositionReportItemList() { }

        public AttemptsByDispositionReportItemList(IEnumerable<AttemptsByDispositionReportItem> items) : base(items) { }
    }
}
