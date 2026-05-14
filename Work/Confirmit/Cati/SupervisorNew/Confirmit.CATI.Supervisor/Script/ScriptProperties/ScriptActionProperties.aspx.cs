using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Script;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Script.Classes;
using Action = Confirmit.CATI.Core.ScheduleDom.Script.Action;
using Parameter = Confirmit.CATI.Core.ScheduleDom.Scheduling.Parameter;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptActionProperties : BaseForm
    {
        private readonly IScheduleService _scheduleService;

        private readonly ISchedulingObjectValidator _validator;

        public ScriptActionProperties()
        {
            _validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }

        private IActionCollection _actionCollection;

        [StoreInViewState]
        protected int? ActionId;

        [StoreInViewState]
        protected Guid? ParentRuleId;

        [StoreInViewState]
        protected Guid? ParentSubruleId;

        [StoreInViewState]
        protected int ScheduleId;

        protected bool IsNew
        {
            get { return !ActionId.HasValue; }
        }

        public Schedule WorkingSchedule
        {
            get { return (Schedule)Session[$"WorkingSchedule_{ScheduleId}"]; }
        }

        protected RuleCollection Rules
        {
            get
            {
                return WorkingSchedule.Rules;
            }
        }

        protected ShiftTypeCollection ShiftTypeCollection
        {
            get
            {
                return WorkingSchedule.ShiftTypes;
            }
        }

        protected IActionCollection ActionCollection
        {
            get
            {
                if (_actionCollection == null)
                {
                    _actionCollection = _scheduleService.GetActions();
                }

                return _actionCollection;
            }
        }

        public CustomParameterCollection ParametersCollection
        {
            get
            {
                return WorkingSchedule.CustomParameters;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            InitControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (IsPostBack == false)
            {
                if (Request["ID"] != null)
                {
                    ScheduleId = int.Parse(Request["ID"]);
                }

                if (Request["ActionId"] != null)
                {
                    ActionId = int.Parse(Request["ActionId"]);
                }

                if (Request["ParentSubruleId"] != null)
                {
                    ParentSubruleId = Guid.Parse(Request["ParentSubruleId"]);
                }

                if (Request["ParentRuleId"] != null)
                {
                    ParentRuleId = Guid.Parse(Request["ParentRuleId"]);
                }

                if (!IsNew)
                {
                    BindData();
                    SetTitle();
                }

                foreach (var param in ParametersCollection)
                {
                    var li = new ListItem(param.Name, param.Name);
                    li.Attributes.Add("parameterType", ((int)param.Type.Value).ToString());
                    ddlSchedulingParams.Items.Add(li);
                }
            }
            
            dialog.OKButton.Text = IsNew ? "Add" : "Save";
        }

        private void InitControls()
        {

            ddlAction.Attributes["onChange"] =
                String.Format("SchedulingActionProperties.onActionChange('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                    ddlAction.ClientID,
                    taActionDescription.ClientID,
                    rblParameters.ClientID,
                    rbParam.ClientID,
                    rbConst.ClientID,
                    tbConst.ClientID,
                    ddlSchedulingParams.ClientID);

            ddlAction.Attributes["onSet"] =
                String.Format("SchedulingActionProperties.onActionSet('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')",
                    ddlAction.ClientID,
                    taActionDescription.ClientID,
                    rblParameters.ClientID,
                    rbParam.ClientID,
                    rbConst.ClientID,
                    tbConst.ClientID,
                    ddlSchedulingParams.ClientID);

            hdnIsSchedulingParam.Attributes["onChange"] =
                String.Format("SchedulingActionProperties.onActionParameterChange('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                    hdnIsSchedulingParam.ClientID,
                    hdnParamValue.ClientID,
                    tbConst.ClientID,
                    ddlSchedulingParams.ClientID,
                    rbConst.ClientID,
                    rbParam.ClientID);

            foreach (Action action in _scheduleService.GetActions())
            {
                var li = new ListItem(action.Name, action.Id.Value.ToString());

                li.Attributes.Add("hasParameter", action.HasParameter.ToString());

                li.Attributes.Add("parameterType",
                    action.ParameterType == null
                        ? ""
                        : ((int)action.ParameterType.Value).ToString());

                li.Attributes.Add("description", action.ParameterDescription);

                ddlAction.Items.Add(li);
            }

            string onclick = String.Format("SchedulingActionProperties.radioButtonClicked('{0}', '{1}', '{2}', '{3}');",
                rbConst.ClientID, rbParam.ClientID, tbConst.ClientID, ddlSchedulingParams.ClientID);

            rbConst.Attributes.Add("onclick", onclick);
            rbParam.Attributes.Add("onclick", onclick);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterActionValidationScript();
            RegisterShowValidParamsScript();
            PageHelper.RegisterClientLibraryForAsyncRequest(this, "Client/SchedulingActionProperties.js");
            RegisterStartupScript(ddlAction.Attributes["onChange"]);
            RegisterStartupScript(hdnIsSchedulingParam.Attributes["onChange"]);
        }

        /// <summary>
        /// Registers script that makes undo warning that shows when changes weren't saved.
        /// </summary>
        private void RegisterActionValidationScript()
        {
            var bFirstTime = true;
            const string scriptKey = "ActionValidationScript";

            var scriptBody = new StringBuilder();
            scriptBody.AppendLine("SchedulingActionProperties.validateAction = function(actionId, inputValue)");
            scriptBody.AppendLine("{");
            scriptBody.AppendLine("  try");
            scriptBody.AppendLine("  {");

            foreach (Action action in _scheduleService.GetActions())
            {
                string regex = GetRegexForAction(action.Id);

                if (String.IsNullOrEmpty(regex) == false)
                {
                    if (bFirstTime)
                    {
                        scriptBody.AppendLine("   if(actionId == " + action.Id.Value + ")");
                        bFirstTime = false;
                    }
                    else
                    {
                        scriptBody.AppendLine("   else if(actionId == " + action.Id.Value + ")");
                    }

                    scriptBody.AppendLine("   {");
                    scriptBody.AppendLine("    var regEx = /" + regex + "/;");
                    scriptBody.AppendLine("    return regEx.test(inputValue.trim());");
                    scriptBody.AppendLine("   }");
                }
            }
            scriptBody.AppendLine("  }");
            scriptBody.AppendLine("  catch (error){}");
            scriptBody.AppendLine("}");

            ScriptManager.RegisterStartupScript(this, GetType(), scriptKey, scriptBody.ToString(), true);

        }

        private void RegisterShowValidParamsScript()
        {
            const string scriptKey = "ShowValidParamsScript";

            var scriptBody = new StringBuilder();
            scriptBody.AppendLine("SchedulingActionProperties.showOnlyValidParams = function(paramType, ddlParamsId)");
            scriptBody.AppendLine("{");
            scriptBody.AppendLine("var ddlParams = document.getElementById(ddlParamsId);");
            scriptBody.AppendLine("switch(paramType)");
            scriptBody.AppendLine("{");
            scriptBody.AppendLine(String.Format("case '{0}': ", (int)SchedulingParameterType.Integer));
            for (var i = 0; i < ParametersCollection.Count; i++)
            {
                scriptBody.AppendLine(String.Format("ddlParams.options[{0}].disabled = false;", i));
            }
            scriptBody.AppendLine("break;");
            scriptBody.AppendLine("default:");
            for (var i = 0; i < ParametersCollection.Count; i++)
            {
                scriptBody.AppendLine(String.Format("if (ddlParams.options[{0}].attributes['Parametertype'].value == paramType)", i));
                scriptBody.AppendLine(String.Format("ddlParams.options[{0}].disabled = false;", i));
                scriptBody.AppendLine("else");
                scriptBody.AppendLine(String.Format("ddlParams.options[{0}].disabled = true;", i));
            }
            scriptBody.AppendLine("}");
            scriptBody.AppendLine("}");

            ScriptManager.RegisterStartupScript(this, GetType(), scriptKey, scriptBody.ToString(), true);
        }

        /// <summary>
        /// Returns validation regex for specified action
        /// </summary>
        /// <remarks>
        /// In future this regex can be specified directly in action.xml file
        /// </remarks>
        /// <param name="actionId"></param>
        private string GetRegexForAction(Nullable<Int32> actionId)
        {
            if (actionId.HasValue == false) return String.Empty;

            switch (actionId)
            {
                case 8: //Set time to NOW [int 0 / 1]
                    return "^[0-1]$";
                case 5: //Fulfill the specified appointment [int min = 0]
                case 19: //Place call history bookmark [int min = 0]                
                    return "^([0]|[1-9][0-9]*)+$";
                case 2:  // Recall after number of minutes [int min = 1]
                case 3:  // Recall after number of shifts
                case 4:  // Recall after number of shifts (random time
                case 25: //Set Next Rule
                case 27: //Set new Call Priority
                case 28: // Increment Priority
                case 29: // Decrement Priority
                case 31: // Set Call expiration timeout
                case 35: // Recall on the specific shift
                    return "^[1-9][0-9]*$"; // [int min = 1]
                case 7: //Recall on next shift of specified type
                    return "^[1-9][0-9]*$";
                case 16: //Assign value to quantity variable, string, quantity variable name = value
                    return @"^(\w)+\s*\=\s*([0-9])+$";
                case 18: //Assign function call result to variable, string, quantity variable name = value
                    return @"^(\w)+\s*\=\s*(\w)+$";
                case 23: //Go To
                    return @"^[1-9][0-9]*[.][1-9][0-9]*$";
                case 26: //Set new ITS [number]
                    return "^[1-9][0-9]*$";
                case 33: //Recall on specific time [dd/mm/yyyy hh:mm]
                case 34: //Set Call expiration time
                    return @"^\d{2}[/]\d{2}[/]\d{4}\s\d{2}:\d{2}$";
                case 30: // Assign Resource
                    return @"^((\-[3])|(\-[2])|(\-[1])|[0]|[1-9][0-9]*(,[1-9][0-9]*)*)$";
                case 37: // Set Shift Type [number, Shift Type ID, 0, -1]
                    return @"^((\-[1])|[0]|[1-9][0-9]*)$";
                case 38: // Set dialing mode 2 - preview or 0 - reset dialing mode
                    return @"^[025]$";
                case 41://Add additional assignment on group
                case 42://Remove specific assignment on group
                    return @"^[1-9][0-9]*(,[1-9][0-9]*)*$";
            }
            return String.Empty;
        }

        private void SetTitle()
        {            
            var subRuleNumber  =  WorkingSchedule.GetNumberByGuid(ParentSubruleId.Value);
            SetOverlayTitle(String.Format("Edit 'Action {0} of SubRule {1}'", ActionId.Value, subRuleNumber));
        }

        private void BindData()
        {
            var rule = Rules.GetItemById(ParentRuleId.Value);
            var subRule = rule.SubRules.GetItemById(ParentSubruleId.Value);
            var action = subRule.SubRuleActions.GetItemById(ActionId.Value);

            cbActionEnabled.Checked = action.Enabled;
            cbFilterEnabled.Checked = action.FilterEnabled;
            codeEditorFilter.Text = action.Filter;
            ddlAction.SelectedValue = action.ActionId.Value.ToString();
            taActionDescription.InnerText = action.Description;

            if (action.Parameter.Type == Parameter.ParamType.Constant)
            {
                rbConst.Checked = true;
                tbConst.Text = action.Parameter.Constant;
                hdnIsSchedulingParam.Value = false.ToString();
                hdnParamValue.Value = action.Parameter.Value;

                if (ActionManager.IsGoToAction(action.ActionId.Value) || ActionManager.IsSetNextRuleAction(action.ActionId.Value))
                {
                    tbConst.Text = WorkingSchedule.GetNumberByGuid(new Guid(action.Parameter.Constant));
                    hdnParamValue.Value = tbConst.Text;
                }
                else if (ActionManager.IsSetCallExpirationTime(action.ActionId.Value) ||
                         ActionManager.IsRecallOnSpecificTime(action.ActionId.Value))
                {
                    tbConst.Text = ScheduleManager.ConvertToDateTime(action.Parameter.Constant, ConvertDirection.ToClient);
                    hdnParamValue.Value = tbConst.Text;
                }                                        
            }
            else
            {
                hdnIsSchedulingParam.Value = true.ToString();

                var parameterName = ParametersCollection.GetItemById(action.Parameter.ParameterID.Value).Name;

                hdnParamValue.Value = parameterName;
                ddlSchedulingParams.SelectedValue = parameterName;
                                
                rbParam.Checked = true;
            }                        
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                ErrorCollection errors;
                var rule = Rules.GetItemById(ParentRuleId.Value);
                var subRule = rule.SubRules.GetItemById(ParentSubruleId.Value);

                var actionInfo = new ActionInfo();

                if (rbParam.Checked)
                {
                    actionInfo.ParameterValue = ddlSchedulingParams.SelectedValue;
                    actionInfo.IsSchedulingParameter = true;
                }
                else
                {
                    actionInfo.IsSchedulingParameter = false;
                    actionInfo.ParameterValue = tbConst.Text;
                }

                actionInfo.Filter = codeEditorFilter.Text;
                actionInfo.FilterEnabled = cbFilterEnabled.Checked;
                actionInfo.Enabled = cbActionEnabled.Checked;
                actionInfo.ActionId = int.Parse(ddlAction.SelectedValue);
                actionInfo.Id = ActionId;

                // if action uses custom parameters as parameter value we have temporary (for validation purposes) to substitute it with real value of custom parameter
                string tempValue = null;

                if (actionInfo.IsSchedulingParameter)
                {
                    /*If Scheduling parameter is used, parameter value contains name of the parameter.
                     But SubRuleAction must contains parameter identifier (not name)*/

                    string paramenterName = actionInfo.ParameterValue;
                    string parameterId = WorkingSchedule.CustomParameters.Where(x => x.Name == paramenterName).First().Id.ToString();

                    tempValue = parameterId;
                    actionInfo.ParameterValue = WorkingSchedule.CustomParameters.GetItemById(int.Parse(parameterId)).Value.ToString();
                }

                if (ActionManager.IsGoToAction(actionInfo.ActionId) || ActionManager.IsSetNextRuleAction(actionInfo.ActionId))
                {
                    var guid = GetGuidByParameterValue(actionInfo);

                    if (ActionManager.IsGoToAction(actionInfo.ActionId))
                    {
                        if (guid == Guid.Empty)
                        {
                            throw new ArgumentException(Strings.errSubRuleDoesntExist);
                        }

                        if (guid == subRule.Id.Value)
                        {
                            throw new ArgumentException(Strings.errNumberOfCurrentSubRule);
                        }
                    }
                    else if (ActionManager.IsSetNextRuleAction(actionInfo.ActionId))
                    {
                        if (guid == Guid.Empty)
                        {
                            throw new ArgumentException(Strings.errRuleDoesntExist);
                        }
                    }
                    actionInfo.ParameterValue = guid.ToString();
                }
                else if (ActionManager.IsSetShiftType(actionInfo.ActionId) ||
                         ActionManager.IsRecallOnNextShiftOfSpecifiedType(actionInfo.ActionId))
                {
                    int id;
                    if (!int.TryParse(actionInfo.ParameterValue, out id))
                    {
                        throw new ArgumentException(Strings.errParameterValue);
                    }

                    //user can enter 0 for "Any Valid", -1 for "None"
                    bool isNoneOrAny = (id == ActionManager.AnyValidShiftTypeId) || (id == ActionManager.NoneShiftTypeId);
                    var shiftType = ShiftTypeCollection.GetItemById(id);

                    if (ActionManager.IsSetShiftType(actionInfo.ActionId))
                    {
                        if (!isNoneOrAny && shiftType == null)
                        {
                            throw new ArgumentException(Strings.errShiftTypeNotExist);
                        }
                    }

                    if (ActionManager.IsRecallOnNextShiftOfSpecifiedType(actionInfo.ActionId))
                    {
                        if (shiftType == null)
                        {
                            throw new ArgumentException(Strings.errShiftTypeNotExist);
                        }
                    }

                    if (isNoneOrAny == false &&
                        shiftType != null &&
                        WorkingSchedule.Shifts.ContainsItemsWithShiftType(shiftType, false, new ErrorCollection()) == false)
                    {
                        throw new ArgumentException(Strings.errNoShiftsInShiftType);
                    }
                }
                else if (ActionManager.IsRecallOnSpecificTime(actionInfo.ActionId) ||
                         ActionManager.IsSetCallExpirationTime(actionInfo.ActionId))
                {
                    actionInfo.ParameterValue = ScheduleManager.ConvertToDateTime(actionInfo.ParameterValue, ConvertDirection.FromClient);
                }
                else if (ActionManager.IsRecallOnTheSpecificShift(actionInfo.ActionId))
                {
                    int shiftId = Int32.Parse(actionInfo.ParameterValue);
                    if (WorkingSchedule.Shifts.GetItemById(shiftId) == null)
                    {
                        throw new ArgumentException(Strings.errShiftNotExist);
                    }
                }

                SubRuleAction subRuleAction;
                if (IsNew == false)
                {
                    // if action uses custom parameters as parameter value we need to restore temporary substituted parameter value
                    subRuleAction = (SubRuleAction)subRule.SubRuleActions.GetItemById(actionInfo.Id.Value).Clone();
                }
                else
                {
                    subRuleAction = new SubRuleAction { Id = subRule.SubRuleActions.GetNewId() };
                }

                if (actionInfo.IsSchedulingParameter)
                {
                    actionInfo.ParameterValue = tempValue;
                }

                if (String.IsNullOrEmpty(actionInfo.ParameterValue) == false
                    && ActionCollection.GetActionById(actionInfo.ActionId).HasParameter == false)
                {
                    //if action shouldn't have a parameter but the parameter has been specified, clear it out.
                    actionInfo.ParameterValue = String.Empty;
                }

                actionInfo.FillAction(subRuleAction);

                if (_validator.Validate(subRule, out errors))
                {
                    if (IsNew)
                    {
                        subRule.SubRuleActions.Add(subRuleAction);
                    }
                    else
                    {

                        int index =
                            subRule.SubRuleActions.IndexOf(subRule.SubRuleActions.GetItemById(ActionId.Value));
                        subRule.SubRuleActions[index] = subRuleAction;
                    }

                    CloseOverlay(true);
                }
                else
                {
                    //notify user about validation errors
                    ShowClientMessage(errors[0].Message);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        /// <summary>
        /// Get guid by parameter value. Use only for GoTo or SetNextRule action type
        /// </summary>
        /// <returns>Empty guid if parameter is incorrect</returns>
        private Guid GetGuidByParameterValue(ActionInfo actionInfo)
        {
            string ruleNumberFormat = @"^[1-9]\d*$";
            string subRuleNumberFormat = @"^[1-9]\d*[.][1-9]\d*$";
            string param = actionInfo.ParameterValue.Trim();

            if (ActionManager.IsGoToAction(actionInfo.ActionId))
            {
                if (!Regex.IsMatch(param, subRuleNumberFormat))
                {
                    return Guid.Empty;
                }
            }
            else if (ActionManager.IsSetNextRuleAction(actionInfo.ActionId))
            {
                if (!Regex.IsMatch(param, ruleNumberFormat))
                {
                    return Guid.Empty;
                }
            }

            Guid guid = WorkingSchedule.GetGuidByNumber(actionInfo.ParameterValue);

            return guid;
        }

    }
}