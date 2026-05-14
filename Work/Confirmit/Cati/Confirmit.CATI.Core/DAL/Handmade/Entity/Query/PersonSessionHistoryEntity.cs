using System;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Query
{
    public class PersonSessionHistoryEntity
    {
        public int CallCenterId { get; set; }

        public int CompanyId { get; set; }

        public int SessionId { get; set; }

        public string CallCenterName { get; set; }

        public int InterviewerId { get; set; }

        public string InterviewerName { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }
    }
}
