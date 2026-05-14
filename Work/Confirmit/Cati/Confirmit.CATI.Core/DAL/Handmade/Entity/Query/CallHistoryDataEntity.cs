using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Query
{
    public class CallHistoryDataEntity
    {
        public int? Id { get; set; }
        public DateTime? FiredTime { get; set; }
        public string ProjectID { get; set; }
        public string Name { get; set; }
        public int? InterviewID { get; set; }
        public int? InterviewerID { get; set; }
        public string InterviewerName { get; set; }
        public string TelephoneNumber { get; set; }
        public short? ExtendedStatus { get; set; }
        public int? Duration { get; set; }
        public int? WaitingTime { get; set; }
        public int? CallCenterId { get; set; }
        public string CallCenterName { get; set; }
        public List<string> ReplicatedVariables { get; set; }
    }
}
