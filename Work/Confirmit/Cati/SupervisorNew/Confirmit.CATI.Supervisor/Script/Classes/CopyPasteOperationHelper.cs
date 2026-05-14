using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Script.Classes
{
    public class CopyPasteOperationHelper
    {
        private readonly Schedule _schedule;

        public CopyPasteOperationHelper(Schedule schedule)
        {
            _schedule = schedule;            
        }

        public bool Paste(SchedulingRulesViewKey copiedRowkey, SchedulingRulesViewKey pasteRowkey, out ErrorCollection errors, out string pastedRowKey)
        {
            errors = new ErrorCollection();

            if ((int)copiedRowkey.Level - (int)pasteRowkey.Level >1)
            {
               errors.Add(new Error("Action cannot be directly copied into the rule")); 
            }

            switch (copiedRowkey.Level)
            {
                case GridBandType.Rules:
                    return PasteRule(copiedRowkey.RuleId, out pastedRowKey);                    
                case GridBandType.Subrules:
                    return PasteSubRule(copiedRowkey.RuleId, copiedRowkey.SubRuleId, pasteRowkey.RuleId, out pastedRowKey);                    
                case GridBandType.Actions:
                    return PasteAction(copiedRowkey.RuleId, copiedRowkey.SubRuleId, copiedRowkey.ActionId, pasteRowkey.RuleId, pasteRowkey.SubRuleId, out errors, out pastedRowKey);
                default:
                    throw new ArgumentException(copiedRowkey.Level.ToString());                    
            }
        }

        private bool PasteRule(Guid copiedRowRuleId, out string pastedRowKey)
        {
            var copiedRowRule = _schedule.Rules.GetItemById(copiedRowRuleId);
            
            var rule = (Rule)copiedRowRule.Clone();
            rule.SubRules.Clear();

            rule.Id = _schedule.Rules.GetNewId();
            _schedule.Rules.Add(rule);

            pastedRowKey = new SchedulingRulesViewKey(rule.Id.Value).GetClientKey();

            return true;
        }

        private bool PasteSubRule(Guid copiedRowRuleId, Guid copiedRowSubRuleId, Guid pasteRowRuleId, out string pastedRowKey)
        {
            var copiedRowRule = _schedule.Rules.GetItemById(copiedRowRuleId);
            var copiedRowSubRule = copiedRowRule.SubRules.GetItemById(copiedRowSubRuleId);
            
            var pasteRowRule = _schedule.Rules.GetItemById(pasteRowRuleId);

            var subRule = (SubRule)copiedRowSubRule.Clone();
            subRule.SubRuleActions.Clear();

            subRule.Id = pasteRowRule.SubRules.GetNewId();
                
            pasteRowRule.SubRules.Add(subRule);

            pastedRowKey = new SchedulingRulesViewKey(pasteRowRule.Id.Value, subRule.Id.Value).GetClientKey();

            return true;
        }

        private bool PasteAction(Guid copiedRowRuleId, 
                                 Guid copiedRowSubRuleId, 
                                 int copiedRowActionId, 
                                 Guid pasteRowRuleId, 
                                 Guid pastedRowSubRuleId, 
                                 out ErrorCollection errors, out string pastedRowKey)
        {
            pastedRowKey = string.Empty;

            var copiedRowRule = _schedule.Rules.GetItemById(copiedRowRuleId);
            var copiedRowSubRule = copiedRowRule.SubRules.GetItemById(copiedRowSubRuleId);

            var action = (SubRuleAction)copiedRowSubRule.SubRuleActions.GetItemById(copiedRowActionId).Clone();
            action.Id = copiedRowSubRule.SubRuleActions.GetNewId();
           
            var pasteRowRule = _schedule.Rules.GetItemById(pasteRowRuleId);
            var pasteRowSubRule = pasteRowRule.SubRules.GetItemById(pastedRowSubRuleId);

            if (ValidatePasteAction(action.ActionId.Value, action.Parameter.Value, pastedRowSubRuleId, out errors) == false)
            {
                return false;
            }

            pasteRowSubRule.SubRuleActions.Add(action);

            pastedRowKey = new SchedulingRulesViewKey(pasteRowRule.Id.Value, pasteRowSubRule.Id.Value, action.Id.Value).GetClientKey();

            return true;
        }

        private bool ValidatePasteAction(int actionId, string  actionParameterValue, Guid pastedRowSubRuleId, out ErrorCollection errors)
        {
            errors = new ErrorCollection();

            if (ActionManager.IsGoToAction(actionId) || ActionManager.IsSetNextRuleAction(actionId))
            {
                var guid = _schedule.GetGuidByNumber(actionParameterValue);                

                if (ActionManager.IsGoToAction(actionId))
                {                 
                    if (guid == pastedRowSubRuleId)
                    {
                        errors.Add(new Error(Strings.errNumberOfCurrentSubRule));
                        return false;
                    }
                }                
            }

            return true;
        }   
    }
}
