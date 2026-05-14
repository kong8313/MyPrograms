using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Supervisor.Script.Classes;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public class EnableActionOperationHelper
    {
        private readonly Schedule _schedule;

        public EnableActionOperationHelper(Schedule schedule)
        {            
            _schedule = schedule;
        }
        public bool Enable(SchedulingRulesViewKey key, bool enable, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (key.Level != GridBandType.Actions)
            {
                return false;
            }
            
            var rule = _schedule.Rules.GetItemById(key.RuleId);
            var subRule = rule.SubRules.GetItemById(key.SubRuleId);
            var subRuleAction = (SubRuleAction)subRule.SubRuleActions.GetItemById(key.ActionId).Clone();

            subRuleAction.Enabled = enable;
            int index = subRule.SubRuleActions.IndexOf(subRule.SubRuleActions.GetItemById(subRuleAction.Id.Value));
            subRule.SubRuleActions[index] = subRuleAction;

            return true;
        }
    }
}
