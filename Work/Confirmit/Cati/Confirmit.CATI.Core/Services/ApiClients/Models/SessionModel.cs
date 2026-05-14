using System;

namespace Confirmit.CATI.Core.Services
{
    public class SessionModel
    {
        public bool CanUserReviewAllQuestionTypes { get; set; }
        public int? CompanyId { get; set; }
        public int? CreatedByCompanyId { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public int[] InterviewIds { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime? LockDate { get; set; }
        public int? LockedBy { get; set; }
        public string Name { get; set; }
        public Progress Progress { get; set; }
        public string ProjectId { get; set; }
        public int SessionId { get; set; }
        public SessionStatus Status { get; set; }
        public DateTime StatusChangeDate { get; set; }
    }
    
    public enum SessionStatus : byte
    {
        New,
        Started,
        Completed
    }
    
    public class Progress
    {
        public int Total { get; set; }
        public int Completed { get; set; }
    }
}