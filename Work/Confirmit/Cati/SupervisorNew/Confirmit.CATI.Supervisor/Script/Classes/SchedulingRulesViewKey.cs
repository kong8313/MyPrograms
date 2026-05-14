using System;
using Confirmit.CATI.Supervisor.Classes.Script;

namespace Confirmit.CATI.Supervisor.Script.Classes
{
    public class SchedulingRulesViewKey
    {
        private const string KeySeparator = "_";

        public SchedulingRulesViewKey(string clientKey)
        {
            if (string.IsNullOrEmpty(clientKey))
            {
                throw new ArgumentException("clientKey");
            }

            var parts = clientKey.Split(new[] { KeySeparator }, StringSplitOptions.None);

            Level = (GridBandType)(parts.Length - 1);

            if (parts.Length > 0)
            {
                RuleId =  new Guid(parts[0]);
            }

            if (parts.Length > 1)
            {
                SubRuleId =  new Guid(parts[1]);                
            }

            if (parts.Length > 2)
            {
                ActionId = int.Parse(parts[2]);
            }            
        }

        public SchedulingRulesViewKey(Guid ruleId)
        {
            RuleId = ruleId;
            Level = GridBandType.Rules;
        }

        public SchedulingRulesViewKey(Guid ruleId, Guid subRuleId): this(ruleId)
        {
            SubRuleId = subRuleId;
            Level = GridBandType.Subrules;
        }

        public SchedulingRulesViewKey(Guid ruleId, Guid subRuleId, int actionId)
            : this(ruleId, subRuleId)
        {
            ActionId = actionId;
            Level = GridBandType.Actions;
        }

        public Guid RuleId { get; private set; }

        public Guid SubRuleId { get; private set; }

        public int ActionId { get; private set; }

        public GridBandType Level { get; private set; }

        public string GetClientKey()
        {
            var key = String.Empty;

            if ((int)Level >= 0)
            {
                key = RuleId.ToString();
            }
            if ((int)Level >= 1)
            {
                key = key + KeySeparator + SubRuleId;
            }
            if ((int)Level >= 2)
            {
                key = key + KeySeparator + ActionId;
            }

            return key;
        }
    }
}