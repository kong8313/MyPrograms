using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Classes.Script;

namespace Confirmit.CATI.Supervisor.Script.Classes
{
    public class MoveOperationHelper
    {
        private readonly Schedule _schedule;

        public MoveOperationHelper(Schedule schedule)
        {
            _schedule = schedule;                   
        }

        public bool Move(SchedulingRulesViewKey key, bool moveUp, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            switch (key.Level)
            {
                case GridBandType.Rules:
                    return MoveRule(key.RuleId, moveUp);
                case GridBandType.Subrules:
                    return  MoveSubRule(key.RuleId, key.SubRuleId, moveUp);
                case GridBandType.Actions:
                    return MoveAction(key.RuleId, key.SubRuleId, key.ActionId, moveUp);
            }

            return false;
        }

        private bool MoveRule(Guid ruleId, bool moveUp)
        {            
            var rule = _schedule.Rules.GetItemById(ruleId);

            int index = _schedule.Rules.IndexOf(rule);

            if (moveUp)
            {
                if (index > 0)
                {
                    _schedule.Rules.Swap(index, index - 1);
                }
            }
            else
            {
                if (index < _schedule.Rules.Count - 1)
                {
                    _schedule.Rules.Swap(index, index + 1);
                }
            }
            return true;
        }

        private bool MoveSubRule(Guid ruleId, Guid subRuleId, bool moveUp)
        {
            var rule = _schedule.Rules.GetItemById(ruleId);

            var subRule = rule.SubRules.GetItemById(subRuleId);

            int index = rule.SubRules.IndexOf(subRule);

            if (moveUp)
            {
                if (index > 0)
                {
                    rule.SubRules.Swap(index, index - 1);
                }
            }
            else
            {
                if (index < rule.SubRules.Count - 1)
                {
                    rule.SubRules.Swap(index, index + 1);
                }
            }

            return true;
        }

        private bool MoveAction(Guid ruleId, Guid subRuleId, int actionId, bool moveUp)
        {
            var rule = _schedule.Rules.GetItemById(ruleId);
            var subRule = rule.SubRules.GetItemById(subRuleId);
            var subRuleAction = subRule.SubRuleActions.GetItemById(actionId);

            int index = subRule.SubRuleActions.IndexOf(subRuleAction);

            if (moveUp)
            {
                if (index > 0)
                {
                    subRule.SubRuleActions.Swap(index, index - 1);
                }
            }
            else
            {
                if (index < subRule.SubRuleActions.Count - 1)
                {
                    subRule.SubRuleActions.Swap(index, index + 1);
                }
            }

            return true;
        }
    }
}
