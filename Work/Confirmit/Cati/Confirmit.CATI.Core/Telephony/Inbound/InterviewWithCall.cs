using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public class InterviewWithCall
    {
        public BvSurveyEntity Survey { get; set; }
        public BvInterviewWithOriginEntity Interview { get; set; }
        public BvCallEntity Call { get; set; }
        public bool IsCallLockAcquired { get; set; }

        public int? SurveyId { get { return Survey == null ? (int?)null : Survey.SID; } }
        public int? InterviewId { get { return Interview == null ? (int?)null : Interview.ID; } }
        public int? CallId { get { return Call == null ? (int?)null : Call.CallID; } }
    }
}