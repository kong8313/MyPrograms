using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Confirmit.CATI.Core.ScheduleDom.Scheduling.Validators;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Script
{
    public partial class ScriptSubruleProperties : BaseForm
    {
        private IActionCollection _actionCollection;

        private readonly IScheduleService _scheduleService;

        private readonly ISchedulingObjectValidator _validator;

        public ScriptSubruleProperties()
        {
            _validator = ServiceLocator.Resolve<ISchedulingObjectValidator>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
        }

        [StoreInViewState]
        protected int ScheduleId;

        [StoreInViewState]
        protected Guid? SubRuleId;

        [StoreInViewState]
        protected Guid? ParentRuleId;

        protected bool IsNew
        {
            get { return !SubRuleId.HasValue; }
        }

        public Schedule WorkingSchedule
        {
            get { return (Schedule)Session[$"WorkingSchedule_{ScheduleId}"]; }
        }
        
        protected RuleCollection Rules
        {
            get { return WorkingSchedule.Rules; }
        }

        protected ShiftTypeCollection ShiftTypeCollection
        {
            get { return WorkingSchedule.ShiftTypes; }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["ID"] != null)
            {
                ScheduleId = int.Parse(Request["ID"]);
            }

            if (IsPostBack == false)
            {
                InitControls();

                if (Request["SubRuleId"] != null)
                {
                    SubRuleId = Guid.Parse(Request["SubRuleId"]);
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
            }

            dialog.OKButton.Text = IsNew ? "Add" : "Save";
        }

        private void SetTitle()
        {
            string number = WorkingSchedule.GetNumberByGuid(SubRuleId.Value);

            SetOverlayTitle(String.Format("Edit 'SubRule {0}'", number));
        }

        private void InitControls()
        {
            ddlITS.Items.Clear();
            ddlShiftType.Items.Clear();

            ddlITS.Items.Add(new ListItem(string.Format("({0}) {1}", 0, Strings.Any), "0"));
            foreach (BvSpState_ListEntity its in StateGroupsManager.GetITSListForScript(WorkingSchedule.Id.Value))
            {
                ListItem li = new ListItem(string.Format("({0}) ({1})", its.StateID, its.Name), its.StateID.ToString());
                ddlITS.Items.Add(li);
            }

            ddlShiftType.Items.Add(new ListItem(GetResString("Any"), "0"));
            foreach (ShiftType shiftType in ShiftTypeCollection)
            {
                if (!shiftType.IsExclusionType)
                    ddlShiftType.Items.Add(new ListItem(shiftType.Name, shiftType.Id.Value.ToString()));
            }
        }

        private void BindData()
        {
            var rule = Rules.GetItemById(ParentRuleId.Value);
            var subrule = rule.SubRules.GetItemById(SubRuleId.Value);

            cbFilterEnabled.Checked = subrule.FilterEnabled;
            codeEditorFilter.Text = subrule.Filter;
            ddlITS.SelectedValue = subrule.ItsId.ToString();
            ddlShiftType.SelectedValue = subrule.ShiftTypeId.ToString();
            tbxDescription.Text = subrule.Description;
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            try
            {
                SubRule subRule;
                ErrorCollection errors;
                Rule parentRule = null;
                if (ParentRuleId.HasValue)
                {
                    parentRule = Rules.GetItemById(ParentRuleId.Value);
                }

                if (IsNew == false)
                {
                    subRule = (SubRule)parentRule.SubRules.GetItemById(SubRuleId.Value).Clone();
                }
                else
                {
                    subRule = new SubRule { Id = parentRule.SubRules.GetNewId() };
                }

                subRule.FilterEnabled = cbFilterEnabled.Checked;
                subRule.Filter = codeEditorFilter.Text;
                subRule.ItsId = int.Parse(ddlITS.SelectedValue);
                subRule.ShiftTypeId = int.Parse(ddlShiftType.SelectedValue);
                subRule.Description = tbxDescription.Text;

                if (_validator.Validate(subRule, out errors))
                {
                    if (IsNew)
                    {
                        parentRule.SubRules.Add(subRule);
                    }
                    else
                    {
                        int index = parentRule.SubRules.IndexOf(parentRule.SubRules.GetItemById(SubRuleId.Value));
                        parentRule.SubRules[index] = subRule;
                    }

                    CloseOverlay(true);
                }
                else
                {
                    ShowClientMessage(errors[0].Message);
                }       
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}