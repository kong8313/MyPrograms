using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfirmitDialerInterface;

namespace SimulatorDialerDriver
{
    public class Campaign
    {
        public int CompanyId { get; private set; }
        public int DialerId { get; private set; }
        public long CampaignId { get; private set; }
        public string Name { get; private set; }
        public DialingMode DialingMode { get; private set; }
        public bool RecordWholeInterview { get; private set;}

        public Campaign(int companyId, int dialerId, long campaignId, string name, DialingMode dialingMode, bool recordWholeInterview = false)
        {
            CompanyId = companyId;
            DialerId = dialerId;
            CampaignId = campaignId;
            Name = name;
            DialingMode = dialingMode;
            RecordWholeInterview = recordWholeInterview;
        }

        public override string ToString()
        {
            return $"{nameof(CompanyId)}: {CompanyId}, {nameof(DialerId)}: {DialerId}, {nameof(CampaignId)}: {CampaignId}, {nameof(Name)}: {Name}, {nameof(DialingMode)}: {DialingMode}, {nameof(RecordWholeInterview)}: {RecordWholeInterview}";
        }
    }
}
