using System;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace SimulatorDialerDriver.Models
{
    public class Interviewer
    {
        public Interviewer(int agentId)
        {
            AgentId = agentId;
        }

        public int AgentId { get; private set; }
        public long CampaignId { get; set; }
        public int CompanyId { get; set; }
        public int DialerId { get; set; }
        public string Name { get; set; }
        public AgentType Type { get; set; }
        public string ConnectionString { get; set; }
        public bool IsPredictive { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Attributes { get; set; }
        public int[] Groups = new int[] { };


        public string DisplayName
        {
            get
            {
                return String.Format("{0}({1})", Name, AgentId);
            }
        }
    }
}
