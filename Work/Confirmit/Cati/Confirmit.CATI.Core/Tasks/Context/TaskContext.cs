using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Tasks
{
    public class TaskContext
    {
        public TaskContext()
        {
            DialHistories = new List<TaskDialHistory>();
        }

        public List<TaskDialHistory> DialHistories { get; set; }

        public TransferOptions TransferOptions { get; set; }

        public long? ActiveDialId { get; set; }

        public string TransferId { get; set; }

        public DateTime? ActiveDialStart { get; set; }
        public string ActiveDialDialerCallerId { get; set; }
        public string ActiveDialTelephoneNumber { get; set; }
        public int? ActiveDialRingTime { get; set; }
        public CallOutcome? ActiveDialCallOutcome { get; set; }
        public Dictionary<string, string> ActiveDialCallOutcomeMetadata { get; set; }

        public bool? IsLiveMonitoringEnabled { get; set; }

        public long CurrentCampaignId { get; set; }

        public void Clear()
        {
            ActiveDialId = null;
            TransferId = null;
            ActiveDialStart = null;
            TransferOptions = null;
            IsLiveMonitoringEnabled = null;
            DialHistories.Clear();
        }

        public TaskContext Clone()
        {
            return new TaskContext() {
                DialHistories = new List<TaskDialHistory>(DialHistories),
                TransferOptions = TransferOptions == null
                    ? null
                    : new TransferOptions {
                        Resource = TransferOptions.Resource,
                        Type = TransferOptions.Type,
                        AllowInterviewing = TransferOptions.AllowInterviewing
                    },
                ActiveDialId = ActiveDialId,
                ActiveDialStart = ActiveDialStart,
                IsLiveMonitoringEnabled = IsLiveMonitoringEnabled,
                ActiveDialRingTime = ActiveDialRingTime,
                ActiveDialDialerCallerId = ActiveDialDialerCallerId,
                ActiveDialCallOutcomeMetadata = ActiveDialCallOutcomeMetadata,
                ActiveDialCallOutcome = ActiveDialCallOutcome,
                ActiveDialTelephoneNumber = ActiveDialDialerCallerId
            };
        }
    }

    public class TaskDialHistory
    {
        public long DialId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string DialerCallerId { get; set; }
        public string TelephoneNumber { get; set; }
        public CallOutcome? DialerCallOutcome { get; set; }
        public int? RingTime { get; set; }
        public Dictionary<string, string> CallOutcomeMetadata { get; set; }

        public TaskDialHistory Clone()
        {
            return new TaskDialHistory {
                DialId = DialId,
                StartTime = StartTime,
                FinishTime = FinishTime,
                DialerCallerId = DialerCallerId,
                RingTime = RingTime,
                CallOutcomeMetadata = CallOutcomeMetadata,
                DialerCallOutcome = DialerCallOutcome,
                TelephoneNumber = TelephoneNumber
            };
        }
    }
}