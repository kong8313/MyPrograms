namespace SimulatorDialerDriver.Distribution
{
    public class GeneratorFilter
    {
        public int? CompanyId { get; set; }
        public int? DialerId { get; set; }
        public long? CampaignId { get; set; }
        public int? AgentId { get; set; }
        public int? InterviewId { get; set; }
        private int? _priority;
        public int Priority
        {
            get
            {
                if (_priority == null)
                {
                    _priority = 0;

                    if (CompanyId != null) _priority++;
                    if (DialerId != null) _priority++;
                    if (CampaignId != null) _priority++;
                    if (AgentId != null) _priority++;
                    if (InterviewId != null) _priority++;
                }

                return (int)_priority;
            }
        }
    }
    public class GeneratorBehavior
    {
        public string Id { get; set; }
        public GeneratorBehaviorType Type { get; set; }
        public string Owner { get; set; }
        public string Value { get; set; }
        public GeneratorFilter Filter { get; set; }
    }
}