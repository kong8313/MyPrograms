using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class UpdateQuotaLimitsEventParameters : ManagementActivityEventDetails
    {
        public int QuotaId { get; set; }
        public string QuotaName { get; set; }
        public int[] RowIds { get; set; }
        public int Limit { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateQuotaLimits)]
    public class UpdateQuotaLimitsEvent : ManagementActivityEvent<UpdateQuotaLimitsEventParameters>
    {
        public UpdateQuotaLimitsEvent(
            int surveySid,
            string projectId,
            int quotaId,
            string quotaName,
            IEnumerable<int> rowIds,
            int limit):
            base(ManagementEventCategory.Quota, ManagementEvent.UpdateQuotaLimits)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new UpdateQuotaLimitsEventParameters { QuotaId = quotaId, QuotaName = quotaName, RowIds = rowIds.ToArray(), Limit = limit };
        }
    }

    [Serializable]
    public class UpdateQuotaDisableCellFlagsEventParameters : ManagementActivityEventDetails
    {
        public int QuotaId { get; set; }
        public string QuotaName { get; set; }
        public int[] RowIds { get; set; }
        public bool NewDisableFlag { get; set; }
    }
    [ManagementEventAttribute(ManagementEvent.UpdateQuotaDisableCellFlags)]
    public class UpdateQuotaDisableCellFlagsEvent : ManagementActivityEvent<UpdateQuotaDisableCellFlagsEventParameters>
    {
        public UpdateQuotaDisableCellFlagsEvent(
            int surveySid,
            string projectId,
            int quotaId,
            string quotaName,
            IEnumerable<int> rowIds,
            bool newDisableFlag):
            base(ManagementEventCategory.Quota, ManagementEvent.UpdateQuotaDisableCellFlags)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new UpdateQuotaDisableCellFlagsEventParameters { QuotaId = quotaId, QuotaName = quotaName, RowIds = rowIds.ToArray(), NewDisableFlag = newDisableFlag };
        }
    }

    [Serializable]
    public class UpdateQuotaCellPriorityEventParameters : ManagementActivityEventDetails
    {
        public int QuotaId { get; set; }
        public string QuotaName { get; set; }
        public int[] RowIds { get; set; }
        public QuotaLimitPriority Priority { get; set; }
    }
    [ManagementEventAttribute(ManagementEvent.UpdateQuotaCellPriority)]
    public class UpdateQuotaCellPriorityEvent : ManagementActivityEvent<UpdateQuotaCellPriorityEventParameters>
    {
        public UpdateQuotaCellPriorityEvent(
            int surveySid,
            string projectId,
            int quotaId,
            string quotaName,
            IEnumerable<int> rowIds,
            QuotaLimitPriority priority):
            base(ManagementEventCategory.Quota, ManagementEvent.UpdateQuotaCellPriority)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new UpdateQuotaCellPriorityEventParameters { QuotaId = quotaId, QuotaName = quotaName, RowIds = rowIds.ToArray(), Priority = priority };
        }
    }

    [Serializable]
    public class SynchronizeQuotaEventParameters : ManagementActivityEventDetails
    {
        public string QuotaName { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SynchronizeQuota)]
    public class SynchronizeQuotaEvent : ManagementActivityEvent<SynchronizeQuotaEventParameters>
    {
        public SynchronizeQuotaEvent(int surveySid, string projectId, string quotaName):
            base(ManagementEventCategory.Quota, ManagementEvent.SynchronizeQuota)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new SynchronizeQuotaEventParameters { QuotaName = quotaName };
        }
    }

}