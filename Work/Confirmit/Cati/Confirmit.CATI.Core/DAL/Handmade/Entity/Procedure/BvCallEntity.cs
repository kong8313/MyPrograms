using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure
{
    public class BvCallEntity : IEquatable<BvCallEntity>
    {
        public static readonly DateTime TimeInsteadNowTimeToCall = new DateTime(1899, 12, 30);

        public int CallID { get; set; }
        public int SurveySID { get; set; }
        public int InterviewID { get; set; }
        public int CallState { get; set; }
        public int ShiftID { get; set; }
        public DateTime? TimeInShift { get; set; }
        public DateTime? TimeToExpire { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public int Resource { get; set; }
        public int ApptID { get; set; }
        public bool Lock { get; set; }
        public int TimeZoneID { get; set; }
        public DateTime TzTimeInShift { get; set; }
        public int ResourceType { get; set; }
        public Guid RuleNumber { get; set; }
        public int OldPriority { get; set; }
        public int ConditionValue { get; set; }
        public int CellId { get; set; }
        public byte DialTypeId { get; set; }
        public byte Type { get; set; }
        public int DialerId { get; set; }
        public long ActiveDialId { get; set; }

        public BvCallEntity()
        {
            this.CallState = 2;
            TimeToExpire = new DateTime(9999, 1, 1);
            Priority = 1;
            OldPriority = 0;
            ShiftID = (int)CallShiftType.None;
            Type = (byte)CallTypes.Outbound;
        }

        public BvCallEntity Copy()
        {
            return new BvCallEntity()
            {
                CallID = this.CallID,
                SurveySID = this.SurveySID,
                InterviewID = this.InterviewID,
                CallState = this.CallState,
                ShiftID = this.ShiftID,
                TimeInShift = this.TimeInShift,
                TimeToExpire = this.TimeToExpire,
                Priority = this.Priority,
                Status = this.Status,
                Resource = this.Resource,
                ApptID = this.ApptID,
                Lock = this.Lock,
                TimeZoneID = this.TimeZoneID,
                TzTimeInShift = this.TzTimeInShift,
                ResourceType = this.ResourceType,
                RuleNumber = this.RuleNumber,
                OldPriority = this.OldPriority,
                ConditionValue = this.ConditionValue,
                CellId = this.CellId,
                DialTypeId = this.DialTypeId,
                Type = this.Type,
                DialerId = this.DialerId,
                ActiveDialId = this.ActiveDialId
            };
        }

        public bool Equals(BvCallEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return  //CallID == other.CallID &&
                    SurveySID == other.SurveySID &&
                    InterviewID == other.InterviewID &&
                    CallState == other.CallState &&
                    ShiftID == other.ShiftID &&
                    TimeInShift.Equals(other.TimeInShift) &&
                    TimeToExpire.Equals(other.TimeToExpire) &&
                    Priority == other.Priority &&
                    Status == other.Status &&
                    Resource == other.Resource &&
                    ApptID == other.ApptID &&
                    Lock == other.Lock &&
                    TimeZoneID == other.TimeZoneID &&
                    TzTimeInShift.Equals(other.TzTimeInShift) &&
                    ResourceType == other.ResourceType &&
                    RuleNumber.Equals(other.RuleNumber) &&
                    OldPriority == other.OldPriority &&
                    ConditionValue == other.ConditionValue &&
                    CellId == other.CellId &&
                    DialTypeId == other.DialTypeId &&
                    Type == other.Type &&
                    DialerId == other.DialerId &&
                    ActiveDialId == other.ActiveDialId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BvCallEntity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CallID;
                hashCode = (hashCode * 397) ^ SurveySID;
                hashCode = (hashCode * 397) ^ InterviewID;
                hashCode = (hashCode * 397) ^ CallState;
                hashCode = (hashCode * 397) ^ ShiftID;
                hashCode = (hashCode * 397) ^ TimeInShift.GetHashCode();
                hashCode = (hashCode * 397) ^ TimeToExpire.GetHashCode();
                hashCode = (hashCode * 397) ^ Priority;
                hashCode = (hashCode * 397) ^ Status;
                hashCode = (hashCode * 397) ^ Resource;
                hashCode = (hashCode * 397) ^ ApptID;
                hashCode = (hashCode * 397) ^ Lock.GetHashCode();
                hashCode = (hashCode * 397) ^ TimeZoneID;
                hashCode = (hashCode * 397) ^ TzTimeInShift.GetHashCode();
                hashCode = (hashCode * 397) ^ ResourceType;
                hashCode = (hashCode * 397) ^ RuleNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ OldPriority;
                hashCode = (hashCode * 397) ^ ConditionValue;
                hashCode = (hashCode * 397) ^ CellId;
                hashCode = (hashCode * 397) ^ DialTypeId.GetHashCode();
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                hashCode = (hashCode * 397) ^ DialerId.GetHashCode();
                hashCode = (hashCode * 397) ^ ActiveDialId.GetHashCode();
                return hashCode;
            }
        }
    }
}
