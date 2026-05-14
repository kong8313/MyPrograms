using System;

namespace Confirmit.CATI.Core.Reports
{
    public partial class InterviewerSessionsReportEntity
    {
        public string PersonName{get;set;}
        public DateTime? StartTime{get;set;}
        public DateTime? FinishTime{get;set;}
        public int? Duration{get;set;}
        public int? Event{get;set;}
        public string Note{get;set;}
    }
}
