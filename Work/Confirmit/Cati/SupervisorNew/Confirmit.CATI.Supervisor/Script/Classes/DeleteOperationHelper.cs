using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Classes.Script;

namespace Confirmit.CATI.Supervisor.Script.Classes
{
    public class DeleteOperationHelper
    {
        private readonly Schedule _schedule;

        public DeleteOperationHelper(Schedule schedule)
        {
            _schedule = schedule;                   
        }

        public bool Delete(SchedulingRulesViewKey key, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            switch (key.Level)
            {
                case GridBandType.Rules:
                    return  DeleteRule(key.RuleId, out errors);
                case GridBandType.Subrules:
                    return  DeleteSubRule(key.RuleId, key.SubRuleId, out errors);
                case GridBandType.Actions:
                    return DeleteSubRule(key.RuleId, key.SubRuleId, key.ActionId, out errors);
            }

            return false;
        }

        private bool DeleteRule(Guid ruleId, out ErrorCollection errors)
        {
            return _schedule.Rules.RemoveById(ruleId, out errors);
        }

        private bool DeleteSubRule(Guid ruleId, Guid subRuleId, out ErrorCollection errors)
        {            
            if (_schedule.IsSubRuleUsed(subRuleId, out errors))
            {
                return false;
            }

            var rule = _schedule.Rules.GetItemById(ruleId);

            return rule.SubRules.RemoveById(subRuleId, out errors);
        }

        private bool DeleteSubRule(Guid ruleId, Guid subRuleId, int actionId, out ErrorCollection errors)
        {
            var rule = _schedule.Rules.GetItemById(ruleId);
            var subRule = rule.SubRules.GetItemById(subRuleId);

            return subRule.SubRuleActions.RemoveById(actionId, out errors);
        }
    }
}